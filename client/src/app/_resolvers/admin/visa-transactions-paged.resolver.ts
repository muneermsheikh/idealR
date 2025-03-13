import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { IVisaTransaction } from 'src/app/_models/admin/visaTransaction';
import { PaginatedResult } from 'src/app/_models/pagination';
import { visaParams } from 'src/app/_models/params/Admin/visaParams';
import { VisaService } from 'src/app/_services/admin/visa.service';

export const visaTransactionsPagedResolver: ResolveFn<PaginatedResult<IVisaTransaction[] | null>> = 
  (route: ActivatedRouteSnapshot) => {
  
    var vParams = new visaParams();
    var id = route.paramMap.get('visaId');
    if(id !== null && id !== '' && id !== '0') vParams.id = +id;

    return inject(VisaService).getPagedVisaTransactions(vParams);
};
