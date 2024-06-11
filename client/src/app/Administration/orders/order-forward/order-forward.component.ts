import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, iif, of, switchMap, tap } from 'rxjs';
import { IOfficialAndCustomerNameDto } from 'src/app/_dtos/admin/client/oficialAndCustomerNameDto';
import { IOrderForwardCategory } from 'src/app/_models/orders/orderForwardCategory';
import { IOrderForwardCategoryOfficial } from 'src/app/_models/orders/orderForwardCategoryOfficial';
import { IOrderForwardToAgent } from 'src/app/_models/orders/orderForwardToAgent';
import { OrderForwardService } from 'src/app/_services/admin/order-forward.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-order-forward',
  templateUrl: './order-forward.component.html',
  styleUrls: ['./order-forward.component.css']
})
export class OrderForwardComponent implements OnInit {

  form: FormGroup = new FormGroup({});
  orderFwd: IOrderForwardToAgent | undefined;
  customers: IOfficialAndCustomerNameDto[]=[];
  bsValueDate = new Date();

  constructor(private fb: FormBuilder, private toastr: ToastrService, private router: Router,
    private service: OrderForwardService, private activatedRoute: ActivatedRoute, private confirm: ConfirmService) {}
    
  ngOnInit(): void {
      this.activatedRoute.data.subscribe({
        next: response => {
            this.orderFwd = response['orderfwd'];
            this.customers = response['customers']
        }
      })
   
      if(this.orderFwd) this.CreateAndInitializeForm(this.orderFwd);
  }
  
  CreateAndInitializeForm(fwd: IOrderForwardToAgent) {
    console.log('fwd: ', fwd);
    this.form = this.fb.group({
      id: [fwd.id ?? 0],
      orderId: [fwd.orderId ?? 0, Validators.required],
      orderNo: [fwd.orderNo ?? 0, Validators.required],
      orderDate: [fwd.orderDate ?? new Date(), Validators.required],
      customerId: [fwd.customerId ?? 0, Validators.required],
      customerName: [fwd.customerName ?? ''],
      customerCity: [fwd.customerCity] ?? '',
      projectManagerId: [fwd.projectManagerId ?? 0],

      orderForwardCategories: this.fb.array(
        fwd.orderForwardCategories.map(cat => (
          this.fb.group({
              id: [cat.id ?? 0],
              orderId: [cat.orderId ?? 0, Validators.required],
              orderItemId: [cat.orderItemId ?? 0, Validators.required],
              orderForwardToAgentId: [cat.orderForwardToAgentId ?? 0, Validators.required],
              professionId: [cat.professionId ?? 0, Validators.required],
              professionName: [cat.professionName ?? ''],
              charges: [cat.charges ?? 0],

              orderForwardCategoryOfficials: this.fb.array(
                cat.orderForwardCategoryOfficials.map(off => (
                    this.fb.group({
                        id: [off.id],
                        orderForwardCategoryId: [off.orderForwardCategoryId ?? 0, Validators.required],
                        orderItemId: [off.orderItemId ?? 0, Validators.required],
                        customerOfficialId: [off.customerOfficialId ?? 0, Validators.required],
                        officialName: [off.officialName ?? ''],
                        agentName: [off.agentName ?? '', Validators.required],
                        dateTimeForwarded: [off.dateTimeForwarded ?? new Date()],
                        emailIdForwardedTo: [off.emailIdForwardedTo ?? ''],
                        phoneNoForwardedTo: [off.phoneNoForwardedTo ?? ''],
                        whatsAppNoForwardedTo: [off.whatsAppNoForwardedTo ?? ''],
                        username: [off.username ?? '']
                    })
                ))
              )
          })
        ))
      )
    })
  }

 //teachers: forwards, batches: category, students: officials
  /** Forwards */
  orderForwards(): FormArray {
    return this.form.get("orderForwards") as FormArray
  }

  newForward(): FormGroup {
    return this.fb.group({
      orderNo: '',
      orderDate: '',
      customerName: '',
      dlForwardCategories: this.fb.array([])
    })
  }
 
 
  addForward() {
    this.orderForwards().push(this.newForward());
  }
 

  removeForward() {

    var orderid = this.orderFwd?.orderId ?? 0;
    var confirmMsg = 'confirm delete this Order Forward.' +
      'WARNING: deleting this order forward will also delete its children ' +
      ' grand children objects';

    const observableInner = this.service.deleteForward(orderid);
    const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap((confirmed) => {
          console.log('switchmap confirmed vaue:', confirmed);
          return observableInner
        })
    ).subscribe(response => {
      console.log('subscribed response:', response);
      this.router.navigateByUrl('administration/orders');
    });

