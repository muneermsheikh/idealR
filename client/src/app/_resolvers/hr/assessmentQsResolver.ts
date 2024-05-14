import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IAssessment } from "../../shared/models/admin/assessment";
import { AssessmentService } from "../../shared/services/hr/assessment.service";

 export const AssessmentQsResolver: ResolveFn<IAssessment|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(AssessmentService).getOrderItemAssessment(+id!);
  };