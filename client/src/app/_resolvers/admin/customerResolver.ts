import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { Customer, ICustomer } from "src/app/_models/admin/customer";
import { CustomersService } from "src/app/_services/admin/customers.service";
 
export const CustomerResolver: ResolveFn<ICustomer|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id') ?? '0';
    if(id==='0') {
      var custType=route.paramMap.get('custType') ?? 'customer';
      var cust = new Customer();
      cust.customerType=custType;
      return of(cust);
    }
    return inject(CustomersService).getCustomer(+id!);
  };