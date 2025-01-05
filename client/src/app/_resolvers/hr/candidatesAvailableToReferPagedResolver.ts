import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ICvsAvailableDto } from "src/app/_dtos/admin/cvsAvailableDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { candidateParams } from "src/app/_models/params/hr/candidateParams";
import { CandidateService } from "src/app/_services/candidate.service";


export const CandidatesAvailableToReferPagedResolver: ResolveFn<PaginatedResult<ICvsAvailableDto[]>|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

      var cvParams = new candidateParams();

      return inject(CandidateService).getAvailableCandidatesPaged(cvParams, false);

  };
