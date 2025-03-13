import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap, tap } from 'rxjs';
import { ISelDecisionDto } from 'src/app/_dtos/admin/selDecisionDto';
import { ISelectionDecision } from 'src/app/_models/admin/selectionDecision';
import { Pagination } from 'src/app/_models/pagination';
import { SelDecisionParams } from 'src/app/_models/params/Admin/selDecisionParams';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { SelectionService } from 'src/app/_services/hr/selection.service';
import { IOfferConclusioDto } from 'src/app/_dtos/admin/offerConclusionDto';
import { EmploymentModalComponent } from './employment-modal/employment-modal.component';
import { SelectionModalComponent } from './selection-modal/selection-modal.component';
import { ISelectionStatus } from '../_models/admin/selectionStatus';


@Component({
  selector: 'app-selections',
  templateUrl: './selections.component.html',
  styleUrls: ['./selections.component.css']
})
export class SelectionsComponent implements OnInit {
  
  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  selections: ISelDecisionDto[]=[];
  selectionStatus: ISelectionStatus[]=[];
   
  pagination: Pagination | undefined;
  user?: User;
  
  //employmentsDto: IEmploymentDto[]=[];
  
  sParams = new SelDecisionParams();
  totalCount = 0;

  pageIndex=1;
    
  todayDate = new Date(Date.now());
  //statusSelected=10;

  loading=false;

  //form: FormGroup = new FormGroup({});
  bsModalRef?: BsModalRef;

  //boolean 
  SelMsgsToCandidates=false;
  SelSMSToCandidates=false;
  RejMsgsToCandidates=false;
  RejSMSToCandidates=false;
  MsgsToClient=false;
  
  selectionStatuses =[
    {name: 'Selected'}, {name: 'Rejected-Low Salary'}, {name: 'Rejected-Low Exp'}, {name: 'Rejected-irrelevant Exp'},
    {name: 'Rejected-other reasons'}
  ]

  constructor(private service: SelectionService, 
      private confirmService: ConfirmService,
      private toastr: ToastrService, 
      //private router: Router,
      private activatedRoute: ActivatedRoute,
      private bsModalService: BsModalService) { }

  ngOnInit(): void {

    this.activatedRoute.data.subscribe(data => {
        this.selections = data['selections'].result;
        this.pagination = data['selections'].pagination;
        this.totalCount = data['selections'].count;
    })
        
  }

  getSelectionsPaged(useCache: boolean) {
    this.service.setParams(this.sParams);
    this.service.getSelectionRecords(useCache).subscribe({
      next: response => {
        if(response.result && response.pagination) {
          this.selections = response.result;
          this.pagination = response.pagination;
          this.totalCount = response.count;
        }
      },
      error: error => console.log(error)
    });
  }

  registerCandidateAcceptance(id: any) {    //id is selDEcisionId

    var accepted: IOfferConclusioDto;

  }

  updateSelection(sel: any) {     //sel is selDecisionnDto
    return this.service.editSelectionDecision(sel).subscribe(() => {
      this.toastr.success('selection decision updated');
    }, error => {
      this.toastr.error(error);
    })
  }

  remindCandidateForAcceptances(cvrefids: number[]) {   //id is seldecisionid
      this.service.remindCandidatesForAcceptance(cvrefids).subscribe({
        next: response => {
          if(response === '') {
            this.toastr.success('Messages composed and saved.  You can view/edit/send these messages from the Messages Component', 'Messages composed')
          } else {
            this.toastr.warning('Failed to generate/save Messages', 'Failed')
          }
        },
        error: err => {
          if(err?.error?.error) {
            this.toastr.error(err?.error?.error, 'Error encountered')
          } else {
            this.toastr.error(err, 'Error encountered')
          }
        }
      })
  }

  deleteSelection(id: any) {

      var candidatesToDelete = this.selections.filter(x => x.id == id).map(x => x.candidateName);

      const observableInner = this.service.deleteSelectionDecisions(id);
      
      var messagePrompt = 'This will delete selection records of ' + candidatesToDelete + '. along with all related records like ' +
        'Employments and all deployment records.  Or, depending upon settings, the deletion might fail ' +
        'if there are related records';
      const observableOuter = this.confirmService.confirm('Confirm Delete', messagePrompt);

      observableOuter.pipe(
        filter((confirmed: boolean) => 
            confirmed===true
        ),
          switchMap(confirmed => observableInner.pipe(
            catchError(err => {
              this.toastr.error(err.error.details, 'Error in deleting the selection record');
              console.log('Error in deleting the Selection', err);
              return of();
            }),
            tap(res => {
              this.toastr.success('deleted Selection records', 'Success');
              //cvrefids.forEach(n => {
                var index = this.selections.findIndex(x => x.id === id);
                if(index === -1) {
                  this.toastr.warning('cannot find index of the selection record just deleted')
                } else {
                  this.selections.splice(index, 1);
                }
              //})
            }
            ),
          )),
          catchError(err => {
            this.toastr.error(err.error.details, 'Error in deleting the selection');
            return of();
          })
        ).subscribe(
            () => {
              console.log('delete succeeded');
              this.toastr.success('Selection record deleted');
            },
            (err: any) => {
              console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
          })
    }
    

