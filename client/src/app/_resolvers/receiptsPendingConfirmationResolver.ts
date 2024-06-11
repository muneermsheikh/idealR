import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { ConfirmReceiptsService } from "../_services/finance/confirm-receipts.service";
import { IPendingDebitApprovalDto } from "../_dtos/finance/pendingDebitApprovalDto";
import { ParamsCOA } from "../_models/params/finance/paramsCOA";

export const ReceiptsPendingConfirmtionResolver: ResolveFn<IPendingDebitApprovalDto[] | undefined | null> = (
  ) => {
      var oParams = new ParamsCOA();  
      return inject(ConfirmReceiptsService).getDebitApprovalsPending(oParams);
  };