import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { ICOA } from "src/app/shared/models/finance/coa";
import { IPagination } from "src/app/shared/models/pagination";
import { COAService } from "src/app/shared/services/finance/coa.service";


export const COAsPagedResolver: ResolveFn<IPagination<ICOA[]|null>> = (
  ) => {
	
	return inject(COAService).getCoas(false);
  };