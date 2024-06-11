import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IOrderAssessmentItem } from 'src/app/_models/admin/orderAssessmentItem';
import { IOrderAssessmentItemQ } from 'src/app/_models/admin/orderAssessmentItemQ';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { OrderAssessmentService } from 'src/app/_services/hr/orderAssessment.service';

@Component({
  selector: 'app-order-assessment-item',
  templateUrl: './order-assessment-item.component.html',
  styleUrls: ['./order-assessment-item.component.css']
})
export class OrderAssessmentItemComponent implements OnInit {

  form: FormGroup = new FormGroup({});
  orderAssessmentItem: IOrderAssessmentItem | undefined;
  @Output() cancelRegister = new EventEmitter();
  returnUrl = '';
  user?: User;
  professionId = 0;

  totalMarks = 0;
  
  constructor(private fb: FormBuilder, private router: Router,
    private activatedRoute: ActivatedRoute, private confirmService: ConfirmService,
    private service: OrderAssessmentService, private toastr: ToastrService){
      //navigationExtras
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

      if (nav?.extras && nav.extras.state) {
          if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

          if( nav.extras.state['user']) {
            this.user = nav.extras.state['user'] as User;
            }
      }
    }
  
  ngOnInit(): void {

      this.activatedRoute.data.subscribe(data => {
          this.orderAssessmentItem = data['assessmentItem']
      })

      if(this.orderAssessmentItem) {
        this.CreateAndInitializeFormArray(this.orderAssessmentItem);
        this.calcualteTotals();
        this.professionId = this.orderAssessmentItem?.professionId;
      }

  }

  CreateAndInitializeFormArray(item: IOrderAssessmentItem) {

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
          orderAssessmentItemId: this.form.get('id'),
          orderItemId: [this.form.get('orderItemId'), Validators.required],
          orderId: [this.form.get('orderId'), Validators.required],
          questionNo: [0, Validators.required],
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
            id: q.id,
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
    this.router.navigateByUrl(this.returnUrl);
  }

  update() {
    this.service.updateOrderAssessmentItem(this.form.value).subscribe({
      next: (succeeded: boolean) => {
        if(succeeded) {
            this.toastr.success('Order Assessment Item successfully updated', 'success');
        } else {
          this.toastr.warning('failed to update the order assessment item', 'Failed to update');
        }
        }
      })
  }

  calcualteTotals() {
    this.totalMarks = this.form.get('orderAssessmentItemQs')?.value.map((x: IOrderAssessmentItemQ) => x.maxPoints).reduce((a: any, b: any) => a + b);
  }

  GetConfirmation(existingQs: number): boolean {
    
    var confirmed=false;

    this.confirmService.confirm('confirm OVER-WRITE existing Assessment Questions', 
      'Currently, there are ' + existingQs + ' assessment Questions relating to this Category.' +
      'If you continue, these questions will be replaced with new set of standard assessment quetions')
      .subscribe(response => confirmed = response);
    
    return confirmed;
    
  }

  populateStddAssessmentQuestions(){

    var existingQs = this.orderAssessmentItemQs.length;
    if(!this.GetConfirmation(existingQs)) {
      this.toastr.info('User aborted', 'abort');
      return;
    }
    
    this.service.getAssessmentQStandard().subscribe({
      next: (qs: IOrderAssessmentItemQ[] ) => {
        if(qs.length > 0) {
          this.orderAssessmentItemQs.clear();
          if(!qs.length) {
            this.toastr.warning('No standard Questions retrieved', 'Failure');
            return;
          }
          this.newOrderAssessmentItemQFromStddQ(qs);
        } else {
          this.toastr.warning('Failed to retrieve the standard assessment Questions')
        }
      },
      error: (error: any) => this.toastr.error(error, "Error in retrieving Standard Questions")
    })
      
  }

  populateCustomAssessmentQuestions(professionid: number) {
    
    var existingQs = this.orderAssessmentItemQs.length;
    if(!this.GetConfirmation(existingQs)) {
      this.toastr.info('User aborted', 'abort');
      return;
    }

    this.service.getAssessmentQBankOfCategoryId(professionid).subscribe({
      next: (qs: IOrderAssessmentItemQ[] ) => {
        if(qs.length > 0) {
          this.orderAssessmentItemQs.clear();
          qs.forEach(x => {
            this.orderAssessmentItem?.orderAssessmentItemQs.push(x);
          })
        } else {
          this.toastr.warning('Failed to retrieve the standard assessment Questions');
        }
      },
      error: (error: any) => this.toastr.error(error, "Error in retrieving Standard Questions")
    })

  }

  updateApprovedBy() {
    if(this.user) {
      this.form.value.approvedBy=this.user.userName;
      this.form.get('approvedBy')?.setValue(this.user.userName);
    }
  }
}
