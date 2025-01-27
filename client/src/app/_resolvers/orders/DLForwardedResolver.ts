import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IOrderForwardCategory } from "src/app/_models/orders/orderForwardCategory";
import { IOrderForwardToAgent } from "src/app/_models/orders/orderForwardToAgent";
import { PaginatedResult } from "src/app/_models/pagination";
import { OrderFwdParams } from "src/app/_models/params/orders/orderFwdParams";
import { OrderForwardService } from "src/app/_services/admin/order-forward.service";


 export const DLForwardedResolver: ResolveFn<PaginatedResult<IOrderForwardToAgent[] | null>> = (    
  ) => {   
    var params = new OrderFwdParams();
    
    return inject(OrderForwardService).getForwardsBriefPaginated(params);
  };