import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IChecklistHRDto } from "src/app/_dtos/hr/checklistHRDto";
import { ChecklistService } from "src/app/_services/hr/checklist.service";

export const ChecklistResolver: ResolveFn<IChecklistHRDto|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var candidateid = route.paramMap.get('candidateid');
    var orderitemid = route.paramMap.get('orderitemid');
    if (candidateid===null || candidateid === '' || orderitemid === null || orderitemid === '') return of(null);
    return inject(ChecklistService).getChecklist(+candidateid, +orderitemid);
  };