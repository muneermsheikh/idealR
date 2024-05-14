import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { IAssessmentQBank } from "../shared/models/admin/assessmentQBank";
import { HrService } from "../shared/services/hr/hr.service";

export const QBankByCategoryIdResolver: ResolveFn<IAssessmentQBank> = (
    route: ActivatedRouteSnapshot,
  ) => {
        var routeid = route.paramMap.get('id');    
        return inject(HrService).getQBankByCategoryId(+routeid!);
  };