import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrderForwardToAgent } from "src/app/_models/orders/orderForwardToAgent";
import { OrderForwardService } from "src/app/_services/admin/order-forward.service";


 export const OrderFwdToAgentResolver: ResolveFn<IOrderForwardToAgent|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if (id===null) return of(null);
    return inject(OrderForwardService).getOrderForwardOfAnOrder(+id);
  };