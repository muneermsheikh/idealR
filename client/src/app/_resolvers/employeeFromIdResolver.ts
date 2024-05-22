import { Injectable, inject } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, ResolveFn } from "@angular/router";
import { Observable, of } from "rxjs";
import { IEmployee } from "../_models/admin/employee";
import { EmployeeService } from "../_services/admin/employee.service";

@Injectable({
     providedIn: 'root'
 })

 export const EmployeeFromIdResolver: ResolveFn<IEmployee|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(EmployeeService).getEmployee(+id!);
  };