import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IProfession } from "src/app/_models/masters/profession";
import { HrService } from "src/app/_services/hr/hr.service";

 export const AssessmentQBankExistingCatsResolver: ResolveFn<IProfession[]|null> = (
  ) => {
    return inject(HrService).getExistingProfFromQBank();
  };