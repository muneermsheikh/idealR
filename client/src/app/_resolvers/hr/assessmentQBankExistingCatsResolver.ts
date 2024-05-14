import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IProfession } from "../../shared/models/masters/profession";
import { HrService } from "../../shared/services/hr/hr.service";

 export const AssessmentQBankExistingCatsResolver: ResolveFn<IProfession[]|null> = (
  ) => {
    return inject(HrService).getExistingProfFromQBank();
  };