import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { IOrderForwardToAgent } from 'src/app/_models/orders/orderForwardToAgent';
import { Pagination } from 'src/app/_models/pagination';
import { OrderFwdParams } from 'src/app/_models/params/orders/orderFwdParams';
import { OrderForwardService } from 'src/app/_services/admin/order-forward.service';

@Component({
  selector: 'app-order-fwds',
  templateUrl: './order-fwds.component.html',
  styleUrls: ['./order-fwds.component.css']
})
export class OrderFwdsComponent {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  form: FormGroup = new FormGroup({});
  
  data: IOrderForwardToAgent[]=[];

  fwdsToAgent: IOrderForwardToAgent[]=[];

  fwdToAgent: IOrderForwardToAgent | undefined;
  fwdParams = new OrderFwdParams();
  pagination: Pagination | undefined;
  totalCount = 0;

  constructor(private fb: FormBuilder, private activatedRouter: ActivatedRoute,
      private service: OrderForwardService) {}

  ngOnInit(): void {
  
    this.service.setFwdParams(this.fwdParams);
    this.loadFwdRecords(this.fwdParams);

  }

  loadFwdRecords(fParams: OrderFwdParams) {
      this.service.setFwdParams(fParams);

      this.service.getForwardsBriefPaginated(fParams).subscribe({
        next: (response: any) => {
          if(response.result && response.pagination) {
              this.fwdsToAgent = response.result;
              this.pagination = response.pagination;
              this.totalCount = response.count;
              console.log('orderfwds loadfwdrecords:', this.fwdsToAgent);
          }
        },
        error: (error: any) => console.log(error)
        
      });
  }

  
  onSearch() {
    const params = this.service.getFwdParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setFwdParams(params);
    this.loadFwdRecords(params);
  }

  
  onPageChanged(event: any){
    const params = this.service.getFwdParams();
    if (params.pageNumber !== event) {
        params.pageNumber = event;
        this.service.setFwdParams(params);
        this.loadFwdRecords(params);
    }
  }

  onReset() {
    const params = new OrderFwdParams();
    this.service.setFwdParams(params);
    this.loadFwdRecords(params);
  }

  OrderFwdEdit(event: any){

  }

  orderFwdDelete(event: any) {

  }

  orderCategoryDelete(event: any) {

  }

  orderCatOfficialDelete(event: any) {

  }
}
