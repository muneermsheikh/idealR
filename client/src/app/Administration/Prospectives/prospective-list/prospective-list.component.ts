import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap, tap } from 'rxjs';
import { ICallRecordResult } from 'src/app/_dtos/admin/callRecordResult';
import { CallRecordStatusReturnDto } from 'src/app/_dtos/admin/callRecordStatusReturnDto';
import { IProspectiveBriefDto } from 'src/app/_dtos/hr/prospectiveBriefDto';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { Pagination } from 'src/app/_models/pagination';
import { prospectiveCandidateParams } from 'src/app/_models/params/hr/prospectiveCandidateParams';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { ProspectiveService } from 'src/app/_services/hr/prospective.service';
import { CallRecordsEditModalComponent } from 'src/app/callRecords/call-records-edit-modal/call-records-edit-modal.component';

@Component({
  selector: 'app-prospective-list',
  templateUrl: './prospective-list.component.html',
  styleUrls: ['./prospective-list.component.css']
})
export class ProspectiveListComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  @ViewChild('discussions', {static: false}) discussionTerm: ElementRef | undefined;
  user?: User;
  returnUrl = '';

  prospectives: IProspectiveBriefDto[]=[];
  prospectiveSelected: IProspectiveBriefDto|undefined;

  pagination: Pagination | undefined;
  bsModalRef: BsModalRef | undefined;
  
  totalCount=0;
  //pParams = new CallRecordParams();
  pParams = new prospectiveCandidateParams();

  statusSelected: string='';
  
  paramsStatus: ICallRecordResult[] = [{status:"All"}, {status: "Active"}, {status: "Declined"}, {status: "Interested"}];

  callRecordStatus: ICallRecordResult[] = [{status: "wrong number"}, {status: "Not Responding"}, {status: "Will Revert later"},
    {status: "Declined-Family issues"}, {status: "Declined for overseas"}, {status: "Declined-Low remuneration"},
    {status: "Declined - SC Not agreed"}, {status: "Interested - to negotiate remuneration"},
    {status: "Interested, and keen"}, {status: "Interested, but doubtful"}]

  constructor(private service: ProspectiveService, private modalService: BsModalService,
    private router: Router, private toastr: ToastrService, private confirm: ConfirmService){
    let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
        }
  }

  ngOnInit(): void {
      this.loadProspectives();
  }
  
  
  onPageChanged(event: any){
    const params = this.service.getParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event.page;
      this.service.setParams(params);

      this.loadProspectives();
    }
  }

  setParameters() {
    var params = new prospectiveCandidateParams();
    params.statusClass=this.pParams.statusClass;
    this.service.setParams(params);
    this.loadProspectives();
  }
  

  loadProspectives() {
    var params = this.service.getParams();
    this.service.getProspectivesPaged(params)?.subscribe({
    next: response => {
      if(response !== undefined && response !== null) {
        this.prospectives = response.result;
        this.totalCount = response?.count;
        this.pagination = response.pagination;
      } else {
        console.log('response is undefined');
      }
    },
    error: error => console.log(error)
   })
   
  }

  selectedClicked(item: any) {
    if(item.checked===true) {
      this.prospectives.filter(x => x.id != item.id).forEach(x => x.checked=false);  
    } else {
      this.prospectives.forEach(x => x.checked=false);
    }
    this.prospectiveSelected = item.checked ? undefined : item;

  }
  
  selected(status: any) {
    this.statusSelected=status;
  }

  deleteProspectiveClicked(event: any)  //event:prospectiveId
  {
    var id=event;
    var confirmMsg = 'confirm delete this prospective Candidate?. WARNING: this cannot be undone';

    const observableInner = this.service.deleteProspectiveRecord(id);
    const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap((confirmed) => {
          return observableInner
        })
    ).subscribe(response => {
      if(response) {
        this.toastr.success('Prospective Candidate deleted', 'deletion successful');
        var index = this.prospectives.findIndex(x => x.id == id);
        if(index >= 0) this.prospectives.splice(index,1);
      } else {
        this.toastr.error('Error in deleting the checklist', 'failed to delete')
      }
      
    });

  }

  editCallRecord(event: any, item: IProspectiveBriefDto) {

    {if(event === null) {
        this.toastr.warning('No Call Record object returned from the modal form');
        return;
      }  

      const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          callRecord: event,
          contactResults: this.callRecordStatus,
          candidateName: item.candidateName,
        }
      }
          
      
      this.bsModalRef = this.modalService.show(CallRecordsEditModalComponent, config);
      const observableOuter = this.bsModalRef.content.passCallRecordEvent;
      
      observableOuter.pipe(
          filter((obj: ICallRecord) => obj !== null),
          switchMap((obj: ICallRecord) => {
            return this.service.updateCallRecord(obj)
          })
      ).subscribe((response: CallRecordStatusReturnDto) => {
        if(response) {
          this.toastr.success('Call Record updated', 'Update successful');
          var index = this.prospectives.findIndex(x => x.id === item.id);
          if(index !== -1) {
            item.status = response.status;
            this.prospectives[index]=item;
          }
        } else {
          this.toastr.error('Error in deleting the checklist', 'failed to delete')
        }
        
      });
    }
  }
    
  convertProspectiveToCandidate(event: number) {
    var id=event;

    var confirmMsg = 'this will convert the selected prospective candidate to a candidate, and remove ' +
      'it from this prospectives list. WARNING: this cannot be undone';

    const observableInner = this.service.convertProspectiveToCandidate(id);
    const observableOuter = this.confirm.confirm('confirm Convert Prospective To Candidate', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => observableInner.pipe(
          catchError(err => {
            return of();
          }),
          tap(res => {
            if(res === 0) {
              this.toastr.warning('Failed to convert the prospective to candidate', 'Failed to convert')
            } else {
              this.toastr.success('Converted the prospective to candidate, with Application No ' + res, 'success');
              var index = this.prospectives.findIndex(x => x.id);
              console.log('index to remove', index);
              if(index >=0) this.prospectives.slice(index,1);
            }
          })
        )
        )
    ).subscribe(applicationNo => {
      if(applicationNo ) {
        this.toastr.success('Prospective Candidate converted, with application No ' + applicationNo, 'Conversion successful');
          var index = this.prospectives.findIndex(x => x.id==id);
          console.log('index to remove in subscribe', index);
          if(index >=0) this.prospectives.splice(index,1);
      } else {
        this.toastr.error('Error in converting the prospective candidate to a Candidate', 'failed to convert')
      }
      
    });
  }
  
  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.loadProspectives();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    //this.pParams = new CallRecordParams();
    this.pParams = new prospectiveCandidateParams();
    this.service.setParams(this.pParams);
    this.loadProspectives();
  }

}
