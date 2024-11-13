import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IRemunerationDto } from 'src/app/_dtos/admin/remunerationDto';
import { IRemuneration } from 'src/app/_models/admin/remuneration';
import { OrderService } from 'src/app/_services/admin/order.service';

@Component({
  selector: 'app-remuneration-modal',
  templateUrl: './remuneration-modal.component.html',
  styleUrls: ['./remuneration-modal.component.css']
})
export class RemunerationModalComponent implements OnInit{

  @Output() updateSelectedRemuneration = new EventEmitter<IRemuneration>();
  remun?: IRemunerationDto;  // any;
  
  closeBtnName: string='';

  form: FormGroup = new FormGroup({});
  
  constructor(private service: OrderService, public bsModalRef: BsModalRef, private fb: FormBuilder ) {
   }

  ngOnInit(): void {
    console.log('remun in modal', this.remun);
  }

 
  confirm() {

    if(this.remun) {
      var remuneration: IRemuneration = {
        id: this.remun.id, orderItemId: this.remun.orderItemId,orderId: this.remun.orderId,
        orderNo: this.remun.orderNo, categoryId: this.remun.professionId, workHours: this.remun.workHours,
        salaryCurrency: this.remun.salaryCurrency, salaryMin: this.remun.salaryMin, 
        salaryMax: this.remun.salaryMax, contractPeriodInMonths: this.remun.contractPeriodInMonths,
        housingProvidedFree: this.remun.housingProvidedFree, housingAllowance: this.remun.housingAllowance,
        housingNotProvided: this.remun.housingNotProvided, foodProvidedFree: this.remun.foodProvidedFree,
        foodAllowance: this.remun.foodAllowance, foodNotProvided: this.remun.foodNotProvided,
        transportProvidedFree: this.remun.transportProvidedFree, transportAllowance: this.remun.transportAllowance,
        transportNotProvided: this.remun.transportNotProvided, otherAllowance: this.remun.otherAllowance,
        leavePerYearInDays: this.remun.leavePerYearInDays, 
        leaveAirfareEntitlementAfterMonths: this.remun.leaveAirfareEntitlementAfterMonths};
      
      this.service.updateRemuneration(remuneration).subscribe({
        next: (response: IRemuneration) => {
          if(response !== null) {
            this.updateSelectedRemuneration.emit(response);
            this.bsModalRef.hide();
          } 
        }
      })
    }
    
  }

  decline() {
    this.bsModalRef.hide();
  }
}
