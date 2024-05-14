import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IUserHistoryDto } from "src/app/_dtos/admin/userHistoryDto";

import { UserHistoryParams } from "src/app/_models/params/userHistoryParams";
import { CandidateHistoryService } from "src/app/_services/candidate-history.service";

export const CandidateHistoryResolver: ResolveFn<IUserHistoryDto | null | undefined> = (
    route: ActivatedRouteSnapshot,
  ) => 
    {
        
        var hParam = new UserHistoryParams();

        var routeid = route.paramMap.get('id');

        if ( routeid!=='' && routeid !== null && routeid !== '0') {
            hParam.personType='prospective';
            hParam.id = +routeid;
        } else 
        {
            if (routeid !== '' && routeid !== '0' && routeid !== null) {
                hParam.personType='prospective';
                hParam.id=+routeid;
            } else {
                var officialId = route.paramMap.get('officialId');
                if(officialId !== null) {
                    hParam.personType='official';
                    hParam.personId = +officialId;
                } else {
                    return of(null);
                }
            }
        }
    
        return inject(CandidateHistoryService).getHistory(hParam);
    };