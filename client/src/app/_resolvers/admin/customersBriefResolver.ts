import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { ICustomerBriefDto } from "src/app/_dtos/admin/customerBriefDto";
import { paramsCustomer } from "src/app/_models/params/Admin/paramsCustomer";
import { CustomersService } from "src/app/_services/admin/customers.service";

 export const CustomersBriefResolver: ResolveFn<ICustomerBriefDto[] | null> = (
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