import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { ICOA } from "src/app/_models/finance/coa";
import { COAService } from "src/app/_services/finance/coa.service";

export const COAListResolver: ResolveFn<ICOA[]|null> = (
  ) => {

  	return inject(COAService).getCoaList();
    
  };