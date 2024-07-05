import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IContractReview } from 'src/app/_models/admin/contractReview';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { IOrderItem } from 'src/app/_models/admin/orderItem';
import { User } from 'src/app/_models/user';
import { ContractReviewService } from 'src/app/_services/admin/contract-review.service';
import { EmployeeService } from 'src/app/_services/admin/employee.service';

@Component({
  selector: 'app-contract-review',
  templateUrl: './contract-review.component.html',
  styleUrls: ['./contract-review.component.css']
})
export class ContractReviewComponent implements OnInit {

  review: IContractReview | undefined;

  form: FormGroup = new FormGroup({});

  user?: User;

  empIdAndNames: IEmployeeIdAndKnownAs[]=[];

  skills: string[]=[];
  
  reviewStatuses= [
    {status: 'Not Reviewed'},{status: 'Accepted'}, {status: 'Accepted with regrets'}, 
    {status: 'Regretted'}
  ]

  constructor( private fb: FormBuilder, private router: Router, 
    private activatedRoute: ActivatedRoute, private empService: EmployeeService,
    private service: ContractReviewService, private toastr: ToastrService ) { 
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

      if (nav?.extras && nav.extras.state) {
          if( nav.extras.state['user']) 
                this.user = nav.extras.state['user'] as User;
      }
      empService.getEmployeeIdAndKnownAs().subscribe({next: response => this.empIdAndNames=response});
  }

  ngOnInit(): void {
    this.activatedRoute.data.subscribe({
      next: data => {
        this.review = data['orderreview'];
        if(this.review) this.InitializeForm(this.review);
      }
    })
  }

  InitializeForm(rvw: IContractReview) {

    this.form = this.fb.group({

      id: rvw.id ?? 0, orderId: rvw.orderId, orderNo: rvw.orderNo,
      orderDate: rvw.orderDate,

      contractReviewItems: this.fb.array(
        rvw.contractReviewItems.map(x => (
          this.fb.group({
            id: rvw.id ?? 0, orderId: rvw.orderId ?? 0, orderItemId: x.orderItemId ?? 0,
            professionName: x.professionName ?? '', quantity: x.quantity ?? 0,
            contractReviewId: [x.contractReviewId ?? 0, Validators.required],
            sourceFrom: x.sourceFrom ?? 'India', requireAssess: [x.requireAssess ?? false, Validators.required],
            charges: x.charges ?? 0, hrExecUsername: [x.hrExecUsername ?? '', Validators.required],
            reviewItemStatus: [x.reviewItemStatus ?? 'Not Reviewed', Validators.required],
            orderDate: rvw.orderDate ?? new Date(),

            contractReviewItemQs: this.fb.array(
              x.contractReviewItemQs.map(q => {
                this.fb.group({
                  id: q.id ?? 0, 
                  contractReviewItemId: [q.contractReviewItemId ?? 0, Validators.required],
                  srNo: q.srNo ?? 0, reviewParameter: [q.reviewParameter ?? '', Validators.required],
                  response: q.response ?? false, responseText: q.responseText ?? '',
                  isResponseBoolean: q.isResponseBoolean ?? false, 
                  isMandatoryTrue: q.isMandatoryTrue ?? false, remarks: q.remarks ?? ''
                })
              })
            )
          })
        )
      )
      )
    })
  }

  get contractReviewItems(): FormArray {
    return this.form.get("contractReviewItems") as FormArray
  }

  contractReviewItemQs(index: number): FormArray {
    return this.contractReviewItems.at(index).get("contractReviewItemQs") as FormArray
  }

  onSubmit() {

  }

  close() {

  }

  DeleteReview() {
    
  }


}
