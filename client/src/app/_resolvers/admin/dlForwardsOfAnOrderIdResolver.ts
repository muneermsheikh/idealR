import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IDLForwardToAgent } from "../../shared/models/admin/dlforwardToAgent";
import { DlForwardService } from "../../shared/services/admin/dl-forward.service";

export const DLForwardsOfAnOrderIdResolver: ResolveFn<IDLForwardToAgent[]|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if (id===null || id === '0') return of(null);
    return inject(DlForwardService).getDLForwardsOfAnOrderId(+id);
  };
