import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IQualification } from "../_models/hr/qualification";
import { QualificationService } from "../_services/admin/qualification.service";

export const QualificationListResolver: ResolveFn<IQualification[] | undefined | null> = (
  ) => {
        return inject(QualificationService).getQualificationList();
  };