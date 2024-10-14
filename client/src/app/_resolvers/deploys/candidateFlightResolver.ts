import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { CandidateFlightGrp, ICandidateFlightGrp } from "src/app/_models/process/candidateFlightGrp";
import { DeployService } from "src/app/_services/deploy.service";

export const CandidateFlightResolver: ResolveFn<ICandidateFlightGrp | null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    if (id==="0") return of(new CandidateFlightGrp());
    return inject(DeployService).getCandidateFlightFromId(+id!);
  };