import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { IPendingDebitApprovalDto, PendingDebitApprovalDto } from 'src/app/_dtos/finance/pendingDebitApprovalDto';
import { ParamsCOA } from 'src/app/_models/params/finance/paramsCOA';
import { getPaginatedResult } from '../paginationHelper';
import { Pagination } from 'src/app/_models/pagination';
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
  oParams: ParamsCOA = new ParamsCOA();

  constructor(private http: HttpClient) { }

  pParams: ParamsCOA = new ParamsCOA();

  getDebitApprovalsPending() {
      
    var oParams = this.pParams;

    const response = this.cache.get(Object.values(oParams).join('-'));
    if(response) return of(response);
    
    let params = this.populateCOAParams(oParams);
        
    return getPaginatedResult<PendingDebitApprovalDto[]>(this.apiUrl + 'finance/DrApprovalsPending', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )
    
  }

  updatePaymentReceipts(confirmations: IPendingDebitApprovalDto[]) {
    return this.http.put<boolean>(this.apiUrl + 'finance/updatePaymentConfirmation', confirmations);
  }

  updateVoucher(voucher: IVoucher) {
      return this.http.put<boolean>(this.apiUrl + 'finance/voucher', voucher);
  }

  deleteVoucher(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'finance/deletevoucher/' + id);
  }

  setParams(cParams: ParamsCOA){
    this.oParams = cParams;
  }

  getParams(){
    return this.oParams;
  }

  addNewVoucher(voucher: IVoucher) {
    return this.http.post<IVoucher>(this.apiUrl + 'finance/newvocher', voucher);
  }
  
  populateCOAParams(oParams: ParamsCOA) {
    let params = new HttpParams();  // getPaginationHeaders(oParams.pageNumber, oParams.pageSize);
  
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
