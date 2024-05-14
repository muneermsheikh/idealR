import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IProspectiveSummaryDto } from "../shared/dtos/hr/propectiveSummaryDto";
import { ProspectiveService } from "../shared/services/hr/prospective.service";

export const ProspectiveCandidatesByCategoryRefResolver: ResolveFn<IProspectiveSummaryDto[] | undefined | null> = (
  ) => {
        return inject(ProspectiveService).getProspectiveSummary(false);
  };