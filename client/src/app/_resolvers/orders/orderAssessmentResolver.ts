import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrderAssessment } from "src/app/_models/admin/orderAssessment";
import { OrderAssessmentService } from "src/app/_services/hr/orderAssessment.service";


 export const OrderAssessmentResolver: ResolveFn<IOrderAssessment|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(OrderAssessmentService).getOrderAssessment(+id);
  };