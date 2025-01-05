import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IProspectiveBriefDto } from "src/app/_dtos/hr/prospectiveBriefDto";
import { prospectiveCandidateParams } from "src/app/_models/params/hr/prospectiveCandidateParams";
import { ProspectiveService } from "src/app/_services/hr/prospective.service";

 export const ProspectiveListResolver: ResolveFn<IProspectiveBriefDto[]|null | undefined> = (
  route: ActivatedRouteSnapshot) => {

    var orderno = route.paramMap.get('orderno') ?? '0';

    if (orderno === '0' || orderno === '') return of(null);

    return inject(ProspectiveService).getProspectivesList(orderno, "active");
  };