import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IAssessmentStandardQ } from "../../shared/models/admin/assessmentStandardQ";
import { StddqsService } from "../../shared/services/hr/stddqs.service";


 export const AssessmentStddQsResolver: ResolveFn<IAssessmentStandardQ[]|null> = (
  ) => {
    return inject(StddqsService).getStddQsWithoutCache();
  };