    /*
    this.confirm.confirm('confirm Delete', confirmMsg).pipe(
          switchMap(confirmed => iif(() => confirmed, observableInner, of(confirmed))) 
      ).subscribe(
        () => {
            this.toastr.success('Deleted the Order Forward Object', '');
        },
        (err: any) => {
          console.log('any error NOT handled n catchError() or ir throwErro() is returned instead of of() inside catchError()', err);
          this.toastr.error(err, 'error not handled in catchError');
        }
      )
      */
  }
    
   
  orderForwardCategories(): FormArray {
    return this.form.get('orderForwardCategories') as FormArray
  }
 
 
  newCategory(): FormGroup {
    return this.fb.group({
      categoryName: '',
      charges: 0,
      dlForwardCategoryOfficials: this.fb.array([])
    })
  }
 
  addCategory() {
    this.orderForwardCategories().push(this.newCategory());
  }

  removeCategory(bi: number) {
    var confirmMsg = 'confirm delete this Order Forward.' +
      'WARNING: deleting this order forward will also delete its children ' +
      ' grand children objects';

    const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);
    observableOuter.subscribe(confirmed => {
      if(confirmed) {
        this.orderForwardCategoryOfficials(bi).clear();
        this.orderForwardCategories().removeAt(bi);
        this.form.markAsDirty;
        this.form.markAsTouched;
      }
    })
  }
   
  removeSWitchMapAndFilterEx(bi: number) {

    var confirmMsg = 'confirm delete this Order Forward.' +
      'WARNING: deleting this order forward will also delete its children ' +
      ' grand children objects';

    const observableInner = this.service.deleteOrderFwdCategory(bi);
    const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap((confirmed) => {
          return observableInner
        })
    ).subscribe(response => {
      this.orderForwardCategoryOfficials(bi).clear();
      this.orderForwardCategories().removeAt(bi);
      this.router.navigateByUrl('administration/orders');
    });

    
  }
  
 
  orderForwardCategoryOfficials(bi: number): FormArray {
    return this.orderForwardCategories().at(bi).get("orderForwardCategoryOfficials") as FormArray
  }
 
  newOfficial(bi: number): FormGroup {
    return this.fb.group({
      id: 0,
      agentName: '',
      customerOfficialId: 0,
      dateTimeForwarded: new Date(),
      emailIdForwardedTo:'',
      orderForwardCategoryId: this.orderForwardCategories().at(bi).get("id"),
      orderItemId: this.orderForwardCategories().at(bi).get("orderItemId"),
      officialName: ''

    })
  }
 
  addOfficial(bi: number) {
    this.orderForwardCategoryOfficials(bi).push(this.newOfficial(bi));
  }
 
  removeOfficial(bi: number, si: number) {
    this.orderForwardCategoryOfficials(bi).removeAt(si);
    this.form.markAsDirty;
    this.form.markAsTouched;
  }
 
 
  onSubmit() {
    var err = this.formContainsError();
    if(err !== '') {
      this.toastr.info(err, "Form contains error");
      return;
    }
    
    if(this.form.get('id')?.value === 0 ) {
        this.service.insertForwarOrderToAgent(this.form.value).subscribe({
          next: (response: any) => {
            this.toastr.success('Order Forward registered successfully', 'success');
            this.close();
          },
          error: (error: any) => {
            this.toastr.error('error encountered', error);
          }
        })
    } else {

      this.service.updateForwarOrderToAgent(this.form.value).subscribe({
        next: (response: any) => {
          this.toastr.success('Order Forward registered successfully', 'success');
          this.close();
        },
        error: (error: any) => {
          this.toastr.error('Error encountered', error);
        }
      })

    }
 
  }

  formContainsError() {
    var formdata = this.form.value;
    var err: string='';
    formdata.orderForwardCategories.forEach((cat: IOrderForwardCategory) => {
        cat.orderForwardCategoryOfficials.forEach((off:IOrderForwardCategoryOfficial) => {
            if(off.customerOfficialId===0) err +='Customer Not Selected';
            if(off.dateOnlyForwarded!== undefined) err += 'Date forwarded not selected';
        })
    });

    this.toastr.info(err, 'info');
    return err;
  }

  officialClicked(event: any, categoryIndex: number, officialIndex: number){
    this.orderForwardCategoryOfficials(categoryIndex).at(officialIndex).get('officialName')?.setValue(event.officialName);
  }

  close() {
    this.router.navigateByUrl('/administration/orders');
  }

  
}
