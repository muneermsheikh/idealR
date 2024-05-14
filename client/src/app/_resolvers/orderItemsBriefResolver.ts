import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrderItemBriefDto } from "../shared/dtos/admin/orderItemBriefDto";
import { OrderitemsService } from "../shared/services/admin/orderitems.service";

 
 export const OrderItemsBriefResolver: ResolveFn<IOrderItemBriefDto[] | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(OrderitemsService).getOrderItems(+id!, false);
  };