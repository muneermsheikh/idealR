import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICustomerReview } from 'src/app/_models/admin/customerReview';
import { User } from 'src/app/_models/user';
import { CustomerReviewService } from 'src/app/_services/admin/customer-review.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-customer-review-modal',
  templateUrl: './customer-review-modal.component.html',
  styleUrls: ['./customer-review-modal.component.css']
})
export class CustomerReviewModalComponent implements OnInit {

  review: ICustomerReview | undefined;
  reviewNewOrExisting: string = '';

  @Output() updateReview = new EventEmitter<ICustomerReview>();
  
  reviewStatus=[{status: "Nice Infratructure"}, {status:"Good HR Team"},
    {status:"Professional attitude"}, {status: "Delivery as committed"},
    {status: "Nice presentation"}, {status: "Insufficient infratructure"},
    {status: "Unprofessional attitude"}, {status: "delayed delivery"},
    {status: "Irrelevant profiles"}, {status: "Bad communication"}
  ]

  bsValue = new Date();
  bsValueDate = new Date();
  user?: User;

  form: FormGroup = new FormGroup({});
  
  constructor(public bsModalRef: BsModalRef, private toastr:ToastrService, 
    private confirmService: ConfirmService, private fb: FormBuilder 
    , private service: CustomerReviewService) {
    
    }

  
  ngOnInit(): void {
    if(this.review) this.InitializeForm(this.review)
      this.reviewNewOrExisting = this.review!.id > 0 ? "Edit Review" : "New Review";
  }

  InitializeForm(rvw: ICustomerReview) {
      this.form = this.fb.group({
        id: rvw.id, customerId:rvw.customerId, customerName: rvw.customerName,
        city: rvw.city, currentStatus: rvw.currentStatus, remarks: rvw.remarks,

        customerReviewItems: this.fb.array(
          rvw.customerReviewItems.map(x => (
            this.fb.group({
              id: x.id, customerReviewId:x.customerReviewId, 
              transactionDate: x.transactionDate, username: x.username,
              customerReviewStatus: x.customerReviewStatus,
              remarks: x.remarks, 
              approvedOn: x.approvedOn,
              approvedByUsername: x.approvedByUsername
            })
          )
        )
      )
    })
  }

  get customerReviewItems(): FormArray{
    return this.form.get("customerReviewItems") as FormArray;
  }

  newCustomerReviewItem(): FormGroup{
    return this.fb.group({
        id:0, customerReviewId:this.review?.id, 
        transactionDate: new Date(),  username: this.user?.userName,
        customerReviewStatus: "", remarks: "", approvedOn: new Date(),
        approvedByUsername: ""
    })
  }

  addCustomerReviewItem() {
    this.customerReviewItems.push(this.newCustomerReviewItem);
  }

  removeReviewItem(index: number) {
    var msg ="This will delete the selected transaction from this page.  However, you need to save the form to remove the object from database!";
    
    this.confirmService.confirm("Confirm Delete", msg).subscribe({next: confirmed => {
        if(confirmed) this.customerReviewItems.removeAt(index);
      }})
  }

  updateClicked() {
    var formdata = this.form.value;
    console.log('form data in review modal', formdata);
    this.updateReview.emit(formdata);
    this.bsModalRef.hide();
  }
}
