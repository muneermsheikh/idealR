import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IDLForwardToAgent } from "src/app/_models/admin/orderForwardToAgent";
import { DlForwardService } from "src/app/_services/admin/dl-forward.service";

export const DLForwardsOfAnOrderIdResolver: ResolveFn<IDLForwardToAgent[]|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if (id===null || id === '0') return of(null);
    return inject(DlForwardService).getDLForwardsOfAnOrderId(+id);
  };
