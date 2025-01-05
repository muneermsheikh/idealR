import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ISelectionStatusDto } from "src/app/_dtos/admin/selectionStatusDto";
import { CvrefService } from "src/app/_services/hr/cvref.service";

export const RejectionStatusResolver: ResolveFn<ISelectionStatusDto[]|null> = (
    route: ActivatedRouteSnapshot,
  ) => {

      return inject(CvrefService).selectionStatuses;

  };
