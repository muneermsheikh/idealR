import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IFinanceVoucher } from "../../shared/models/finance/financeVoucher";
import { VouchersService } from "src/app/shared/services/finance/vouchers.service";

export const FinanceVoucherResolver: ResolveFn<IFinanceVoucher|any> = (
	route: ActivatedRouteSnapshot,
  ) => {
	var id = route.paramMap.get('id');
	if (id==='' || id=== null || id===undefined) return of(null);
	return inject(VouchersService).getVoucherFromId(+id);
  };
  