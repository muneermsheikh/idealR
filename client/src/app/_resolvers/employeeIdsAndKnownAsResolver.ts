import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IEmployeeIdAndKnownAs } from "../_models/admin/employeeIdAndKnownAs";
import { EmployeeService } from "../_services/admin/employee.service";


export const EmployeeIdsAndKnownAsResolver: ResolveFn<IEmployeeIdAndKnownAs[]|null> = (
  ) => {
    return inject(EmployeeService).getEmployeeIdAndKnownAs();
  };