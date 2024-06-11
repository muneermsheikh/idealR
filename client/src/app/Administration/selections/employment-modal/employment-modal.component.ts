import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IEmployment } from 'src/app/_models/admin/employment';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-employment-modal',
  templateUrl: './employment-modal.component.html',
  styleUrls: ['./employment-modal.component.css']
})
export class EmploymentModalComponent {

  @Input() emp: IEmployment | undefined;
  @Input() updateEmp = new EventEmitter();
  @Input() offerAcceptedEvent = new EventEmitter<number>();
  @Input() OfferRejectedEvent = new EventEmitter<number>();

  bsValue = new Date();
  bsRangeValue= new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();

  form: FormGroup = new FormGroup({});
  
  constructor(public bsModalRef: BsModalRef, private toastr:ToastrService, 
    private confirmService: ConfirmService, private fb: FormBuilder) { }

  ngOnInit(): void {
    if(this.emp) this.InitializeForm(this.emp);
  }

  InitializeForm(emp: IEmployment) {

    this.form = this.fb.group({
        id: [emp.id], 
        cVRefId: [emp.cVRefId],
        orderNo: [emp.orderNo],
        customerName: [emp.customerName],
        selectionDecisionId: [emp.selectionDecisionId],
        applicationNo: [emp.applicationNo],
        categoryName: [emp.categoryName],

        selectedOn: [emp.selectedOn],
        charges: [emp.charges],
        salaryCurrency: [emp.salaryCurrency, Validators.required],
        salary: [emp.salary, Validators.required],
        contractPeriodInMonths: [emp.contractPeriodInMonths, Validators.required],
        weeklyHours: [emp.weeklyHours, Validators.required],
        housingProvidedFree: [emp.housingProvidedFree],
        housingNotProvided: [emp.housingNotProvided],
        housingAllowance: [emp.housingAllowance],
        foodProvidedFree: [emp.foodProvidedFree],
        foodNotProvided: [emp.foodNotProvided],
        foodAllowance: [emp.foodAllowance],
        transportProvidedFree: [emp.transportNotProvided],
        transportNotProvided: [emp.transportNotProvided],
        transportAllowance: [emp.transportAllowance],
        otherAllowance: [emp.otherAllowance],
        leavePerYearInDays: [emp.leavePerYearInDays],
        leaveAirfareEntitlementAfterMonths: [emp.leaveAirfareEntitlementAfterMonths],
        offerAcceptedOn: [emp.offerAcceptedOn]
    })
  }

  updateEmployment() {

    //verify data inputs
      this.updateEmp.emit(this.emp);
      this.bsModalRef.hide();
  }

  close() {
      this.bsModalRef.hide();
  }

  offerAccepted() {
    if(this.emp) this.offerAcceptedEvent.emit(this.emp.id);
  }

  offerRejected() {
    if(this.emp) this.OfferRejectedEvent.emit(this.emp.id);
  }
}
