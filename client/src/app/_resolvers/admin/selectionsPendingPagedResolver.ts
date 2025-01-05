import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ISelPendingDto } from "src/app/_dtos/admin/selPendingDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { CVRefParams } from "src/app/_models/params/Admin/cvRefParams";
import { CvrefService } from "src/app/_services/hr/cvref.service";


export const SelectionsPendingPagedResolver: ResolveFn<PaginatedResult<ISelPendingDto[]>|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

    var selParams = new CVRefParams();
    selParams.selectionStatus="Pending";
    var service = inject(CvrefService);
    service.setParams(selParams);
    return service.referredCVsPaginated(false);

  };
