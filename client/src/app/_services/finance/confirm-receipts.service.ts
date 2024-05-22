import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { IPendingDebitApprovalDto, PendingDebitApprovalDto } from 'src/app/_dtos/finance/pendingDebitApprovalDto';
import { IUpdatePaymentConfirmationDto } from 'src/app/_dtos/finance/updatePaymentConfirmationDto';
import { ParamsCOA } from 'src/app/_models/params/finance/paramsCOA';
import { getPaginatedResult, getPaginationHeaders } from '../paginationHelper';
import { Pagination } from 'src/app/_models/pagination';
import { ICOA } from 'src/app/_models/finance/coa';
import { IVoucher } from 'src/app/_models/finance/voucher';

@Injectable({
  providedIn: 'root'
})
export class ConfirmReceiptsService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  cache = new Map();
  pagination: Pagination | undefined;

  constructor(private http: HttpClient) { }


  getDebitApprovalsPending(oParams: ParamsCOA) {
      
    const response = this.cache.get(Object.values(oParams).join('-'));
    if(response) return of(response);
    
    let params = this.populateCOAParams(oParams);
        
    return getPaginatedResult<PendingDebitApprovalDto[]>(this.apiUrl + 'finance/debitapprovals', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )
    
  }

  updatePaymentReceipts(confirmations: IUpdatePaymentConfirmationDto[]) {
    return this.http.put<boolean>(this.apiUrl + 'finance/updatePaymentConfirmation', confirmations);
  }

  updateVoucher(voucher: IVoucher) {
      return this.http.put<boolean>(this.apiUrl + 'finance/voucher', voucher);
  }

  deleteVoucher(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'finance/deletevoucher/' + id);
  }

  addNewVoucher(voucher: IVoucher) {
    return this.http.post<IVoucher>(this.apiUrl + 'finance/newvocher', voucher);
  }
  
  populateCOAParams(oParams: ParamsCOA) {
    let params = getPaginationHeaders(oParams.pageNumber, oParams.pageSize);
  
      if (oParams.search) params = params.append('search', oParams.search);
      if (oParams.accountName !== '' )  params = params.append('coaId', oParams.accountName);
      if (oParams.sort !== '') params = params.append('sort', oParams.sort);
      if (oParams.accountId !== 0) params = params.append('accountId', oParams.accountId.toString());
      if (oParams.accountType !== '') params = params.append('accountType', oParams.accountType);
      if (oParams.accountClass !== '') params = params.append('accountClass', oParams.accountClass);
      
    return params;
  }

    
  deleteFromCache(id: number) {
    this.cache.delete(id);
    this.pagination!.totalItems--;
  }
}
