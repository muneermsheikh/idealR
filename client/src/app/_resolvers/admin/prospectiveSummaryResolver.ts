import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IProspectiveSummaryDto } from "src/app/_dtos/hr/propectiveSummaryDto";
import { ProspectiveService } from "src/app/_services/hr/prospective.service";

 export const ProspectiveSummaryResolver: ResolveFn<IProspectiveSummaryDto[]|null> = (
  ) => {
    return inject(ProspectiveService).getProspectiveSummary(false);
  };