import { Injectable, inject } from "@angular/core";
import { Resolve, ResolveFn } from "@angular/router";
import { Observable } from "rxjs";

import { IOrderBriefDto, OrderBriefDto } from "../shared/dtos/admin/orderBriefDto";
import { IPagination } from "../shared/models/pagination";
import { OrderService } from "../shared/services/admin/order.service";

 export const OrdersBriefResolver: ResolveFn<IPagination<IOrderBriefDto[]>> = (
  ) => {
    return inject(OrderService).getOrdersBrief(false);
  };