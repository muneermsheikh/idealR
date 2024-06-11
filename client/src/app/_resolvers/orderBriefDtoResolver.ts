import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrderBriefDto } from "../_dtos/admin/orderBriefDto";
import { OrderService } from "../_services/admin/order.service";

 export const OrderBriefDtoResolver: ResolveFn<IOrderBriefDto | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if (id===null) return of(null);
    return inject(OrderService).getOrderBrief(+id!);
  };