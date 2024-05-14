import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IFinanceVoucher } from "src/app/shared/models/finance/financeVoucher";
import { IPagination } from "src/app/shared/models/pagination";
import { VouchersService } from "src/app/shared/services/finance/vouchers.service";

export const FinanceVouchersResolver: ResolveFn<IPagination<IFinanceVoucher[]>|null> = () => 
	{
		return inject(VouchersService).getVouchers(false);
  	};