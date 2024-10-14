import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { IContractReviewItemDto } from 'src/app/_dtos/orders/contractReviewItemDto';
import { IContractReviewItem } from 'src/app/_models/admin/contractReviewItem';
import { IContractReviewItemStddQ } from 'src/app/_models/admin/contractReviewItemStddQ';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { IContractReviewItemQ } from 'src/app/_models/orders/contractReviewItemQ';
import { ContractReviewService } from 'src/app/_services/admin/contract-review.service';
import { EmployeeService } from 'src/app/_services/admin/employee.service';

@Component({
  selector: 'app-contract-rvw-item',
  templateUrl: './contract-rvw-item.component.html',
  styleUrls: ['./contract-rvw-item.component.css']
})
export class ContractRvwItemComponent implements OnInit {

  @Input() item: IContractReviewItemDto | undefined;
  @Output() itemSaveEvent = new EventEmitter<IContractReviewItemDto>();

  stddQs: IContractReviewItemStddQ[]=[]
  toggle: boolean=false;
  itemQsExist:boolean=false;

  empIdAndNames: IEmployeeIdAndKnownAs[]=[];
  
  reviewStatuses= [
    {status: 'Not Reviewed'},{status: 'Accepted'}, {status: 'Accepted with regrets'}, 
    {status: 'Regretted'}
  ]

  constructor(private empService: EmployeeService, private service: ContractReviewService,
      private toastr: ToastrService) {
    empService.getEmployeeIdAndKnownAs().subscribe({next: response => this.empIdAndNames=response});
  }

  ngOnInit(): void {
    this.itemQsExist = this.item!.contractReviewItemQs.length > 0
  }
  responseChanged(q: IContractReviewItemQ) {
    q.response=true;
  }

  toggleImage() {
    this.toggle = !this.toggle;
  }

  verifydata() {
      var err='';
      if(this.item?.hrExecUsername === '') err = 'HR Executive not assigned';
      if(this.item?.contractReviewId === 0) err += '- Contract Review Id undefined';
      if(this.item?.requireAssess ) err += '- Require Assess not defined';
      if(this.item?.reviewItemStatus === '') err += '- Status not selected';
      
      this.item?.contractReviewItemQs.forEach(x =>{
        if(x.isMandatoryTrue && !x.response) err += "- " + x.reviewParameter + ' is set as mandatory, but is not marked as accepted';
      })

      return err;
  }

  update(){

    var err = this.verifydata();
    if(err !== '') {
      this.toastr.error(err, 'cannot continue');
      return;
    }

    this.itemSaveEvent.emit(this.item);
 }

  addItemQs(orderitemid: number) {

    this.service.InsertContractReviewItemFromOrderItemId(orderitemid).subscribe({
      next: (response: IContractReviewItemDto) => {
        if(response !== null) {
          this.item = response;
          this.itemQsExist=response.contractReviewItemQs.length > 0;
        }
      }
    })
  }


}
