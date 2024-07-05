import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { ICustomerReview } from 'src/app/_models/admin/customerReview';
import { User } from 'src/app/_models/user';
import { CustomerReviewService } from 'src/app/_services/admin/customer-review.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-cust-rvw-edit-modal',
  templateUrl: './cust-rvw-edit-modal.component.html',
  styleUrls: ['./cust-rvw-edit-modal.component.css']
})
export class CustRvwEditModalComponent implements OnInit {

  review: ICustomerReview | undefined;
  @Output() editEvent = new EventEmitter<boolean>();
  
  bsValueDate = new Date();
  bsValue = new Date();
  user?: User;
  returnUrl = '';
  
  form: FormGroup = new FormGroup({});

  
  reviewStatus=[{status: "Nice Infratructure"}, {status:"Good HR Team"},
    {status:"Professional attitude"}, {status: "Delivery as committed"},
    {status: "Nice presentation"}, {status: "Insufficient infratructure"},
    {status: "Unprofessional attitude"}, {status: "delayed delivery"},
    {status: "Irrelevant profiles"}, {status: "Bad communication"}
  ]

  constructor(private toastr: ToastrService, private service: CustomerReviewService, 
    private activatedRoute: ActivatedRoute, private fb: FormBuilder, private bsModalRef: BsModalRef,
    private confirm: ConfirmService, private router: Router){
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

      if (nav?.extras && nav.extras.state) {
          if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

          if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
      }
    }

  ngOnInit(): void {
    
    this.activatedRoute.data.subscribe(data => {
      this.review = data['review'];
      if(this.review !== undefined) this.InitializeForm(this.review);
      }
    )
   
    
  }

  InitializeForm(rvw: ICustomerReview) {

      this.form = this.fb.group({
        id: rvw.id,  
        customerId: rvw.customerId,
        customerName: rvw.customerName,
        city: rvw.city,
        currentStatus: rvw.currentStatus,

        customerReviewItems: this.fb.array(
          rvw.customerReviewItems.map(x => (
            this.fb.group({
              id: x.id, 
              transactionDate: x.transactionDate,
              customerReviewId: x.customerReviewId,
              customerReviewStatus: x.customerReviewStatus,
              remarks: x.remarks,
              approvedOn: x.approvedOn,
              approvedByUsername: x.approvedByUsername
            })
          ))
        )
        
      })
    }

  //customerofficials
    get customerReviewItems(): FormArray {
        return this.form.get("customerReviewItems") as FormArray
    }

      newReviewItem(): FormGroup {
        return this.fb.group({
            id: 0, 
            transactionDate: new Date(),
            customerReviewId: this.review?.id,
            customerReviewStatus: "",
            remarks: "",
            approvedOn: new Date(),
            approvedByUsername: this.user?.userName
        })
      }

      addReviewItem() {
        this.customerReviewItems.push(this.newReviewItem());
      }

      removeItem(index: number) {
        
          this.confirm.confirm("Confirm Delete", "This will delete review item.  To delete from the database as well, remember to SAVE this form before you close it")
            .subscribe({next: confirmed => {
              if(confirmed) this.customerReviewItems.removeAt(index);
          }})
      }
    //event emitters

    updateReview() {
    var formdata = this.form.value;
    if(formdata.id ===0) {
      this.service.postNewCustomerReview(formdata).subscribe({
        next: inserted => {
          if(inserted) {
            this.toastr.success('The customer review is inserted in database', 'success');
            this.editEvent.emit(true);
          }
        }
      })
    } else {
      this.service.updateCustomerReview(formdata).subscribe({
        next: updated => {
          if(updated) {
            this.toastr.success('The customer review is updated in the database', 'success');
            this.editEvent.emit(true);
          }
        }
      })
    }
  }

    approveItem(index: number) {
      var id=this.customerReviewItems.at(index).get('id')?.value;

      var confirmMsg = 'This will set the selected transaction as approved, provided the user has required credentials';

      const observableInner = this.service.approveCustomerReviewItem(+id!);
      const observableOuter = this.confirm.confirm('confirm Approve', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => {
          return observableInner
        })
    ).subscribe(response => {
      if(response) {
        this.toastr.success('Customer Review Item set as updated', 'Approval set');
      } else {
        this.toastr.error('Error in setting the customer review item as Approved', 'failed to approve')
      }
      
    });
    }
  goback() {
    this.router.navigateByUrl(this.returnUrl);
  }
}
