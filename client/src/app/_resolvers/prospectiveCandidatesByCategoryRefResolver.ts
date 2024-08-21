import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { IProspectiveCandidate } from "../_models/hr/prospectiveCandidate";
import { prospectiveCandidateParams } from "../_models/params/hr/prospectiveCandidateParams";
import { ProspectiveService } from "../_services/hr/prospective.service";
import { Pagination } from "../_models/pagination";

export const ProspectiveCandidatesByCategoryRefResolver: ResolveFn<IProspectiveCandidate[] | undefined | null> = (
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
        return inject(ProspectiveService).getProspectiveCandidates(pParams);
  };