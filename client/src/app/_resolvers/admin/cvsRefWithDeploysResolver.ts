import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { ICVReferredDto } from "src/app/_dtos/admin/cvReferredDto";
import { CvrefService } from "src/app/_services/hr/cvref.service";

 export const CVsRefWithDeploysResolver: ResolveFn<ICVReferredDto | undefined> = (
   route: ActivatedRouteSnapshot,
 ) => {
   var id = route.paramMap.get('cvrefid');
   if (id===null) return of(undefined);
   return inject(CvrefService).getCVRefWithDeploys(+id!);
 };