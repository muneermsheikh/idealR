import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrderBriefDto } from "../shared/dtos/admin/orderBriefDto";
import { OrderService } from "../shared/services/admin/order.service";

 
 export const OrderBriefResolver: ResolveFn<IOrderBriefDto | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(OrderService).getOrderBrief(+id!);
  };