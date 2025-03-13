import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { ICustomerBriefDto } from "src/app/_dtos/admin/customerBriefDto";
import { customerParams } from "src/app/_models/params/Admin/customerParams";
import { CustomersService } from "src/app/_services/admin/customers.service";

 export const CustomersBriefResolver: ResolveFn<ICustomerBriefDto[] | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('custType');
    if(id===null) return of(null);
    if(!(id==='associate' || id==='vendor')) id='Customer'
    console.log('custType', id);
    return inject(CustomersService).getCustomerList(id);
  };