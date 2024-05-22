import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IEmployment } from "src/app/_models/admin/employment";
import { EmploymentService } from "src/app/_services/admin/employment.service";

 export const LoggedInUserTaskResolver: ResolveFn<IEmployment[] | null> = (
  ) => {
    return inject(EmploymentService).getEmployments(false);
  };