import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrderItemAssessmentQ } from "src/app/_models/admin/orderItemAssessmentQ";
import { AssessmentService } from "src/app/_services/hr/assessment.service";

 export const AssessmentQsOfOrderIdResolver: ResolveFn<IOrderItemAssessmentQ[]|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if (id===null) return of(null);
    return inject(AssessmentService).getOrderItemAssessmentQs(+id!);
  };