import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IInterview } from "../_models/hr/interview";
import { InterviewService } from "../_services/hr/interview.service";

 export const InterviewResolver: ResolveFn<IInterview | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderno');
    if (id===null || id === '') return of(null);
    return inject(InterviewService).getOrGenerateinterview(+id!);
  };