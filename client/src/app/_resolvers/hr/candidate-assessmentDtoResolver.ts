import { inject } from '@angular/core';
import {ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot} from '@angular/router';
import { of } from 'rxjs';
import { ICandidateAssessmentDto } from 'src/app/_dtos/hr/candidateAssessmentDto';
import { CVAssessService } from 'src/app/_services/cvassess.service';
import { CandidateAssessmentService } from 'src/app/_services/hr/candidate-assessment.service';

export const CandidateAssessmentDtoResolver: ResolveFn<ICandidateAssessmentDto|null> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {

    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    var orderitemid = route.paramMap.get('orderitemid');
    if(orderitemid===null) return of(null);

    return inject(CandidateAssessmentService).getCandidateAssessmentDto (+id!, +orderitemid!);

};

