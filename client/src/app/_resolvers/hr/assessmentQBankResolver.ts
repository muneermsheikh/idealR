import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IAssessmentQBank } from "src/app/_models/admin/assessmentQBank";
import { HrService } from "src/app/_services/hr/hr.service";

export const AssessmentQBankResolver: ResolveFn<IAssessmentQBank[]|null> = (
  ) => {
    return inject(HrService).getAssessmentQBank();
  };