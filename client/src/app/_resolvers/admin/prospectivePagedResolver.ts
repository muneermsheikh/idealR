import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IProspectiveSummaryDto } from "src/app/_dtos/hr/propectiveSummaryDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { prospectiveCandidateParams } from "src/app/_models/params/hr/prospectiveCandidateParams";
import { ProspectiveService } from "src/app/_services/hr/prospective.service";

 export const ProspectivePagedResolver: ResolveFn<PaginatedResult<IProspectiveSummaryDto[]|null>> = (
  ) => {
    var params = new prospectiveCandidateParams();
    return inject(ProspectiveService).getProspectivesPaged(params);
  };