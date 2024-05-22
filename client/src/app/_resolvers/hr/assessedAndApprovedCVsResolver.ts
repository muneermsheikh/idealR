import { ResolveFn } from "@angular/router";
import { inject } from "@angular/core";
import { ICandidateAssessedDto } from "src/app/_dtos/hr/candidateAssessedDto";
import { CvrefService } from "src/app/_services/hr/cvref.service";

 
export const AssessedAndApprovedCVsResolver: ResolveFn<ICandidateAssessedDto[] | null> = (
  ) => {
    return inject(CvrefService).getShortlistedCandidates(false);
  };