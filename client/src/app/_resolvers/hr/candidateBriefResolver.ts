import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { ICandidateBriefDto } from "src/app/_dtos/admin/candidateBriefDto";
import { CandidateService } from "src/app/_services/candidate.service";

export const CandidateBriefResolver: ResolveFn<ICandidateBriefDto | null> = (
    route: ActivatedRouteSnapshot,
  ) => {

      var id = route.paramMap.get('id');
      if (id===null) return of(null);
      return inject(CandidateService).getCandidateBrief(+id!);

  };