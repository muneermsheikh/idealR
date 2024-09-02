import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ISkillData } from "src/app/_models/hr/skillData";
import { EmployeeService } from "src/app/_services/admin/employee.service";

export const SkillDataResolver: ResolveFn<ISkillData[] | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    return inject(EmployeeService).getSkillDatas();
  };