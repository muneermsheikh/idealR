import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IFinanceVoucher } from "src/app/_models/finance/voucher";
import { VouchersService } from "src/app/_services/finance/vouchers.service";

export const FinanceVouchersResolver: ResolveFn<IFinanceVoucher[] | null> = () => 
	{
		return inject(VouchersService).getVouchers(false);
  	};