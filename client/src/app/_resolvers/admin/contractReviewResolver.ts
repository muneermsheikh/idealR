import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IContractReview } from "src/app/_models/admin/contractReview";
import { ContractReviewService } from "src/app/_services/admin/contract-review.service";


 export const ContractReviewResolver: ResolveFn<IContractReview|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(ContractReviewService).getOrGenerateContractReview(+id!);
  };