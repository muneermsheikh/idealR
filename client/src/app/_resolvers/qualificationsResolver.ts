import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IPagination } from "../shared/models/pagination";
import { IQualification } from "../shared/models/hr/qualification";
import { MastersService } from "../shared/services/masters.service";

export const QualificationsResolver: ResolveFn<IPagination<IQualification[]> | undefined | null> = (
 ) => {
       return inject(MastersService).getQualifications(false);
 };