import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IOfficialAndCustomerNameDto } from 'src/app/_dtos/admin/client/oficialAndCustomerNameDto';
import { IClientIdAndNameDto } from 'src/app/_dtos/admin/clientIdAndNameDto';
import { Pagination } from 'src/app/_models/pagination';
import { SelectOfficialParams } from 'src/app/_models/params/Admin/selectOfficialParams';
import { User } from 'src/app/_models/user';
import { OrderService } from 'src/app/_services/admin/order.service';
import { SelectCustomersService } from 'src/app/_services/admin/select-customers.service';

@Component({
  selector: 'app-select-associates-modal',
  templateUrl: './select-associates-modal.component.html',
  styleUrls: ['./select-associates-modal.component.css']
})
export class SelectAssociatesModalComponent implements OnInit {

  @Output() selectedOfficialsEvent = new EventEmitter<IOfficialAndCustomerNameDto[]>();

  availableOfficials: IOfficialAndCustomerNameDto[]=[];
  selectedOfficials: IOfficialAndCustomerNameDto[]=[];
  
  cParams: SelectOfficialParams = new SelectOfficialParams();

  lastCustomerType: string='';
  pagination: Pagination | undefined;
  totalCount: number=0;

  constructor(public bsModalRef: BsModalRef, private service: SelectCustomersService){}
  
  ngOnInit(): void {
    this.service.setParams(this.cParams);
    this.getAssociates();
  }

  getAssociates() {
    this.service.getOfficialsAndCustomers().subscribe({
      next: response => {
        this.availableOfficials = response.result;
        this.pagination = response.pagination;
        this.totalCount = response.totalCount;
      }
    })
    console.log('available officials:', this.availableOfficials);
  }

  updateChecked(checkedValue: IOfficialAndCustomerNameDto) {
    const index = this.selectedOfficials.indexOf(checkedValue);
    index !== -1 ? this.selectedOfficials.splice(index, 1, checkedValue) : this.selectedOfficials.push(checkedValue);
  }

  Close() {
    console.log('select associate modal:', this.selectedOfficials);
    this.selectedOfficialsEvent.emit(this.selectedOfficials);
    this.bsModalRef.hide();
  }

  onPageChanged(event: any){

    if(!this.cParams) return;

    if(this.cParams.pageNumber !== event.page || this.cParams.custType !== this.lastCustomerType) {
      this.cParams.pageNumber = event.page;
      this.service.setParams(this.cParams);
      this.lastCustomerType = this.cParams.custType;
      this.getAssociates();
    }
  }

}
