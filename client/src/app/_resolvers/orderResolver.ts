import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrder } from "../shared/models/admin/order";
import { OrderService } from "../shared/services/admin/order.service";

export const OrderResolver: ResolveFn<IOrder | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(OrderService).getOrder(+id!);
  };