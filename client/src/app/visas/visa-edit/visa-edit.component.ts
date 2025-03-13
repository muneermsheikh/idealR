import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { ICustomerBriefDto } from 'src/app/_dtos/admin/customerBriefDto';
import { ICustomer } from 'src/app/_models/admin/customer';
import { IVisaHeader, VisaHeader } from 'src/app/_models/admin/visaHeader';
import { User } from 'src/app/_models/user';
import { CustomersService } from 'src/app/_services/admin/customers.service';
import { VisaService } from 'src/app/_services/admin/visa.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-visa-edit',
  templateUrl: './visa-edit.component.html',
  styleUrls: ['./visa-edit.component.css']
})
export class VisaEditComponent implements OnInit {

  customers: ICustomerBriefDto[] = [];
  visa: IVisaHeader | undefined;

  bsValueDate = new Date();
  bsValue = new Date();
  user?: User;

  form: FormGroup = new FormGroup({});

  constructor(private toastr: ToastrService, private service: VisaService,
    private fb: FormBuilder, private confirm: ConfirmService, private custService: CustomersService,
    private router: Router, private activatedRoute: ActivatedRoute, private cd: ChangeDetectorRef) {
      let nav: Navigation | null = this.router.getCurrentNavigation();  

      if(nav?.extras && nav?.extras.state) {
          let nav: Navigation|null = this.router.getCurrentNavigation() ;
          if( nav?.extras.state!['user']) this.user = nav?.extras.state!['user'] as User;
        }
    }
  
    ngOnInit(): void {
      this.activatedRoute.data.subscribe(data => {
          //this.customers = data['customers'];
          //this.visa = data['visa'];

          this.Initialize(this.visa!);
      })
      
      this.custService.getCustomerList('Customer').subscribe({
        next: (response: ICustomerBriefDto[]) => this.customers = response
      }) 
      //if(this.visa===undefined || this.visa === null) this.visa = new VisaHeader();
      //this.Initialize(this.visa);
    }

    Initialize(v: IVisaHeader) {
    
      this.form = this.fb.group({
          id: 0, visaNo: '', customerId: 0, customerName: ['', Validators.required], 
          visaDateG: ['', Validators.required], visaDateH: [''], visaExpiryG: ['', Validators.required],
          visaSponsorName: ['', Validators.required],
          
          visaItems: this.fb.array(
              v.visaItems.map(x => (
                this.fb.group({
                  id: 0, visaHeaderId: 0, visaNo: '', visaCategoryArabic: '',
                  visaCategoryEnglish: ['', Validators.required], visaConsulate: ['', Validators.required], 
                  visaQuantity: [0, Validators.required], visaCanceled: false
                })
              ))
          )
      })
    }

    get visaItems(): FormArray {
      var arr= this.form.get("visaItems") as FormArray;
      return arr;
    }

    newVisaItem(): FormGroup {
      return this.fb.group({
        id: 0, visaHeaderId: this.visa?.id, visaNo: ['', Validators.required],
        visaCategoryArabic: '', visaCategoryEnglish: ['', Validators.required],
        visaConsulate: ['', Validators.required], 
        visaQuantity: [1, [Validators.required, Validators.min(1)]],
        visaCanceled: false
      })
    }

    addVisaItem() {
      this.visaItems.push(this.newVisaItem);
      this.cd.detectChanges();
      this.form.markAsDirty;
    }

    removeVisaItem(index: number) {
      this.visaItems.removeAt(index);
      this.form.markAsDirty;
    }

    updateVisa() {
      var formdata = this.form.value;

      if(formdata.id === 0) {
        this.service.insertNewVisa(formdata).subscribe({
          next: (response: IVisaHeader) => {
            if(response !== null) {
              this.toastr.success('Visa inserted', 'Success');
            } else {
              this.toastr.warning('Failed to insert the visa', 'Failed to insert')
            }
          }, error: (err: any) => this.toastr.error(err.error?.details, 'Error encountered')
        })
      } else {
        this.service.updateVisa(formdata).subscribe({
          next: (response: IVisaHeader) => {
            if(response !== null) {
              this.toastr.success('Visa updated', 'Success')
            } else {
              this.toastr.warning('Failure', 'Failed to update the Visa')
            }
          }, error: (err: any) => this.toastr.error(err.error?.details, 'Error encountered')
        })
      }
    }

    deleteVisa() {
      this.service.deleteVisa(this.visa!.id).subscribe({
        next: (succeeded: boolean) => {
          if(succeeded) {
            this.toastr.success('Visa deleted', 'deletion succeeded')
          } else {
            this.toastr.warning('Failed to delete te visa', 'Failure')
          }
        }, error: (err: any) => this.toastr.error(err.error?.details, 'Error encountered')
      })
    }

    close() {
      this.router.navigateByUrl('/')
    }
}
