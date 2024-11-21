import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { Candidate, ICandidate } from "src/app/_models/hr/candidate";
import { CandidateService } from "src/app/_services/candidate.service";
 
export const CandidateResolver: ResolveFn<ICandidate|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    console.log('candidate resolver id:', id);
    if (id===null ||  id === '' || id === '0') {
      return of(new Candidate());
    }
    return inject(CandidateService).getCandidate(+id);
  };