import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ISelPendingDto } from "src/app/_dtos/admin/selPendingDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { CandidateFlightParams } from "src/app/_models/params/process/CandidateFlightParams";
import { deployParams } from "src/app/_models/params/process/deployParams";
import { DeployService } from "src/app/_services/deploy.service";


export const CandidateFlightsPagedResolver: ResolveFn<PaginatedResult<ISelPendingDto[]>|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var dParams = new CandidateFlightParams();
    return inject(DeployService).getCandidateFlightHeadersPagedList(dParams);
  };
