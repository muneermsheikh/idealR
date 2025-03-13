import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ISelPendingDto } from "src/app/_dtos/admin/selPendingDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { CVRefParams } from "src/app/_models/params/Admin/cvRefParams";
import { candidateParams } from "src/app/_models/params/hr/candidateParams";
import { CvrefService } from "src/app/_services/hr/cvref.service";


export const CvReferredPagedResolver: ResolveFn<PaginatedResult<ISelPendingDto[]>|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

        var refparams = new CVRefParams();
        
        var id = route.paramMap.get('orderid');
        if(id !==null && id !=='' && id !== '0') refparams.orderId=+id!;   
        
        inject(CvrefService).setParams(refparams);
          
        return inject(CvrefService).referredCVsPaginated(false);

  };
