import { Injectable, inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { Employee, IEmployee } from "../_models/admin/employee";
import { EmployeeService } from "../_services/admin/employee.service";

 export const EmployeeFromIdResolver: ResolveFn<IEmployee> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    console.log('employeefromidresolver', id);
    if (id===null || id==='' || id === '0') return of(new Employee);
    return inject(EmployeeService).getEmployeeById(+id!);
  };