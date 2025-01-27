import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { IProspectiveHeaderDto } from "src/app/_dtos/hr/prospectiveHeaderDto";
import { CvrefService } from "src/app/_services/hr/cvref.service";


export const OrderNosHeaderResolver: ResolveFn<IProspectiveHeaderDto[]|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var status = route.paramMap.get('status') ?? 'pending';

    return inject(CvrefService).getCVReferredOrderNosDto(status);
  };