import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { IPagination } from "../shared/models/pagination";
import { IProspectiveCandidate } from "../shared/models/hr/prospectiveCandidate";
import { prospectiveCandidateParams } from "../shared/params/hr/prospectiveCandidateParams";
import { ProspectiveService } from "../shared/services/hr/prospective.service";

export const ProspectiveCandidatesByCategoryRefResolver: ResolveFn<IPagination<IProspectiveCandidate[]> | undefined | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
        var routeid = route.paramMap.get('categoryRef');    
        var status = route.paramMap.get('status');
        if (routeid === '') return null;
        var pParams = new prospectiveCandidateParams();
        pParams.categoryRef=routeid!;
        pParams.categoryRef=routeid!;
        pParams.status = status!;
        inject(ProspectiveService).setParams(pParams);
        return inject(ProspectiveService).getProspectiveCandidates(false);
  };