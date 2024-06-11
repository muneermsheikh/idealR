import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IOrderAssessment } from 'src/app/_models/admin/orderAssessment';
import { User } from 'src/app/_models/user';
import { OrderAssessmentService } from 'src/app/_services/hr/orderAssessment.service';

@Component({
  selector: 'app-order-assessment',
  templateUrl: './order-assessment.component.html',
  styleUrls: ['./order-assessment.component.css']
})
export class OrderAssessmentComponent implements OnInit {

    form: FormGroup = new FormGroup({});
    user?: User;
    returnUrl='';

    orderAssessment: IOrderAssessment | undefined;
    
    constructor(private fb: FormBuilder, private router: Router, private activatedRoute: ActivatedRoute,
      private service: OrderAssessmentService, private toastr: ToastrService){

        let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;
            if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
            //if(nav.extras.state['orderBrief']) this.orderBrief=nav.extras.state['orderBrief'] as IOrderBriefDto;
        }
          
    }


    ngOnInit(): void {

      this.activatedRoute.data.subscribe(data => {
        this.orderAssessment = data['orderAssessment']
      })
                
      if(this.orderAssessment !== undefined) this.CreateAndInitializeFormArray(this.orderAssessment);

    }

      CreateAndInitializeFormArray(assess: IOrderAssessment) {

          this.form = this.fb.group({
              orderId: [assess.orderId, Validators.required],
              orderNo: [assess.orderNo, Validators.required],
              customerName: [assess.customerName, Validators.required],

              orderAssessmentItems: this.fb.array( 
                assess.orderAssessmentItems.map(x => (
                    this.fb.group({
                        id: [x.id],
                        orderAssessmentId: [x.orderAssessmentId],
                        orderItemId: [x.orderItemId, Validators.required],
                        orderId: [assess.orderId, Validators.required],
                        professionId: [x.professionId, Validators.required],
                        professionName: [x.professionName, Validators.required],

                        orderAssessmentItemQs: this.fb.array(
                            x.orderAssessmentItemQs.map(q => (
                              this.fb.group({
                                  id: [q.id ?? 0],
                                  orderAssessmentItemId: x.id ?? 0,
                                  orderItemId: [x.orderItemId, Validators.required],
                                  orderId: x.orderId,
                                  questionNo: [q.questionNo, Validators.required],
                                  subject: [q.subject, Validators.required],
                                  question: [q.question, Validators.required],
                                  maxPoints: [q.maxPoints, Validators.required],
                                  isMandatory: [q.isMandatory, Validators.required]
                              })
                            ))
                        )
                    })
                ))
              )
          })
        }

      get orderAssessmentItems(): FormArray {
        return this.form?.get("orderAssessmentItems") as FormArray;
      }
      
      newOrderAssessmentItem(): FormGroup {
        return this.fb.group({
          id: 0,
          orderAssessmentItemId: 0,
          orderItemId: 0
        })
      }   //not required, as this is static

      addOrderAssessmentItem() {
        this.orderAssessmentItems.push(this.newOrderAssessmentItem);
      }

      removeOrderAssessmentItem(itemIndex: number) {
        this.orderAssessmentItems.removeAt(itemIndex);
      }
      
      orderAssessmentItemQs(itemIndex: number) : FormArray {
        return this.orderAssessmentItems.at(itemIndex).get("orderAssessmentItemQs") as FormArray;
      }
      
      newOrderAssessmentItemQ(itemIndex: number): FormGroup {
        return this.fb.group({
              id: 0,
              orderAssessmentItemId: this.orderAssessmentItems.at(itemIndex).get('id'),
              orderItemId: [this.orderAssessmentItems.at(itemIndex).get('orderItemId'), Validators.required],
              orderId: [this.orderAssessmentItems.at(itemIndex).get('orderId'), Validators.required],
              questionNo: [0, Validators.required],
              subject: ['', Validators.required],
              question: ['', Validators.required],
              maxPoints: [0, Validators.required],
              isMandatory: [false, Validators.required]
        })
      }
      
      addNewOrderAssessmentItemQ(itemIndex: number) {
        this.orderAssessmentItemQs(itemIndex).push(this.newOrderAssessmentItemQ(itemIndex))
      }

      removeOrderAssessmentItemQ(itemIndex: number, qIndex: number) {
          this.orderAssessmentItemQs(itemIndex).removeAt(qIndex);
      }

      updateAssessment() {
         
        this.service.updateOrderAssessment(this.form.value).subscribe({
          next: (succeeded:boolean) => {
              if(succeeded) {
                this.toastr.success('Updated the Order Assessment data', 'success');
              } else {
                this.toastr.warning('Failed to update the order assessment data', 'Failure')
              }
          },
          error: (error: any) => this.toastr.error(error, 'Error in updating the Order Assessment')
        })
      }

  }

