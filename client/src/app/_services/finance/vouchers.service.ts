import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { IApiReturnDto } from 'src/app/_dtos/admin/apiReturnDto';
import { IVoucher } from 'src/app/_models/finance/voucher';
import { Pagination } from 'src/app/_models/pagination';
import { transactionParams } from 'src/app/_models/params/finance/transactionParams';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult, getPaginationHeaders } from '../paginationHelper';
import { IStatementofAccountDto } from 'src/app/_dtos/finance/statementOfAccountDto';

@Injectable({
  providedIn: 'root'
})
export class VouchersService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  sParams = new transactionParams();
  vouchers: IVoucher[]=[];
  pagination: Pagination | undefined;
  count = 0;
  cache = new Map();
  
  constructor(private http: HttpClient) { }
  
  getVouchers(oParams: transactionParams) {

    const response = this.cache.get(Object.values(oParams).join('-'));
    if(response) return of(response);

    let params = getPaginationHeaders(oParams.pageNumber, oParams.pageSize);

    if (oParams.cOAId !== 0 )  params = params.append('coaId', oParams.cOAId.toString());
    if (oParams.voucherNo !== 0 )  params = params.append('voucherNo', oParams.voucherNo.toString());
    if (oParams.voucherDated  !== '' )  params = params.append('vocherDated', oParams.voucherDated);
    if (oParams.cVRefId !== 0 )  params = params.append('cVRefId', oParams.cVRefId.toString());
    if (oParams.amount !== 0 )  params = params.append('amount', oParams.amount.toString());
    if (oParams.accountName === '') params = params.append('accountName', oParams.accountName);
    if (oParams.search) params = params.append('search', oParams.search);
    params = params.append('sort', this.sParams.sort);
    params = params.append('pageNumber', this.sParams.pageNumber.toString());
    params = params.append('pageSize', this.sParams.pageSize.toString());

    
    return getPaginatedResult<IVoucher[]>(this.apiUrl + 'finance/voucherspagedlist', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(oParams).join('-'), response);
        return response;
      })
    )
   
  }

  getVoucherFromId( id: number): any {
    var voucher: IVoucher;
    if(id===0) return of(undefined);
    
    return this.http.get<IVoucher|undefined>(this.apiUrl + 'finance/vouchers/' + id);
  }

  deleteVoucher(id: number)
  {
    return this.http.delete<boolean>(this.apiUrl + 'finance/deletevoucher/' + id);
  }

  insertVoucherWithUploads(model: FormData) {
    return this.http.post<string>(this.apiUrl+ 'finance/newvoucherwithattachment', model, {
      reportProgress: true,
      observe: 'events'
    });
  }

  public updateWithFiles(formData: FormData) {
    return this.http.put(this.apiUrl + 'finance/updateVoucherwithattachment', formData, {
      reportProgress: true,
      observe: 'events',
    });
  }

  getNextVoucherNo() {
      return this.http.get<number>(this.apiUrl + 'finance/nextvoucherno');
  }
  

  getStatementOfAccount(accountid: number, fromdate: string, uptodate: string) {

    return this.http.get<IStatementofAccountDto>(this.apiUrl + 'finance/soa/' + accountid + '/' + fromdate + '/' + uptodate);
  
  }  

  deleteVoucherFromCache(id: number) {
    this.cache.delete(id);
    this.pagination!.totalItems--;
  }

  
  setParams(params: transactionParams) {
    this.sParams = params;
  }
  
  getParams() {
    return this.sParams;
  }
}
