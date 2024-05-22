import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IUserHistory } from "src/app/_models/admin/userHistory";
import { CandidateHistoryService } from "src/app/_services/user-history.service";

export const CandidateHistoryFromHistoryIdResolver: ResolveFn<IUserHistory|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

      var id = route.paramMap.get('id');
      if (id===null) return of(null);
      return inject(CandidateHistoryService).getCandidateHistoryByHistoryId(+id!);

  };