import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastRef, ToastrService } from 'ngx-toastr';
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

@Component({
  selector: 'app-selections',
  templateUrl: './selections.component.html',
  styleUrls: ['./selections.component.css']
})
export class SelectionsComponent implements OnInit {
  
  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  selections: ISelDecisionDto[]=[];
   
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
    this.getSelectionsPaged(this.sParams);
  }

  getSelectionsPaged(oParams: SelDecisionParams) {
    this.service.getSelectionRecords(oParams).subscribe({
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

  editSelection(sel: any) {     //sel is selDecisionnDto
    return this.service.editSelectionDecision(sel).subscribe(() => {
      this.toastr.success('selection decision updated');
    }, error => {
      this.toastr.error(error);
    })
  }

  remindCandidateForDecision(id: any) {   //id is seldecisionid

  }

  deleteSelection(id: number) {
    return this.service.deleteSelectionDecision(id).subscribe(response => {
      this.toastr.success('the chosen selection deleted');
    }, error => {
      this.toastr.error(error);
    })
  }

 displayEmploymentModal(event: any){

    var id = event;
    if(id === 0) {
      this.toastr.warning('invalid selection decision id');
      return;
    }

    const observableOuter = this.service.getEmploymentFromSelectionId(id);
    
    observableOuter.pipe(
      filter((response: any) => response !==null),
      switchMap((response) => {
        const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
            emp: response,
            username: this.user?.userName
          }
        }
    
        this.bsModalRef = this.bsModalService.show(EmploymentModalComponent, config);
        const observableInner = this.bsModalRef.content.updateEmp;

        return observableInner
      })
    ).subscribe((response) => {
      console.log('inner response:', response );
    })

  }
  
  displaySelectionModal(event: any){

    var id = event;
    if(id === 0) {
      this.toastr.warning('invalid selection decision id');
      return;
    }

    const observableOuter = this.service.getSelectionBySelDecisionId(id);
    
    observableOuter.pipe(
      filter((response: any) => response !==null),
      switchMap((response) => {
        const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
            sel: response
          }
        }
    
        this.bsModalRef = this.bsModalService.show(SelectionModalComponent, config);
        const observableInner = this.bsModalRef.content.updateObj;

        return observableInner
      })
    ).subscribe((response) => {
      console.log('inner response:', response );
    })

  }
 
  onPageChanged(event: any){
    const params = this.service.getParams();
    if (this.sParams.pageNumber !== event) {
      this.sParams.pageNumber = event;
      this.getSelectionsPaged(this.sParams);
    }
  }

  onSortSelected(event: any) {
    var sort = event.value;

    const prms = this.service.getParams();
    prms.sort = sort;
    prms.pageNumber=1;
    prms.pageSize=10;
    this.service.setParams(prms);
    this.getSelectionsPaged(prms);
  }

  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.getSelectionsPaged(params);
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new SelDecisionParams();
    this.service.setParams(this.sParams);
    this.getSelectionsPaged(this.sParams);
  }
}
