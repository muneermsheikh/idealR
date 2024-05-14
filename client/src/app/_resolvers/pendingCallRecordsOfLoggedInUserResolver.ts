import { Inject, inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IPagination } from "../shared/models/pagination";
import { IUserHistoryDto } from "../shared/dtos/admin/userHistoryDto";
import { userHistoryParams } from "../shared/params/admin/userHistoryParams";
import { take } from "rxjs/operators";
import { IUser } from "../shared/models/admin/user";
import { AccountsService } from "../shared/services/accounts.service";
import { CallRecordsService } from "../shared/services/call-records.service";

export const OrderBriefDtoResolver: ResolveFn<IPagination<IUserHistoryDto[]> | null | undefined> = (
) => {
  var hParams = new userHistoryParams();
  
  Inject(AccountsService).currentUser$.pipe(take(1)).subscribe((x: IUser) => {
    hParams.userName = x.username;
    hParams.status="active";
  })
  if(hParams.userName==="") return undefined;
  
  inject(CallRecordsService).setParams(hParams);
  inject(CallRecordsService).getHistories(false);
};