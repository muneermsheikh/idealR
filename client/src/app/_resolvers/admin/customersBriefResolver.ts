import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";

 export const CustomersBriefResolver: ResolveFn<IPagination<ICustomerBriefDto[]>|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('custType');
    if(id===null) return of(null);
    if(!(id==='customer' || id==='associate' || id==='vendor')) return of(null);
        
    var params = new paramsCustomer();
    params.customerType=id;
    inject(CustomersService).setCustParams(params);
    return inject(CustomersService).getCustomers(false);
  };