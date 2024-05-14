import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { ICustomer } from "src/app/_models/admin/customer";
import { CustomersService } from "src/app/_services/admin/customers.service";
 
export const CustomerResolver: ResolveFn<ICustomer|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if(id===null || id==='') return of(null);
    
    return inject(CustomersService).getCustomer(+id!);
  };