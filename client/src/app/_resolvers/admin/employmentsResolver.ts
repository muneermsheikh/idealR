import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IEmployment } from "src/app/shared/models/admin/employment";
import { IPagination } from "src/app/shared/models/pagination";
import { EmploymentService } from "src/app/shared/services/admin/employment.service";

 export const LoggedInUserTaskResolver: ResolveFn<IPagination<IEmployment[]>|null> = (
  ) => {
    return inject(EmploymentService).getEmployments(false);
  };