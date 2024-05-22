import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IAssessmentStandardQ } from "src/app/_models/admin/assessmentStandardQ";
import { StddqsService } from "src/app/_services/hr/stddqs.service";

 export const AssessmentStddQsResolver: ResolveFn<IAssessmentStandardQ[]|null> = (
  ) => {

      return inject(StddqsService).getStddQsWithoutCache();

  };