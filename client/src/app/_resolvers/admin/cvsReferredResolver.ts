import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IPagination } from "../../shared/models/pagination";
import { ICVReferredDto } from "../../shared/dtos/admin/cvReferredDto";
import { CVRefParams } from "../../shared/params/admin/cvRefParams";
import { CvrefService } from "../../shared/services/hr/cvref.service";

export const CVsReferredResolver: ResolveFn<IPagination<ICVReferredDto[]> | undefined | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if(id===null || id==='' || +id === 0) return of(null);
    var refparams = new CVRefParams();
    refparams.orderId=+id;
    inject(CvrefService).setCVRefParams(refparams);
    return inject(CvrefService).referredCVs(false);
  };