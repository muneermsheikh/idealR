import { Injectable, inject } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, ResolveFn } from "@angular/router";
import { Observable, of } from "rxjs";
import { IInterviewItemDto } from "../_models/hr/interviewItemDto";
import { candidatesMatchingParams } from "../_models/params/hr/candidatesMatchingParams";
import { InterviewService } from "../_services/hr/interview.service";

 export const InterviewItemResolver: ResolveFn<IInterviewItemDto[] | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('interviewitemid');
    if (id===null) return of(null);
    var params = new candidatesMatchingParams();
    params.interviewId = +id;
    return inject(InterviewService).getInterviewItemCatAndCandidates(+id);
  };