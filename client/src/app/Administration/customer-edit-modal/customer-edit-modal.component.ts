import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICustomer } from 'src/app/_models/admin/customer';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-customer-edit-modal',
  templateUrl: './customer-edit-modal.component.html',
  styleUrls: ['./customer-edit-modal.component.css']
})
export class CustomerEditModalComponent implements OnInit {

  @Output() updateEvent = new EventEmitter<ICustomer>();
  
  customer: ICustomer | undefined;
  
  bsValueDate = new Date();
  bsValue = new Date();
  user?: User;
  
  form: FormGroup = new FormGroup({});

  
  constructor(private bsModalRef: BsModalRef, private toastr: ToastrService,
    private fb: FormBuilder, private confirm: ConfirmService){}

  ngOnInit(): void {
    if(this.customer !== undefined) this.InitializeForm(this.customer);
  }

  InitializeForm(cust: ICustomer) {

    this.form = this.fb.group({
      id: cust.id,  customerName: cust.customerName, knownAs: cust.knownAs, customerType: cust.customerType,
      add: cust.add, add2: cust.add2, city: cust.city, pin: cust.pin, district: cust.district,
      state: cust.state, country: cust.country, email: cust.email, website: cust.website,
      phone: cust.phone, phone2: cust.phone2, customerStatus: cust.customerStatus,
      createdOn: cust.createdOn, introduction: cust.introduction,
      
      customerOfficials: this.fb.array(
        cust.customerOfficials.map(x => (
          this.fb.group({
            id: x.id, appUserId: x.appUserId, customerId: x.customerId,
            gender: x.gender, title: x.title, officialName: x.officialName, 
            designation: x.designation, divn: x.divn, phoneNo: x.phoneNo, 
            mobile: x.mobile, email: x.email, status: x.status, priorityHR: x.priorityHR,
            priorityAdmin: x.priorityAdmin, priorityAccount: x.priorityAccount
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

            id: 0, appUserId: 0, customerId: this.customer?.id, 
            gender: "Male", title: '', officialName: '', designation: '', 
            divn: '', phoneNo: '',  mobile: '', email: '', status: 'Active', 
            priorityHR: 1, priorityAdmin: 1, priorityAccount: 1
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
    this.updateEvent.emit(formdata);
    this.bsModalRef.hide();
  }

  close() {
    this.bsModalRef.hide();
  }

}
