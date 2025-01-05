import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ISelPendingDto } from "src/app/_dtos/admin/selPendingDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { candidateParams } from "src/app/_models/params/hr/candidateParams";
import { CvrefService } from "src/app/_services/hr/cvref.service";


export const CvReferredPagedResolver: ResolveFn<PaginatedResult<ISelPendingDto[]>|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

      return inject(CvrefService).referredCVsPaginated(false);

  };
