import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { ICOA } from "src/app/_models/finance/coa";
import { COAService } from "src/app/_services/finance/coa.service";


export const COAsPagedResolver: ResolveFn<ICOA[] | null> = (
  ) => {
	
	    return inject(COAService).getCoas(false);
      
  };