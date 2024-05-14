import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ICustomerNameAndCity } from "src/app/_models/admin/customernameandcity";
import { ClientService } from "src/app/_services/admin/client.service";


 export const CustomerNameCityResolver: ResolveFn<ICustomerNameAndCity[]|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
     return inject(ClientService).getCustomerAndCities();
  };