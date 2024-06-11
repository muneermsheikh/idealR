import { inject } from '@angular/core';
import {ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot} from '@angular/router';
import { of } from 'rxjs';
import { ICandidateAssessedShortDto } from 'src/app/_dtos/hr/candidateAssessedShortDto';
import { CVAssessService } from 'src/app/_services/cvassess.service';

export const CandidateAssessedResolver: ResolveFn<ICandidateAssessedShortDto[]|null> = (
  route: ActivatedRouteSnapshot ) => {

    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(CVAssessService).getAssessmentsOfACandidate(+id!);

};