  displayEmploymentModal(employment: any, empItem: ISelDecisionDto){

    if(employment === null) {
      this.toastr.warning('No Employment data could be retrieved from the database');
      return;
    }  

    const config = {
        class: 'modal-dialog-centered modal-md',
        initialState: {
          emp: employment,
          candidateName: empItem.applicationNo + '-' + empItem.candidateName,
          categoryRef: empItem.categoryRef,
          companyName: empItem.customerName
          //, username: this.user?.userName
        }
      }

      this.bsModalRef = this.bsModalService.show(EmploymentModalComponent, config);

      const observableOuter =  this.bsModalRef.content.updateEmp;
      
      observableOuter.pipe(
        filter((response: any) => response !==null),
        switchMap((response: any) =>  {          
          var index = this.selections.findIndex(x => x.id == empItem.id);
          if(index >= 0 && this.selections[index].selectionStatus !== response.selectionStatus) {
            this.selections[index].selectionStatus=response.selectionStatus;
          }
          
          let result = new Date(response.offerAcceptanceConcludedOn);
          result.setHours(result.getHours() + 9);
          response.offerAcceptanceConcludedOn = result;
            //bcz inexplicably, the date on reaching api is preponed by 8:30 hours, thereby making it a previous day
          return this.service.updateEmploymentWithUploads(response)    //the modal form emits edited IEmployment object + file uploaded, if any. 
        })
      ).subscribe((id: number) => {
        if(id > 0) {
          this.toastr.success('Employment updated', 'Success');
          var index = this.selections.findIndex(x => x.id == empItem.id);
          if(index !== -1) {
            this.selections[index].employmentId = id;
            console.log(empItem);
            this.selections[index].selectionStatus = empItem.selectionStatus;
          }
        } else {
            this.toastr.error("Failed to update the employment data", "error encountered")
        }
        
        error: (err: any) => { 
          this.toastr.error(err.error.details, 'Error encountered')
        }
                
      })
        
  }
  
  displaySelectionModal(selDecision: any, sel: ISelDecisionDto){    //value recd is SelDecisionDto object

    const config = {
      class: 'modal-dialog-centered modal-md',
      initialState: {
        sel: selDecision,
        candidateName: sel.applicationNo + '-' + sel.candidateName,
        customerName: sel.customerName,
        categoryRefAndName: sel.categoryRef
      }
    }

    this.bsModalRef = this.bsModalService.show(SelectionModalComponent, config);
    const observableOuter = this.bsModalRef.content.updateObj;

    observableOuter.pipe(
      filter((response: any) => response !==null),
      switchMap((response: ISelectionDecision) =>  {
        
        var index = this.selections.findIndex(x => x.id == sel.id);
        if(index >= 0) {
          
          this.selections[index].selectedOn=response.selectedOn;
          this.selections[index].selectionStatus=response.selectionStatus;
        }
        response.selectedOn.setHours(response.selectedOn.getHours()+9); //bvz inexplicably, the date on reaching api is preponed by 8:30 hours, thereby making it a previous day
        return this.service.editSelectionDecision(response)
      })
    ).subscribe((response: boolean) => {

      if(response) {
        this.toastr.success('Selection model edited', 'Success');

      } else {
        this.toastr.warning('Failed to update the selection model', 'Failure');
      }
      
      
    })

  }
 
  /*sendReminders(event: any) {     //selection.id

      var cvrefids: number[]=[];
      cvrefids.push(event);

      this.service.remindCandidatesForAcceptance(cvrefids).subscribe({
        next: (response: string) => {

          if(response ==='' || response === null ) {
            this.toastr.success('Acceptance reminder email messages composed.  You can view/edit these messages in the MESSAGES section', 'success')
          } else {
            this.toastr.warning(response, 'Failed to compose reminder acceptance email messages')
          }

        }, error : err => {
          if(err?.error?.error) {
            this.toastr.error(err?.error?.error, 'Error encountered')
          } else {
            this.toastr.error(err, 'Error encountered')
          }
        }
      })
  }
  */

  onPageChanged(event: any){
    const params = this.service.getParams();
    if (this.sParams.pageNumber !== event.page) {
      this.sParams.pageNumber = event.page;
      this.service.setParams(this.sParams);
      this.getSelectionsPaged(true);
    }
  }

  onSortSelected(event: any) {
    var sort = event.value;

    const prms = this.service.getParams();
    prms.sort = sort;
    prms.pageNumber=1;
    prms.pageSize=10;
    this.service.setParams(prms);
    this.getSelectionsPaged(true);
  }

  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.getSelectionsPaged(true);
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new SelDecisionParams();
    this.service.setParams(this.sParams);
    this.getSelectionsPaged(true);
  }

  DoHousekeeping() {
    this.service.Housekeeping().subscribe({
      next: (response: string) => {
        console.log('response housekeeping', response);
        
        if(response==='') {
          this.toastr.success('houskeeping done for processes CVRef and Selections', 'success')
        } else {
          this.toastr.warning(response, 'failed to do housekeeping')
        }
      }, error: (err: any) => {
        this.toastr.error(err, "Failed to carry out the housekeeping for CVRef and Selections")
      }
    })
  }

  
}
