import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { IVisaBriefDto } from 'src/app/_dtos/admin/visaBriefDto';
import { PaginatedResult } from 'src/app/_models/pagination';
import { visaParams } from 'src/app/_models/params/Admin/visaParams';
import { VisaService } from 'src/app/_services/admin/visa.service';

export const visaListPagedResolver: ResolveFn<PaginatedResult<IVisaBriefDto[] | null>> = 
  (route: ActivatedRouteSnapshot) => {
  
    var vParams = new visaParams();
    var id = route.paramMap.get('id');
    if(id !== null && id !== '' && id !== '0') vParams.id = +id;

    return inject(VisaService).getPagedVisasBrief(vParams);
};
