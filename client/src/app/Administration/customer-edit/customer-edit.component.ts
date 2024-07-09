import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, RouteConfigLoadEnd, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ICustomer } from 'src/app/_models/admin/customer';
import { User } from 'src/app/_models/user';
import { CustomersService } from 'src/app/_services/admin/customers.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-customer-edit',
  templateUrl: './customer-edit.component.html',
  styleUrls: ['./customer-edit.component.css']
})
export class CustomerEditComponent implements OnInit {
  
  customer: ICustomer | undefined;
  
  bsValueDate = new Date();
  bsValue = new Date();
  user?: User;
  returnUrl = '';
  
  form: FormGroup = new FormGroup({});


  constructor(private toastr: ToastrService, private service: CustomersService, private activatedRoute: ActivatedRoute,
    private fb: FormBuilder, private confirm: ConfirmService, private router: Router){
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

      if (nav?.extras && nav.extras.state) {
          if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

          if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
      }
    }

  ngOnInit(): void {

    
    this.activatedRoute.data.subscribe(data => {
      this.customer = data['customer'];
      if(this.customer !== undefined) this.InitializeForm(this.customer);
      }
    )
   
    
  }

  InitializeForm(cust: ICustomer) {

    this.form = this.fb.group({
      id: cust.id,  customerName: [cust.customerName, Validators.required], knownAs: [cust.knownAs, Validators.required], 
      customerType: [cust.customerType, Validators.required], add: cust.add, add2: cust.add2, city: [cust.city, Validators.required], 
      pin: cust.pin, district: cust.district, state: cust.state, country: cust.country, email: [cust.email, Validators.required], 
      website: cust.website, phone: cust.phone, phone2: cust.phone2, customerStatus: cust.customerStatus,
      createdOn: cust.createdOn, introduction: cust.introduction,
      
      customerOfficials: this.fb.array(
        cust.customerOfficials.map(x => (
          this.fb.group({
            id: x.id, appUserId: x.appUserId, customerId: x.customerId, gender: [x.gender, Validators.required], 
            title: [x.title, Validators.required], officialName: [x.officialName, Validators.required], 
            designation: x.designation, divn: [x.divn, Validators.required], phoneNo: x.phoneNo, knownAs: x.knownAs,
            mobile: x.mobile, email: [x.email, Validators.required], status: x.status, priorityHR: x.priorityHR,
            priorityAdmin: x.priorityAdmin, priorityAccount: x.priorityAccount,userName: x.userName, 
          })
        ))
      )

      /* , vendorSpecialties: this.fb.array(
        cust.vendorSpecialties.map(x => (
          this.fb.group({
            id: x.id, customerId: x.customerId, name: x.name,
            vendorFacilityId: x.vendorFacilityId, 
          })
        ))
      )
        */
      
      , agencySpecialties: this.fb.array(
        cust.agencySpecialties.map(x => (
          this.fb.group({
            id: x.id, customerId: x.customerId, professionId: x.professionId,
            name: x.name
          })
        ))
      )
      
    })
  }

  //customerofficials
      get customerOfficials(): FormArray {
        return this.form.get("customerOfficials") as FormArray
      }

      newCustomerOfficial(): FormGroup {
        return this.fb.group({

            id: 0, appUserId: 0, customerId: this.customer?.id, gender: 'Male',
            title: '', officialName: '', designation: '', knownAs: '',
            divn: '', phoneNo: '',  mobile: '', email: '', status: 'Active', 
            priorityHR: 1, priorityAdmin: 1, priorityAccount: 1, userName: ''
        })
      }

      addCustomerOfficial() {
        this.customerOfficials.push(this.newCustomerOfficial());
      }

      removeCustomerOfficial(index: number) {
        
          this.confirm.confirm("Confirm Delete", "This will delete customer official.  To delete from the database as well, remember to SAVE this form before you close it")
            .subscribe({next: confirmed => {
              if(confirmed) this.customerOfficials.removeAt(index);
          }})
      }

//agency specialties

      get vendorSpecialties(): FormArray {
        return this.form.get("vendorSpecialties") as FormArray
      }

      newVendorSpecialty(): FormGroup {
        return this.fb.group({

            id: 0, customerId: this.customer?.id, 
            vendorFacilityId: 0, name: ''
        })
      }

      addVendorSpecialty() {
        this.vendorSpecialties.push(this.newVendorSpecialty());
      }

      removeVendorSpecialty(index: number) {
        
          this.confirm.confirm("Confirm Delete", "This will delete Vendor specialty.  To delete from the database as well, remember to SAVE this form before you close it")
            .subscribe({next: confirmed => {
              if(confirmed) this.vendorSpecialties.removeAt(index);
          }})
      }

//event emitters

  updateCustomer() {
    var formdata = this.form.value;
    this.service.updateCustomer(formdata).subscribe({
      next: response => {
        if(response === '') {
          this.toastr.success('The Customer is successfully updated', 'success');
          this.close();
        }
      }
    })
  }

  close() {
    this.router.navigateByUrl(this.returnUrl);
  }
}
