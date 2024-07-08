import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrderForwardToAgentDto } from "src/app/_dtos/orders/orderForwardToAgentDto";
import { OrderForwardService } from "src/app/_services/admin/order-forward.service";


 export const OrderFwdToAgentResolver: ResolveFn<IOrderForwardToAgentDto|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if (id===null) return of(null);
    return inject(OrderForwardService).getOrderForwardOfAnOrder(+id);
  };