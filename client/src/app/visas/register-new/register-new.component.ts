import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { max } from 'rxjs';
import { ICustomerBriefDto } from 'src/app/_dtos/admin/customerBriefDto';
import { IVisaHeader, VisaHeader } from 'src/app/_models/admin/visaHeader';
import { User } from 'src/app/_models/user';
import { VisaService } from 'src/app/_services/admin/visa.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-register-new',
  templateUrl: './register-new.component.html',
  styleUrls: ['./register-new.component.css']
})
export class RegisterNewComponent implements OnInit{

    visa: IVisaHeader | undefined;
    customers: ICustomerBriefDto[] = [];
    bsValueDate = new Date();
    bsValue = new Date();
    user?: User;
    returnUrl = '';
    totalCount = 0;
    visaIdFromRouter = '';
    
    form: FormGroup = new FormGroup({});
    errors: string[]=[];  
  
    constructor(private toastr: ToastrService, private service: VisaService, private activatedRoute: ActivatedRoute,
      private fb: FormBuilder, private confirm: ConfirmService, private router: Router){
        let visaIdFromRouter = activatedRoute.snapshot.paramMap.get('visaid') ?? "0";
        let nav: Navigation|null = this.router.getCurrentNavigation() ;
  
        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;
  
            if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
        }
      }
  
    ngOnInit(): void {    
      this.service.getCustomerList('Customer').subscribe({
        next: (response: ICustomerBriefDto[]) => this.customers = response
      })
      
      /*this.activatedRoute.data.subscribe( data => {
          this.visa = data['visa'],
          this.customers = data['customers']
      }); */

      if(this.visaIdFromRouter == "" || this.visaIdFromRouter === "0") {
        this.visa = new VisaHeader()
      } else {
        this.service.getVisaHeader(+this.visaIdFromRouter).subscribe({
          next: (response: IVisaHeader) => this.visa = response
        })
      }
      if(!this.visa)  this.visa = new VisaHeader();

      this.InitializeForm(this.visa!);
    }
  
    InitializeForm(v: IVisaHeader) {
      this.form = this.fb.group({
        id: v.id ?? 0,  customerId: v.customerId ?? 0,
        customerName: [v.customerName ?? ''], 
        visaNo: '', visaSponsorName: '',
        visaDateG: ['', Validators.required],
        visaExpiryG: ['', Validators.required]
        
        , visaItems: this.fb.array(
          v.visaItems!.map(x => (
            this.fb.group({
              id: x.id ?? 0, 
              srNo: x.srNo ?? 1,
              visaCategoryEnglish: [x.visaCategoryEnglish ?? '', 
                  [Validators.required, Validators.minLength(5), Validators.maxLength(250)]],
              visaCategoryArabic: '',
              visaConsulate: [x.visaConsulate, [Validators.required, Validators.maxLength(  15)]],
              visaQuantity: [x.visaQuantity ?? 0, [Validators.min(1), Validators.max(250)]],
            })
          ))
        )
      })
    }
  
    get visaItems(): FormArray {
      return this.form.get("visaItems") as FormArray
    }

    newVisaItem(): FormGroup {
      var maxSrNo = this.visaItems.length===0 ? 1 
        : Math.max(...this.visaItems.value.map((x: { srNo: number; }) => x.srNo))+1;

      return this.fb.group({
        id: 0, 
        srNo : maxSrNo,
        visaCategoryEnglish: ['', 
            [Validators.required, Validators.minLength(5), Validators.maxLength(250)]],
        visaCategoryArabic: '',
        visaConsulate: ['', Validators.required],
        visaNo: '',
        visaQuantity: [0, [Validators.min(1), Validators.max(250)]],
      })
    }
  
    addVisaItem() {
      if(this.visaItems.length) {
        if(this.form.get('visaNameEnglish')?.invalid || this.form.get('visaQuantity')?.invalid) {
          this.toastr.warning('This form is dirty - ensrue visa name and visa quantity is properly filled in before attemtpign to add another visa item', 
            'form not properly filled in', {closeButton: true, extendedTimeOut: 0});
          return;
        }
      }
      this.visaItems.push(this.newVisaItem());
    }

    removeVisaItem(index: number) {
      this.visaItems.removeAt(index);
      this.form.markAsDirty;
    }

    updateVisa() {
      this.form.markAllAsTouched;
      if(this.errors.length > 0) {
        this.toastr.error('this form has errors.  Fix the errors before attempting to save the form', 
          'Form is dirty', {closeButton: true, extendedTimeOut: 0});
        return;
      }
      var formdata = this.form.value;
      if(formdata.id === 0) {
        this.service.insertNewVisa(formdata).subscribe({
          next: (response: IVisaHeader) => {
            if(response === null) {
              this.toastr.warning('Failed to insert the Visa', 'Failure')
            } else {
              this.toastr.success('Vias inserted', 'Success')
            }
          }, error: (err: any) => this.toastr.error(err.error?.details, 'Error')
        })
      } else {
        this.service.updateVisa(formdata).subscribe({
          next: (response: IVisaHeader) => {
            if(response === null) {
              this.toastr.warning('Failed to save the Visa', 'Failure')
            } else {
              this.toastr.success('Vias saved', 'Success')
            }
          }, error: (err: any) => this.toastr.error(err.error?.details, 'Error')
        })
      }
    }
    
    recalculateCount(): number {
      this.totalCount = +this.visaItems.value.map((x: any) => x.visaQuantity)
        .reduce((a:number,b:number) => +a+(+b),0);
      return this.totalCount;
    }

  
}
