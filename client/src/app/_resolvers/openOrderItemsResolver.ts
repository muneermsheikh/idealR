import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IOrderItemBriefDto } from "../_dtos/admin/orderItemBriefDto";
import { OrderService } from "../_services/admin/order.service";

 export const OpenOrderItemsResolver: ResolveFn<IOrderItemBriefDto[]> = (
  ) => {
     return inject(OrderService).getOpenOrderItemsBriefListDto();
  };