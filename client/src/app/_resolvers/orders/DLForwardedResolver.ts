import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IOrderForwardCategory } from "src/app/_models/orders/orderForwardCategory";
import { PaginatedResult } from "src/app/_models/pagination";
import { OrderFwdParams } from "src/app/_models/params/orders/orderFwdParams";
import { OrderForwardService } from "src/app/_services/admin/order-forward.service";


 export const DLForwardedResolver: ResolveFn<PaginatedResult<IOrderForwardCategory[] | null>> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    var service = inject(OrderForwardService);
    var params = new OrderFwdParams();
    params.orderId=+id;
    
    return  service.getForwardsBriefPaginated(params);
  };