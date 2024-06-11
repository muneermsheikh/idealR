import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrderAssessmentItem } from "src/app/_models/admin/orderAssessmentItem";
import { OrderAssessmentService } from "src/app/_services/hr/orderAssessment.service";


 export const OrderAssessmentItemResolver: ResolveFn<IOrderAssessmentItem|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(OrderAssessmentService).getOrderAssessmentItem(+id);
  };