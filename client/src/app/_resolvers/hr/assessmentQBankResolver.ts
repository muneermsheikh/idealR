import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IAssessmentQBank } from "../../shared/models/admin/assessmentQBank";
import { HrService } from "../../shared/services/hr/hr.service";

export const AssessmentQBankResolver: ResolveFn<IAssessmentQBank[]|null> = (
  ) => {
    return inject(HrService).getAssessmentQBank();
  };