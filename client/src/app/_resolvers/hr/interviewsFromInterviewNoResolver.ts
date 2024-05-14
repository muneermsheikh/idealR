import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from "@angular/router";
import { of } from "rxjs";
import { IInterview } from "src/app/shared/models/hr/interview";
import { InterviewService } from "src/app/shared/services/hr/interview.service";
 
export const InterviewFromOrderNoResolver: ResolveFn<IInterview|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderno');
    if (id===null) return of(null);
    return inject(InterviewService).getOrCreateInterviewFromOrderNo(+id!);
  };