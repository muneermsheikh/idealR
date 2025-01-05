import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { ICallRecord } from "src/app/_models/admin/callRecord";
import { CallRecordParams } from "src/app/_models/params/callRecordParams";
import { UserHistoryService } from "src/app/_services/admin/user-history.service";
import { CallRecordsService } from "src/app/_services/call-records.service";


export const CandidateHistoryFromHistoryIdResolver: ResolveFn<ICallRecord|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

      var id = route.paramMap.get('id');
      if (id===null) return of(null);
      var callRecParams = new CallRecordParams();
      callRecParams.id=+id;
      return inject(CallRecordsService).getHistories(callRecParams);

  };