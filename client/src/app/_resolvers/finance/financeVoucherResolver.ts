import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IVoucher, Voucher } from "src/app/_models/finance/voucher";
import { VouchersService } from "src/app/_services/finance/vouchers.service";

export const FinanceVoucherResolver: ResolveFn<IVoucher|any> = (
	route: ActivatedRouteSnapshot,
  ) => {
	var id = route.paramMap.get('id');
	if (id==='' || id=== null || id===undefined) return of(null);
	if (id==='0') return of(new Voucher());
	
	return inject(VouchersService).getVoucherFromId(+id);
  };
  