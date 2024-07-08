import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IContractReview } from 'src/app/_models/admin/contractReview';
import { User } from 'src/app/_models/user';
import { ContractReviewService } from 'src/app/_services/admin/contract-review.service';

@Component({
  selector: 'app-contract-rvw',
  templateUrl: './contract-rvw.component.html',
  styleUrls: ['./contract-rvw.component.css']
})
export class ContractRvwComponent implements OnInit {
  review: IContractReview | undefined;
  
  reviewNewOrExisting: string = '';

  objectEdited: boolean = false;
  
  reviewStatus=[{status: "Nice Infratructure"}, {status:"Good HR Team"},
    {status:"Professional attitude"}, {status: "Delivery as committed"},
    {status: "Nice presentation"}, {status: "Insufficient infratructure"},
    {status: "Unprofessional attitude"}, {status: "delayed delivery"},
    {status: "Irrelevant profiles"}, {status: "Bad communication"}
  ]

  bsValue = new Date();
  bsValueDate = new Date();
  user?: User;

  reviewStatuses= [
    {status: 'Not Reviewed'},{status: 'Accepted'}, {status: 'Accepted with regrets'}, 
    {status: 'Regretted'}
  ]
  
  constructor(private router: Router, private activatedRouter: ActivatedRoute,
      private service: ContractReviewService, private toastr: ToastrService) { }

  ngOnInit(): void {

    this.activatedRouter.data.subscribe({
      next: data => {
        this.review = data['orderReview'];
      }
    })
  }

  updateClicked() {
    if(this.review !== undefined) {
        this.service.updateContractReview(this.review).subscribe({
          next: response => {
            if(response !== null) {
              this.toastr.success('the Contract Review was updated', 'success')
            } else {
              this.toastr.warning('failed to update the contract review', 'failure')
            }
        },
        error: err => this.toastr.error(err, 'Error encountered')
      })
    }
  }

  accumulateEvents(event: any) {
    if(event===null) return;

    this.objectEdited=true;

    if(this.review !== undefined) {
      var index = this.review?.contractReviewItems.findIndex(x => x.orderItemId == event.orderItemId);
      if(index === -1) {
        this.review.contractReviewItems.push(event);
      } else {
        this.review.contractReviewItems[index]=event;
      }
  
    }
    
  }

  close() {
    this.router.navigateByUrl('/administration/orders')
  }
}
