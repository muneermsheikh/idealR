import { Injectable, inject } from "@angular/core";
import { Resolve, ResolveFn } from "@angular/router";
import { Observable } from "rxjs";
import { IPagination } from "../shared/models/pagination";
import { IEmployment } from "../shared/models/admin/employment";
import { employmentParams } from "../shared/params/admin/employmentParam";
import { EmploymentService } from "../shared/services/admin/employment.service";

export const EmploymentsResolver: ResolveFn<IPagination<IEmployment[]> | null> = (
  ) => {
    inject(EmploymentService).setEParams(new employmentParams());
    return inject(EmploymentService).getEmployments(false);
  };