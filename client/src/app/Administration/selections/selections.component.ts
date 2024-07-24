import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { ISelDecisionDto } from 'src/app/_dtos/admin/selDecisionDto';
import { ISelectionDecision } from 'src/app/_models/admin/selectionDecision';
import { Pagination } from 'src/app/_models/pagination';
import { SelDecisionParams } from 'src/app/_models/params/Admin/selDecisionParams';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { SelectionService } from 'src/app/_services/hr/selection.service';
import { EmploymentModalComponent } from './employment-modal/employment-modal.component';
import { IOfferConclusioDto } from 'src/app/_dtos/admin/offerConclusionDto';
import { SelectionModalComponent } from './selection-modal/selection-modal.component';
import { IEmployment } from 'src/app/_models/admin/employment';


@Component({
  selector: 'app-selections',
  templateUrl: './selections.component.html',
  styleUrls: ['./selections.component.css']
})
export class SelectionsComponent implements OnInit {
  
  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  selections: ISelDecisionDto[]=[];
  selectionsSelected: ISelDecisionDto[]=[];
   
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
      private router: Router,
      private activatedRoute: ActivatedRoute,
      private bsModalService: BsModalService) { }

  ngOnInit(): void {
    
    var id=this.activatedRoute.snapshot.paramMap.get('id');
    
    if(id !=='' && id !== '0') this.sParams.orderId=+id!;

    this.getSelectionsPaged(false);
    
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
    return this.service.deleteSelectionDecision(id).subscribe(response => {
      this.toastr.success('the chosen selection deleted');
    }, error => {
      this.toastr.error(error);
    })
  }

 displayEmploymentModal(employment: any, empItem: ISelDecisionDto){

    if(employment === null) {
      this.toastr.warning('No Employment object available for the candidate');
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
        switchMap((response: IEmployment) =>  {
          
          var index = this.selections.findIndex(x => x.id == empItem.id);
          if(index >= 0) {
            this.selections[index].selectionStatus=response.offerAccepted;
          }
          
          let result = new Date(response.offerAcceptanceConcludedOn);
          result.setHours(result.getHours() + 9);
          response.offerAcceptanceConcludedOn = result;
            //bvz inexplicably, the date on reaching api is preponed by 8:30 hours, thereby making it a previous day
          return this.service.updateEmployment(response)    //the modal form emits edited IEmployment object
        })
      ).subscribe((response: boolean) => {
  
        if(response) {
          this.toastr.success('Employment updated', 'Success');
  
        } else {
          this.toastr.warning('Failed to update the Employment Object', 'Failure');
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
 
  
  selectedClicked(item: any) {

    var index = this.selectionsSelected.findIndex(x => x.id === item.id);
    if(index === -1) {
      this.selectionsSelected.push(item)
    } else {
      if(item.checked)   {
        this.selectionsSelected[index].checked=true
      } else {
        this.selectionsSelected.splice(index,1,item)
      }
    }
    console.log(this.selectionsSelected);
  }

  sendReminders() {
      var cvrefids = this.selectionsSelected.map(x => x.cvRefId);
      this.service.remindCandidatesForAcceptance(cvrefids).subscribe({
        next: response => {
          if(response ==='' || response === null) {
            this.selections.map(x => x.checked=false);
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
}
