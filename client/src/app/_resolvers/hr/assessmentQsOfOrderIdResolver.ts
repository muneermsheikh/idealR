import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IAssessment } from "../../shared/models/admin/assessment";
import { AssessmentService } from "../../shared/services/hr/assessment.service";
import { IOrderItemAssessment } from "src/app/shared/models/admin/orderItemAssessment";
import { IOrderItemAssessmentQ } from "src/app/shared/models/admin/orderItemAssessmentQ";

 export const AssessmentQsOfOrderIdResolver: ResolveFn<IOrderItemAssessmentQ[]|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if (id===null) return of(null);
    return inject(AssessmentService).getOrderItemAssessmentQs(+id!);
  };