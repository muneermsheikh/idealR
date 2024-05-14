import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IPendingDebitApprovalDto } from "../shared/dtos/finance/pendingDebitApprovalDto";
import { ConfirmReceiptsService } from "../shared/services/finance/confirm-receipts.service";

export const ReceiptsPendingConfirmtionResolver: ResolveFn<IPendingDebitApprovalDto[] | undefined | null> = (
  ) => {
        return inject(ConfirmReceiptsService).getPendingConfirmations();
  };