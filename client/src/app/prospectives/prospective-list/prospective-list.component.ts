import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap, take, tap } from 'rxjs';
import { ICallRecordResult } from 'src/app/_dtos/admin/callRecordResult';
import { CallRecordStatusReturnDto } from 'src/app/_dtos/admin/callRecordStatusReturnDto';
import { IProspectiveBriefDto } from 'src/app/_dtos/hr/prospectiveBriefDto';
import { IProspectiveHeaderDto } from 'src/app/_dtos/hr/prospectiveHeaderDto';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { Pagination } from 'src/app/_models/pagination';
import { prospectiveCandidateParams } from 'src/app/_models/params/hr/prospectiveCandidateParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
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
  @ViewChild('searchPhoneNo', {static: false}) searchPhoneNo: ElementRef | undefined;
  @ViewChild('searchName', {static: false}) searchTermName: ElementRef | undefined;

  @ViewChild('discussions', {static: false}) discussionTerm: ElementRef | undefined;
  user?: User;
  returnUrl = '';

  prospectives: IProspectiveBriefDto[]=[];
  printProspectives: IProspectiveBriefDto[]=[];

  prospectiveSelected: IProspectiveBriefDto|undefined;

  pagination: Pagination | undefined;
  bsModalRef: BsModalRef | undefined;
  
  totalCount=0;
  //pParams = new CallRecordParams();
  pParams = new prospectiveCandidateParams();
  
  statusSelected: string='';
  categoryReferences: string[]=[];

  ProspList: IProspectiveBriefDto[]=[];

  headers: IProspectiveHeaderDto[]=[];    //contains only Orderno
  printtitle: string='';
  isPrintPDF = false;
  distinctRefCats: string[]=[];
  distinctRefCat = '';
  

  paramsStatus: ICallRecordResult[] = [{status:"All"}, {status: "Active"}, {status: "Declined"}, {status: "Interested"}];

  callRecordStatus: ICallRecordResult[] = [{status: "wrong number"}, {status: "Not Responding"}, {status: "Will Revert later"},
    {status: "Declined-Family issues"}, {status: "Declined for overseas"}, {status: "Declined-Low remuneration"},
    {status: "Declined - SC Not agreed"}, {status: "Interested - to negotiate remuneration"},
    {status: "Interested, and keen"}, {status: "Interested, but doubtful"}]

  constructor(private service: ProspectiveService, private modalService: BsModalService,
    private router: Router, private toastr: ToastrService, private confirm: ConfirmService,
    private activatedRoute: ActivatedRoute, private accountService: AccountService){
    let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
            if(!this.user) this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
        }
  }

  ngOnInit(): void {
      this.activatedRoute.data.subscribe(data => {
        
          this.prospectives = data['prospectives'].result;
          this.pagination = data['prospectives'].pagination;
          this.totalCount = data['prospectives'].totalCount;
          this.headers = data['headers'];

      })
  }
  
      /*getHeadersDto(status:string) {
        return this.service.getProspectiveHeadersDto(status).subscribe({
          next: (response: IProspectiveHeaderDto[]) => {
            this.headers = response;
          }
        })
      }
    */
  
  onPageChanged(event: any){
    const params = this.service.getParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event.page;
      this.service.setParams(params);

      this.loadProspectives();
    }
  }

  refreshDataset() {

    this.service.setParams(this.pParams);
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
            userName: this.user?.userName
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
                if(index >=0) this.prospectives.slice(index,1);
              }
            })
          )
          )
      ).subscribe(applicationNo => {
        if(applicationNo ) {
          this.toastr.success('Prospective Candidate converted, with application No ' + applicationNo, 'Conversion successful');
            var index = this.prospectives.findIndex(x => x.id==id);
            if(index >=0) this.prospectives.splice(index,1);
        } else {
          this.toastr.error('Error in converting the prospective candidate to a Candidate', 'failed to convert')
        }
        
      });
    }
    
    onSearch() {
      const params = this.service.getParams();
      if(this.searchTerm?.nativeElement.value !=='') params.search = this.searchTerm?.nativeElement.value;
      if(this.searchPhoneNo?.nativeElement.value !=='') params.search = this.searchPhoneNo?.nativeElement.value;
      if(this.searchTermName?.nativeElement.value !=='') params.search = this.searchTermName?.nativeElement.value;
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

    distinctRefChanged() {
      if (this.headers.length === 0) {
        this.service.getProspectiveHeadersDto(this.pParams.statusClass).subscribe({
          next: (response: IProspectiveHeaderDto[]) => {
            if(response !== null && response.length > 0) {
              this.headers = response;
              this.isPrintPDF = true
            } else {
              this.toastr.warning('Failed to retrieve headers', 'Failed')
            }
          }
        })
      }
    }
      
    /* generatePDF() {

      if(this.distinctRefCat !== '' && this.pParams.statusClass !== '') {
        var prospcs = this.prospectives.filter(x => 
            x.categoryRef.startsWith(this.distinctRefCat) && x.status?.startsWith(this.pParams.statusClass));
        
        if(prospcs.length === 0) {
          this.toastr.warning('Your filtered conditions did not return any record');
          return;
        }

        const doc = new jsPDF('p', 'px', 'a4');
        var YPos=10;
        prospcs.forEach(x => {
            var XPos = 10;
            doc.text(x.candidateName, XPos+=150, YPos);
            doc.text(x.categoryRef, XPos+=150, YPos);
            doc.text(x.customerName, XPos+=100, YPos);
            doc.text(x.phoneNo, XPos+=75, YPos);
            doc.text(x.nationality, XPos+=75, YPos);
            doc.text(x.dateRegistered.toDateString().substring(4,10), XPos+=75, YPos);
            doc.text(x.statusDate.toDateString().substring(4,10), XPos, YPos);
      
            YPos +=15;
          })
    
      var today = new Date();
      var fileNameSaved = "Prospective" + today.getFullYear + today.getMonth + today.getDate + today.getHours;
      doc.save(fileNameSaved);
    
      if(fileNameSaved !== '') this.toastr.success('PDF Saved with the name ' + fileNameSaved, 'Success')
      }
    }
    */

    generatePDF() {
      this.isPrintPDF = !this.isPrintPDF;

      var orderno = this.distinctRefCat;    //seleted value
      var status = this.pParams.statusClass;

      this.printtitle =  "Prospective Candidate Availability Status as on " + new Date() + "<br>For Order No. " + orderno + ", status = " + status;
        
      var params = this.service.getParams();
      
      this.service.getProspectivesList(orderno, status).subscribe({
          next: (response: IProspectiveBriefDto[]) => {
          if(response !== null && response.length > 0) {
            this.printProspectives = response
          } else {
            console.log('response is undefined');
          }
        },
        error: error => console.log(error)
      })

      //this.router.navigateByUrl("/prospectives/pdf/001012/'active'")
    }

    closePrintSection() {
      this.isPrintPDF = false;
      this.distinctRefCat = '';
    }
}

   

