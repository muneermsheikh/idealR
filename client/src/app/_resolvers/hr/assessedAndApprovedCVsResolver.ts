import { ResolveFn } from "@angular/router";
import { ICandidateAssessedDto } from "../../shared/dtos/hr/candidateAssessedDto";
import { IPagination } from "src/app/shared/models/pagination";
import { inject } from "@angular/core";
import { CvrefService } from "src/app/shared/services/hr/cvref.service";
 
export const AssessedAndApprovedCVsResolver: ResolveFn<IPagination<ICandidateAssessedDto[]>|null> = (
  ) => {
    return inject(CvrefService).getShortlistedCandidates(false);
  };