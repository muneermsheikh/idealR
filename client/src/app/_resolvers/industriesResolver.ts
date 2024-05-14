import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IIndustryType } from "../shared/models/admin/industryType";
import { MastersService } from "../shared/services/masters.service";

export const IndustriesResolver: ResolveFn<IIndustryType[]> = (
  ) => {
    return inject(MastersService).getIndustries();
  };