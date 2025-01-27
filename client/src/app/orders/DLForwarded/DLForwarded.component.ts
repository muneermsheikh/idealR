import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IOrderForwardCategory } from 'src/app/_models/orders/orderForwardCategory';
import { IOrderForwardToAgent } from 'src/app/_models/orders/orderForwardToAgent';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { OrderFwdParams } from 'src/app/_models/params/orders/orderFwdParams';
import { OrderForwardService } from 'src/app/_services/admin/order-forward.service';

@Component({
  selector: 'app-order-fwds',
  templateUrl: './DLForwarded.component.html',
  styleUrls: ['./DLForwarded.component.css']
})
export class DLForwardedComponent implements OnInit {

  //@ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  form: FormGroup = new FormGroup({});
  
  fwds: IOrderForwardCategory[]=[];
  
  fwdParams = new OrderFwdParams();
  pagination: Pagination | undefined;
  totalCount = 0;

  hideCategory:boolean=false;

  customerName: string = '';
  orderNo: number=0;
  orderDate: Date | undefined;
  firstLoop=true;

  constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute,
      private service: OrderForwardService, private toastr: ToastrService) { }

  ngOnInit(): void {

    this.activatedRoute.data.subscribe({
      next: response => {
          this.fwds = response['DLForwarded'].result;
          this.pagination = response['pagination'];
          this.totalCount = response['totalCount'];

          console.log('pagination', this.pagination);
          var fwd=this.fwds[0];
          if(fwd) {
              this.customerName = fwd.customerName;
              this.orderNo = fwd.orderNo;
              this.orderDate = fwd.orderDate;
          }

          console.log('fwded', fwd);
      }
    })
  }

  loadFwdRecords(fParams: OrderFwdParams) {
      this.service.setFwdParams(fParams);
      this.service.getForwardsBriefPaginated(fParams).subscribe({
        next: (response: PaginatedResult<IOrderForwardCategory[]>) => {
          console.log('DLForwarded OrderFwdToAgent', response);
          if(response.result && response.pagination) {
              this.fwds = response.result;
              this.pagination = response.pagination;
              this.totalCount = response.pagination.totalItems;
          }
        },
        error: (error: any) => console.log(error)
        
      });
  }
    
  onPageChanged(event: any){
    const params = this.service.getFwdParams();
    if (params.pageNumber !== event) {
        params.pageNumber = event;
        this.service.setFwdParams(params);
        this.loadFwdRecords(params);
    }
  }

  orderFwdDelete() {
    this.service.deleteForward(this.fwds[0].id).subscribe({
      next: succeeded => {
        if(succeeded) {
          this.toastr.success('Deleted', 'The Order Forward along with its related records was deleted');
        } else {
          this.toastr.warning('Failed to delete the Order forward', 'Failure')
        }
      }
    })
  }

  orderCategoryDelete(event: any) {
    this.service.deleteOrderFwdCategory(event).subscribe({
      next: succeeded => {
        this.toastr.success('Deleted', 'The Order Forward along with its related records was deleted');
        var index = this.fwds?.findIndex(x => x.id === event.id);
        if(index !== -1) this.fwds?.splice(index!, 1);

      }
    })
  }

  orderCatOfficialDelete(event: any) {

  }
}
