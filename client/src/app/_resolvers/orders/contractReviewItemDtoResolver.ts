import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IContractReviewItemDto } from "src/app/_dtos/orders/contractReviewItemDto";
import { ContractReviewService } from "src/app/_services/admin/contract-review.service";


 export const ContractReviewItemDtoResolver: ResolveFn<IContractReviewItemDto|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(ContractReviewService).getContractReviewItem (+id);
  };