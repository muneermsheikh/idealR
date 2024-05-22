import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IAssessmentStandardQ } from "src/app/_models/admin/assessmentStandardQ";
import { StddqsService } from "src/app/_services/hr/stddqs.service";

 export const AssessmentStddQResolver: ResolveFn<IAssessmentStandardQ|undefined|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(StddqsService).getStddQ(+id!);
    
  };