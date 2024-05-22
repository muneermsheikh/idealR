import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IUserHistoryDto } from "src/app/_dtos/admin/userHistoryDto";
import { UserHistoryParams } from "src/app/_models/params/userHistoryParams";
import { CandidateHistoryService } from "src/app/_services/user-history.service";
 
export const CandidateHistoryFromProspectiveIdResolver: ResolveFn<IUserHistoryDto|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    var hParams = new UserHistoryParams();
        hParams.personType="prospective";
        hParams.personId = +id;

    return inject(CandidateHistoryService).getHistory(hParams);

  };