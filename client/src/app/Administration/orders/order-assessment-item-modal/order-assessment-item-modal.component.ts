import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Navigation, Router } from '@angular/router';
import { BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { AssessmentQBank, IAssessmentQBank } from 'src/app/_models/admin/assessmentQBank';
import { IOrderAssessmentItem } from 'src/app/_models/admin/orderAssessmentItem';
import { IOrderAssessmentItemQ } from 'src/app/_models/admin/orderAssessmentItemQ';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { OrderAssessmentService } from 'src/app/_services/hr/orderAssessment.service';

@Component({
  selector: 'app-order-assessment-item-modal',
  templateUrl: './order-assessment-item-modal.component.html',
  styleUrls: ['./order-assessment-item-modal.component.css']
})
export class OrderAssessmentItemModalComponent implements OnInit {
  
  form: FormGroup = new FormGroup({});
  orderAssessmentItem: IOrderAssessmentItem | undefined;
  
  @Output() cancelRegister = new EventEmitter();
  @Output() updateEvent = new EventEmitter<boolean>();
  returnUrl = '';
  user?: User;
  professionId = 0;

  totalMarks = 0;
  bsValueDate = new Date();
  
  constructor(private fb: FormBuilder, private confirm: ConfirmService,
    public bsModalService: BsModalService,
    private service: OrderAssessmentService, private toastr: ToastrService){}
  
  ngOnInit(): void {
    
      if(this.orderAssessmentItem) {

        this.CreateAndInitializeFormArray(this.orderAssessmentItem);
        this.calcualteTotals();
        this.professionId = this.orderAssessmentItem?.professionId;
      }
  }

  CreateAndInitializeFormArray(item: IOrderAssessmentItem) {
    console.log('item in createandinitiaize:', item);
    this.form = this.fb.group({
        id: [item.id],
        orderAssessmentId: [item.orderAssessmentId],
        orderItemId: [item.orderItemId, Validators.required],
        orderId: [item.orderId, Validators.required],
        orderNo: [item.orderNo, Validators.required],
        customerName: [item.customerName, Validators.required],
        professionId: [item.professionId, Validators.required],
        professionName: [item.professionName, Validators.required],
        designedBy: [item.designedBy],
        dateDesigned: [item.dateDesigned ?? new Date()],
        approvedBy: [item.approvedBy],

        orderAssessmentItemQs: this.fb.array(
          item.orderAssessmentItemQs.map(q => (
            this.fb.group({
                id: [q.id ?? 0],
                orderAssessmentItemId: q.orderAssessmentItemId,
                orderItemId: [q.orderItemId, Validators.required],
                orderId: q.orderId,
                questionNo: [q.questionNo, Validators.required],
                subject: [q.subject, Validators.required],
                question: [q.question, Validators.required],
                maxPoints: [q.maxPoints, Validators.required],
                isMandatory: [q.isMandatory, Validators.required]
            })
          ))
      )
        
    })
  }

  get orderAssessmentItemQs() : FormArray {
    return this.form.get("orderAssessmentItemQs") as FormArray
  }

  newOrderAssessmentItemQ(): FormGroup {
    return this.fb.group({
          id: 0,
          orderAssessmentItemId: this.form.get('id')?.value,
          orderItemId: [this.form.get('orderItemId')?.value, Validators.required],
          orderId: [this.form.get('orderId')?.value, Validators.required],
          questionNo: [this.orderAssessmentItemQs.length===0 ? 1 
            : Math.max(...this.orderAssessmentItemQs.value.map((x: { questionNo: number; }) => x.questionNo))+1, 
            Validators.required],
          subject: ['', Validators.required],
          question: ['', Validators.required],
          maxPoints: [0, Validators.required],
          isMandatory: [false, Validators.required]
    })
  }

  newOrderAssessmentItemQFromStddQ(qs: IOrderAssessmentItemQ[]) {
        
      qs.forEach(q => {
        this.orderAssessmentItemQs.push(
          this.fb.group({
            //id: q.id,
            orderAssessmentItemId: q.orderAssessmentItemId,
            orderItemId: [q.orderItemId, Validators.required],
            orderId: [q.orderId, Validators.required],
            questionNo: [q.questionNo, Validators.required],
            subject: [q.subject, Validators.required],
            question: [q.question, Validators.required],
            maxPoints: [q.maxPoints, Validators.required],
            isMandatory: [q.isMandatory, Validators.required]
          })
        )
      })
        
  }

  addNewOrderAssessmentItemQ() {
    this.orderAssessmentItemQs.push(this.newOrderAssessmentItemQ())
  }

  removeOrderAssessmentItemQ(itemIndex: number) {
      this.orderAssessmentItemQs.removeAt(itemIndex);
  }

  cancel() {
    this.cancelRegister.emit(false);
    this.bsModalService.hide();
  }

  update() {
    console.log('form value:', this.form.value);
    this.service.updateOrderAssessmentItem(this.form.value).subscribe({
      next: (succeeded: boolean) => {
        if(succeeded) {
            this.updateEvent.emit(true);
            this.toastr.success('Order Assessment Item successfully updated', 'success');
            this.bsModalService.hide();
        } else {
          this.toastr.warning('failed to update the order assessment item', 'Failed to update');
        }
        }
      })
  }

  calcualteTotals() {
    this.totalMarks = this.form.get('orderAssessmentItemQs')?.value.map((x: IOrderAssessmentItemQ) => x.maxPoints).reduce((a: number, b: number) => +a + +b);
  }

 
  populateStddAssessmentQuestions(){
    
    const observableInner = this.service.getAssessmentQStandard();
    const observableOuter = this.confirm.confirm('confirm Copy standard assessment Questions', 
      "This will replace any existing Assessment Questions, and replace it with the standard Assessment Questions" +
      "Press OK to proceed, CANCEL to abort");

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => {
          return observableInner
        })
    ).subscribe(response => {
      if(response.length > 0) {
          this.orderAssessmentItemQs.clear();
          this.newOrderAssessmentItemQFromStddQ(response);
        } else {
          this.toastr.warning('Failed to retrieve the standard assessment Questions')
        }
      })
      
  }

  populateCustomAssessmentQuestions() {
    
    var professionid = this.form.get('professionId')?.value;

    const observableInner = this.service.getAssessmentQBankOfCategoryId(professionid);

    const observableOuter = this.confirm.confirm('confirm Copy Custom assessment Questions', 
      "This will replace any existing Assessment Questions, and replace it with the standard Assessment Questions" +
      "Press OK to proceed, CANCEL to abort");

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => {
          return observableInner
        })
    ).subscribe((qs: IAssessmentQBank) => {
      if(qs !== null) {
        this.orderAssessmentItemQs.clear();
        qs.assessmentStddQs.forEach(x => {
          var stddQ: IOrderAssessmentItemQ = {
            id:0, orderAssessmentItemId: this.orderAssessmentItem!.id,
            orderItemId: this.orderAssessmentItem!.orderItemId,
            orderId: this.orderAssessmentItem!.orderId,
            questionNo: x.questionNo, question: x.question,
            subject: x.subject, maxPoints: x.maxPoints,
            isMandatory: false
          }
          this.orderAssessmentItem?.orderAssessmentItemQs.push(stddQ);
        })
      } else {
        this.toastr.warning('No custom assessment parameters exist for the chosen category', 'Parameters Not Found');
      }
      })
  }

  updateApprovedBy() {
    if(this.user) {
      this.form.value.approvedBy=this.user.userName;
      this.form.get('approvedBy')?.setValue(this.user.userName);
    }
  }
}
