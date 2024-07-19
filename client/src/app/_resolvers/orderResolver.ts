import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrder, Order } from "../_models/admin/order";
import { OrderService } from "../_services/admin/order.service";

export const OrderResolver: ResolveFn<IOrder | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    if (id==="0") return of(new Order());
    return inject(OrderService).getOrder(+id!);
  };