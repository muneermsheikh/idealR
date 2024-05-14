import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { ISelectionStatus } from "../shared/models/admin/selectionStatus";
import { SelectionService } from "../shared/services/hr/selection.service";


export const SelectionStatusResolver: ResolveFn<ISelectionStatus[]> = (
    ) => {
          return inject(SelectionService).getSelectionStatus();
    };