import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { IVisaTransaction } from 'src/app/_models/admin/visaTransaction';
import { PaginatedResult } from 'src/app/_models/pagination';
import { visaParams } from 'src/app/_models/params/Admin/visaParams';
import { VisaService } from 'src/app/_services/admin/visa.service';

export const visaTransactionByCVRefIdResolver: ResolveFn<IVisaTransaction | null> = 
  (route: ActivatedRouteSnapshot) => {
  
    var vParams = new visaParams();
    var id = route.paramMap.get('cvRefId');
    if(id !== null && id !== '' && id !== '0') vParams.cvRefId = +id;

    return inject(VisaService).getPagedVisaTransactions(vParams);
};
