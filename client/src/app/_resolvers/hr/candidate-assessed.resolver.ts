import { inject } from '@angular/core';
import {ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot} from '@angular/router';
import { of } from 'rxjs';
import { ICandidateAssessedDto } from 'src/app/_dtos/hr/candidateAssessedDto';
import { CVAssessService } from 'src/app/_services/cvassess.service';

export const CandidateAssessedResolver: ResolveFn<ICandidateAssessedDto[]|null> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {

    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(CVAssessService).getCVAssessmentsOfACandidate(+id!);

};

