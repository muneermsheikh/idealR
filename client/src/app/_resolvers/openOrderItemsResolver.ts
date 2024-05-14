import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IOrderItemBriefDto } from "../shared/dtos/admin/orderItemBriefDto";
import { OrderService } from "../shared/services/admin/order.service";


 export const OpenOrderItemsResolver: ResolveFn<IOrderItemBriefDto[]> = (
  ) => {
     return inject(OrderService).getOrderItemsBriefDto();
  };