import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { IClientIdAndNameDto } from "src/app/_dtos/admin/clientIdAndNameDto";
import { CustomersService } from "src/app/_services/admin/customers.service";


 export const CustomerIdAndNamesResolver: ResolveFn<IClientIdAndNameDto[]|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
     return inject(CustomersService).getClientIdAndNames();
  };