import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IEmploymentDto } from 'src/app/_dtos/admin/employmentDto';
import { Employment, IEmployment } from 'src/app/_models/admin/employment';
import { User } from 'src/app/_models/user';
import { EmploymentService } from 'src/app/_services/admin/employment.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { SelectionService } from 'src/app/_services/hr/selection.service';

@Component({
  selector: 'app-employment-modal',
  templateUrl: './employment-modal.component.html',
  styleUrls: ['./employment-modal.component.css']
})
export class EmploymentModalComponent {

  @Input() emp: IEmployment | undefined;      //retrieved by selection-line
  @Input() updateEmp = new EventEmitter<IEmployment>(); //to be returned to calling program, selections.component
  //@Input() offerAcceptedEvent = new EventEmitter<number>();
  //@Input() OfferRejectedEvent = new EventEmitter<number>();

  bsValue = new Date();
  bsRangeValue= new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();

  categoryRef: string='';
  companyName: string='';
  candidateName = '';

  form: FormGroup = new FormGroup({});
  
  
  statuses = [{status: 'Accepted'}, 
    {status: 'Declined - Salary Low'},
    {status: 'Declined - Company history suspicious'}, 
    {status: 'Declined - Work Hours too long'},
    {status: 'Declined - Cannot pay Services Charges'},
    {status: 'Declined - Family problems'}
  ]

  constructor(public bsModalRef: BsModalRef, private toastr:ToastrService, 
    private confirmService: ConfirmService, private fb: FormBuilder 
    , private service: EmploymentService
    ) { }

  ngOnInit(): void {
    if(this.emp) this.InitializeForm(this.emp);
  }

  InitializeForm(emp: IEmployment) {

    this.form = this.fb.group({
        id: [emp.id],
        selDecisionId: [emp.selectionDecisionId], 
        cvRefId: [emp.cvRefId],
        //orderNo: [emp.orderNo],
        //companyName: [emp.companyName],
        selectionDecisionId: [emp.selectionDecisionId],
        //applicationNo: [emp.applicationNo],
        //categoryRef: [emp.categoryRef],

        selectedOn: [emp.selectedOn],
        charges: [emp.charges],
        salaryCurrency: [emp.salaryCurrency, Validators.required],
        salary: [emp.salary, Validators.required],
        contractPeriodInMonths: [emp.contractPeriodInMonths, Validators.required],
        weeklyHours: [emp.weeklyHours, Validators.required],
        housingProvidedFree: [emp.housingProvidedFree],
        housingNotProvided: [emp.housingNotProvided ?? false],
        housingAllowance: [emp.housingAllowance],
        foodProvidedFree: [emp.foodProvidedFree],
        foodNotProvided: [emp.foodNotProvided ?? false],
        foodAllowance: [emp.foodAllowance],
        transportProvidedFree: [emp.transportProvidedFree],
        transportNotProvided: [emp.transportNotProvided ?? false],
        transportAllowance: [emp.transportAllowance],
        otherAllowance: [emp.otherAllowance],
        leavePerYearInDays: [emp.leavePerYearInDays],
        leaveAirfareEntitlementAfterMonths: [emp.leaveAirfareEntitlementAfterMonths],
        offerAccepted: [emp.offerAccepted],
        offerAcceptanceConcludedOn: [
          emp.offerAcceptanceConcludedOn 
        ]
    })
  }

  updateEmployment() {      //emits the edited emp:IEmployment object

    //this should ideally be handled by the parent component, but cannot implement SwitchMap for more than 2 observables
    //need to study how forkjoin/CombineLatest 

    var formdata = this.form.value;
    formdata.housingAllowance= formdata.housingAllowance ?? 0;
    formdata.foodAllowance = formdata.foodAllowance ?? 0;
    formdata.transportAllowance = formdata.transportAllowance ?? 0;

    var strErr='';
    //verify data
    if(formdata.foodAllowance > 0 && (formdata.foodNotProvided || formdata.foodProvidedFree))
      strErr = 'only one value allowed : either Food Allowance, Food Not Provided Or Food Provided ';
    if(formdata.housingAllowance > 0 && (formdata.housingProvidedFree || formdata.housingNotProvided))
      strErr += 'only one value allowed : either Housing Allowance, Housing Not Provided Or Housing Provided ';
    if(formdata.transportAllowance > 0 && (formdata.transportProvidedFree || formdata.transportNotProvided))
      strErr += 'only one value allowed : either Transport Allowance, Transport Not Provided Or Transport Provided ';
    
    if(formdata.cVRefId===null) formdata.cVRefId=0;

    if(strErr !== '') {
      this.toastr.warning(strErr, 'Invalid data');
      return;
    }
      
      this.updateEmp.emit(formdata);
      this.bsModalRef.hide();
  }

  close() {
      this.bsModalRef.hide();
  }

}
