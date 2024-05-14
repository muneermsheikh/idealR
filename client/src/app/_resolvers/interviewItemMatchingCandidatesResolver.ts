import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { InterviewService } from "../shared/services/hr/interview.service";
import { IInterviewItemDto } from "../shared/models/hr/interviewItemDto";


 export const InterviewItemMatchingCandidatesdResolver: ResolveFn<IInterviewItemDto[] | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('interviewitemid');
    if (id===null) return of(null);
    return inject(InterviewService).getInterviewItemCatAndCandidates(+id!);
  };