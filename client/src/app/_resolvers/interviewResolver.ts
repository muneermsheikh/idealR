import { Injectable, inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { Observable, of } from "rxjs";

import { IInterview } from "../shared/models/hr/interview";
import { InterviewService } from "../shared/services/hr/interview.service";


 export const InterviewResolver: ResolveFn<IInterview | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if (id===null) return of(null);
    return inject(InterviewService).getOrCreateInterview(+id!);
  };