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
import { IVoucherEntry } from 'src/app/_models/finance/voucherEntry';

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

  updateVoucherEntries(confirmations: IVoucherEntry[]) {
    return this.http.put<string>(this.apiUrl + 'finance/updateVoucherEntries', confirmations);
  }

  approveDrApprovals(ids: number[]) {
    return this.http.put<boolean>(this.apiUrl + 'finance/updateDrApprovals', ids);
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
      if (oParams.cOAId !== 0) params = params.append('cOAIDId', oParams.cOAId.toString());
      if (oParams.accountType !== '') params = params.append('accountType', oParams.accountType);
      
    return params;
  }

    
  deleteFromCache(id: number) {
    this.cache.delete(id);
    this.pagination!.totalItems--;
  }
}
