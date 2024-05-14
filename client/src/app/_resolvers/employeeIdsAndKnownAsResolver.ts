import { Injectable, inject } from "@angular/core";
import { Resolve, ResolveFn } from "@angular/router";
import { Observable } from "rxjs";
import { IEmployeeIdAndKnownAs } from "../shared/models/admin/employeeIdAndKnownAs";
import { MastersService } from "../shared/services/masters.service";


export const EmployeeIdsAndKnownAsResolver: ResolveFn<IEmployeeIdAndKnownAs[]|null> = (
  ) => {
    return inject(MastersService).getEmployeeIdAndKnownAs();
  };