import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { ISelDecisionDto } from "src/app/_dtos/admin/selDecisionDto";
import { PaginatedResult } from "src/app/_models/pagination";
import { SelDecisionParams } from "src/app/_models/params/Admin/selDecisionParams";
import { SelectionService } from "src/app/_services/hr/selection.service";


export const SelectionsPagedResolver: ResolveFn<PaginatedResult<ISelDecisionDto[]>|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    
    var id= route.paramMap.get('id');
    
    var services = inject(SelectionService);

    if(id !=='' && id !== '0') {
      var params = new SelDecisionParams();
      params.orderId=+id!;
      services.setParams(params);
    }

    return services.getSelectionRecords(false);
  };
