import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ISelPendingDto } from "src/app/_dtos/admin/selPendingDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { deployParams } from "src/app/_models/params/process/deployParams";
import { DeployService } from "src/app/_services/deploy.service";


export const DeploymentsPagedResolver: ResolveFn<PaginatedResult<ISelPendingDto[]>|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var dParams = new deployParams();
    return inject(DeployService).getDeploymentPagedList(dParams);

  };
