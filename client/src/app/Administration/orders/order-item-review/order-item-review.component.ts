import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IContractReviewItemDto } from 'src/app/_dtos/orders/contractReviewItemDto';
import { IContractReviewItem } from 'src/app/_models/admin/contractReviewItem';
import { IContractReviewItemStddQ } from 'src/app/_models/admin/contractReviewItemStddQ';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { IContractReviewItemQ } from 'src/app/_models/orders/contractReviewItemQ';
import { User } from 'src/app/_models/user';
import { ContractReviewService } from 'src/app/_services/admin/contract-review.service';
import { EmployeeService } from 'src/app/_services/admin/employee.service';
import { ConfirmService } from 'src/app/_services/confirm.service';


@Component({
  selector: 'app-order-item-review',
  templateUrl: './order-item-review.component.html',
  styleUrls: ['./order-item-review.component.css']
})
export class OrderItemReviewComponent {

  //called by order-edit
  @Output() updateModalReview = new EventEmitter<IContractReviewItem>();
  
  reviewItem?: IContractReviewItemDto;
  
  form: FormGroup = new FormGroup({});  
  isAddMode: boolean = false;
  reviewStatus = [{name: 'Not Reviewed'}, {name: 'Accepted'}, {name: 'Rejected'}];
  user?: User;
  empIdAndNames: IEmployeeIdAndKnownAs[]=[];

  skills: string[]=[];
  stddQs: IContractReviewItemStddQ[]=[];
  itemQsExist:boolean=false;

  constructor(public bsModalRef: BsModalRef, private empService: EmployeeService,
    private fb: FormBuilder, private router: Router, private activatedRoute: ActivatedRoute,
    private service: ContractReviewService, private confirm: ConfirmService ) { 
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

      if (nav?.extras && nav.extras.state) {
          if( nav.extras.state['user']) 
                this.user = nav.extras.state['user'] as User;
      }

      empService.getEmployeeIdAndKnownAs().subscribe({next: response => this.empIdAndNames=response});
  }

  ngOnInit(): void {
 
    if(this.reviewItem) this.createAndInitializeForm(this.reviewItem);

  }

  createAndInitializeForm(rvw: IContractReviewItemDto) {
      this.form = this.fb.group({
        id: rvw.id ?? 0, orderId: rvw.orderId ?? 0, orderItemId: rvw.orderItemId ?? 0,
        professionName: rvw.professionName ?? '', quantity: rvw.quantity ?? 0,
        ecnr: rvw.ecnr ?? false, contractReviewId: [rvw.contractReviewId ?? 0, Validators.required],
        sourceFrom: rvw.sourceFrom ?? 'India', requireAssess: [rvw.requireAssess ?? false, Validators.required],
        charges: rvw.charges ?? 0, hrExecUsername: [rvw.hrExecUsername ?? '', Validators.required],
        reviewItemStatus: [rvw.reviewItemStatus ?? 'Not Reviewed', Validators.required],
        orderDate: rvw.orderDate ?? new Date(), customerName: [rvw.customerName ?? '', Validators.required],

        contractReviewItemQs: this.fb.array(
          rvw.contractReviewItemQs.map(x => (
            this.fb.group({
              id: x.id ?? 0, 
              orderItemId: x.orderItemId ?? 0, 
              contractReviewItemId: [x.contractReviewItemId ?? 0, Validators.required],
              srNo: x.srNo ?? 0, reviewParameter: [x.reviewParameter ?? '', Validators.required],
              response: x.response ?? false, responseText: x.responseText ?? '',
              isResponseBoolean: x.isResponseBoolean ?? false, 
              isMandatoryTrue: x.isMandatoryTrue ?? false, remarks: x.remarks ?? ''

            })
          ))
        )        
      })

      var username = this.form.get('hrExecUsername')!.value;
      console.log('username:', username, 'empIdAndNames', this.empIdAndNames);
      if (username) {
        var idandname = this.empIdAndNames.filter(x => x.username===username)[0];
        console.log('idandname', idandname);
        this.skills = idandname?.hrSkills?.map((x: any) => x.professionName);
      }
  }

  get contractReviewItemQs() : FormArray {
    return this.form.get("contractReviewItemQs") as FormArray
  }

  addQ() {
    this.contractReviewItemQs.push(this.newQ());
  }

  removeQ(index: number) {
    this.contractReviewItemQs.removeAt(index);
    this.contractReviewItemQs.markAsDirty();
    this.contractReviewItemQs.markAsTouched();
  }


  confirmUpdate() {
    this.service.updateContractReviewItem(this.form.value)
      .subscribe(response => {
        this.updateModalReview.emit(response);
      })
        
    this.bsModalRef.hide();
  }

  decline() {
    this.bsModalRef.hide();
  }

  employeeChanged(event: any) {
    console.log('emplotyeecanged event:', event);
    this.skills = event.hrSkills.map((x: any) => x.professionName);
  }

  /*InsertStddReviewQs() {

    this.service.InsertContractReviewItemFromOrderItemId(this.reviewItem!.orderItemId).subscribe({
      next: (response: IContractReviewItemDto) => {
        if(response !== null) {
          this.reviewItem = response;
          this.itemQsExist=response.contractReviewItemQs.length > 0;
        }
      }
    })

  }*/

  newQ(): FormGroup {
    var maxSrNo = this.contractReviewItemQs.length===0 ? 1 
      : Math.max(...this.contractReviewItemQs.value.map((x:any) => x.srNo))+1;
    
      return this.fb.group({
          id: 0, orderItemId: this.reviewItem!.orderItemId ?? 0, 
          contractReviewItemId: [this.reviewItem!.id ?? 0, Validators.required],
          srNo: maxSrNo, reviewParameter: ['', Validators.required],
          response: false, responseText: '',
          isResponseBoolean: false, 
          isMandatoryTrue: false, remarks: ''
      })
  }

  
}
