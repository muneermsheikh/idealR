import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { CandidateService } from "../../shared/services/candidate.service";
import { ICandidateBriefDto } from "../../shared/dtos/admin/candidateBriefDto";

 export const CandidateByAppNoResolver: ResolveFn<ICandidateBriefDto|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('appno');
    if (id===null) return of(null);
    return inject(CandidateService).getCandidateBriefDtoFromAppNo(+id!);
  };