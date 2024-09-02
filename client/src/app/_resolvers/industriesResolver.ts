import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IIndustryType } from "../_models/admin/industryType";
import { EmployeeService } from "../_services/admin/employee.service";

export const IndustryListResolver: ResolveFn<IIndustryType[]> = (
  ) => {
    return inject(EmployeeService).getIndustryList();
  };