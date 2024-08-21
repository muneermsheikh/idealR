import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IIntervw } from "src/app/_models/hr/intervw";
import { InterviewService } from "src/app/_services/hr/interview.service";


 export const IntervwResolver: ResolveFn<IIntervw | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderno');
    if (id===null || id === '') return of(null);
    return inject(InterviewService).getOrGenerateinterviewnew(+id!);
  };