import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { ICVReferredDto } from "src/app/_dtos/admin/cvReferredDto";
import { CVRefParams } from "src/app/_models/params/Admin/cvRefParams";
import { CvrefService } from "src/app/_services/hr/cvref.service";

export const CVsReferredResolver: ResolveFn<ICVReferredDto[] | undefined | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('orderid');
    if(id===null || id==='' || +id === 0) return of(null);
    var refparams = new CVRefParams();
    refparams.orderId=+id;
    inject(CvrefService).setCVRefParams(refparams);
    return inject(CvrefService).referredCVs(false);
  };