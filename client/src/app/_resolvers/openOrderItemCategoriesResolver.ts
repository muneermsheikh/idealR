import { inject } from "@angular/core";
import { IOpenOrderItemCategoriesDto } from "../shared/dtos/admin/openOrderItemCategriesDto";
import { ResolveFn } from "@angular/router";
import { OrderService } from "../shared/services/admin/order.service";

 export const OpenOrderItemCategoriesResolver: ResolveFn<IOpenOrderItemCategoriesDto[] | undefined> = (
    ) => {
    
    return inject(OrderService).getOpenOrderItemCategoriesDto();
  };