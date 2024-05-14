import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IPagination } from "../shared/models/pagination";
import { ISelPendingDto } from "../shared/dtos/admin/selPendingDto";
import { SelectionService } from "../shared/services/hr/selection.service";

 
export const PendingSelectionsResolver: ResolveFn<IPagination<ISelPendingDto[]> | null | undefined> = (
  ) => {
    return inject(SelectionService).getPendingSelections(false);
  };