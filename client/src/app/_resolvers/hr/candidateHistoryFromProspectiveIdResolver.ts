import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IUserHistory } from "../../shared/models/admin/userHistory";
import { userHistoryParams } from "../../shared/params/admin/userHistoryParams";
import { CandidateHistoryService } from "../../shared/services/candidate-history.service";
 
export const CandidateHistoryFromProspectiveIdResolver: ResolveFn<IUserHistory|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    var hParams = new userHistoryParams();
        hParams.personType="prospective";
        hParams.personId = +id;
    return inject(CandidateHistoryService).getHistory(hParams);
  };