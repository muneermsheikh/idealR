import { Injectable, inject } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, ResolveFn } from "@angular/router";
import { Observable, of } from "rxjs";
import { IInterviewItem } from "../shared/models/hr/interviewItem";
import { InterviewService } from "../shared/services/hr/interview.service";
import { candidatesMatchingParams } from "../shared/params/hr/candidatesMatchingParams";
import { IInterviewItemDto } from "../shared/models/hr/interviewItemDto";


 export const InterviewItemResolver: ResolveFn<IInterviewItemDto[] | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('interviewitemid');
    if (id===null) return of(null);
    var params = new candidatesMatchingParams();
    params.interviewId = +id;
    return inject(InterviewService).getInterviewItemCatAndCandidates(+id);
  };