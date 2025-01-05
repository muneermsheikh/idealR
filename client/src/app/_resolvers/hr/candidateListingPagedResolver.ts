import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ICandidateBriefDto } from "src/app/_dtos/admin/candidateBriefDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { candidateParams } from "src/app/_models/params/hr/candidateParams";
import { CandidateService } from "src/app/_services/candidate.service";


export const CandidateListingPagedResolver: ResolveFn<PaginatedResult<ICandidateBriefDto[]>|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

        var cvParams = new candidateParams();
        return inject(CandidateService).getCandidatesPaged(cvParams,false);

  };
