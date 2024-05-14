import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IAssessmentStandardQ } from "../shared/models/admin/assessmentStandardQ";
import { StddqsService } from "../shared/services/hr/stddqs.service";
 
export const StddQResolver: ResolveFn<IAssessmentStandardQ | null | undefined> = (
    route: ActivatedRouteSnapshot,
    ) => {
        var id = route.paramMap.get('id');
        if (id===null) return of(null);
          return inject(StddqsService).getStddQ(+id);
    };