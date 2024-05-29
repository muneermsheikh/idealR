import { HttpParams } from "@angular/common/http";
import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { CandidateBriefDto, ICandidateBriefDto } from "src/app/_dtos/admin/candidateBriefDto";
import { Candidate, ICandidate } from "src/app/_models/hr/candidate";
import { CandidateService } from "src/app/_services/candidate.service";
 
export const CandidateBriefDtoByIdResolver: ResolveFn<ICandidateBriefDto|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null ||  id === '' || id === '0') {
      return of(new CandidateBriefDto());
    }

    return inject(CandidateService).getCandidateBriefDtoFromId(+id);
  };