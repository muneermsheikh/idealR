import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { UserHistoryParams } from '../_models/params/userHistoryParams';
import { Pagination } from '../_models/pagination';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { IUserHistoryDto } from '../_dtos/admin/userHistoryDto';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IUserHistory } from '../_models/admin/userHistory';
import { IUserHistoryItem } from '../_models/admin/userHistoryItem';

@Injectable({
  providedIn: 'root'
})
export class CallRecordsService {
  
  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  hParams: UserHistoryParams | undefined; // = new userHistoryParams();
  pagination: Pagination | undefined; //<IUserHistoryDto[]>;
  cache = new Map();
  
  constructor(private http: HttpClient) { }

  getHistories(hParams: UserHistoryParams) {      //: Observable<IPagination<IUserHistoryDto[]>>

    const response = this.cache.get(Object.values(hParams).join('-'));
    if(response) return of(response);

    let params = getPaginationHeaders(hParams.pageNumber, hParams.pageSize);

    if(hParams.userName !== '') params = params.append('userName', hParams.userName);
    if(hParams.applicationNo !== 0) params = params.append('applicationNo', hParams.applicationNo.toString());
    if(hParams.emailId !== '') params = params.append('emailId', hParams.emailId);
    if(hParams.applicationNo !== undefined) params = params.append('applicationNo', hParams.applicationNo?.toString());
    if(hParams.mobileNo !== '') params = params.append('mobileNo', hParams.mobileNo);
    if(hParams.personName !== '') params = params.append('personName', hParams.personName);
    if(hParams.status !== '') params = params.append('status', hParams.status);
    
    return getPaginatedResult<IUserHistoryDto[]>(this.apiUrl + 'userHistory/paginated', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(hParams).join('-'), response);
          return response;
        })
      )
    }

  getHistoryWithItems(historyId: number) {
    return this.http.get<IUserHistory>(this.apiUrl + 'userHistory/historywithitems/' + historyId);
  }

  updateHistoryItem(item: IUserHistoryItem) {
    return this.http.post<IUserHistoryItem>(this.apiUrl + 'userHistory/userhistoryitem', item);
  }
  
  getHttpParams(){

    if(this.hParams == undefined) return null;

    let params = new HttpParams();

    if(this.hParams.userName !== '') params = params.append('userName', this.hParams.userName);
    if(this.hParams.applicationNo !== 0) params = params.append('applicationNo', this.hParams.applicationNo.toString());
    if(this.hParams.emailId !== '') params = params.append('emailId', this.hParams.emailId);
    if(this.hParams.applicationNo !== undefined) params = params.append('applicationNo', this.hParams.applicationNo?.toString());
    if(this.hParams.mobileNo !== '') params = params.append('mobileNo', this.hParams.mobileNo);
    if(this.hParams.personName !== '') params = params.append('personName', this.hParams.personName);
    if(this.hParams.status !== '') params = params.append('status', this.hParams.status);
   
    params = params.append('pageIndex', this.hParams.pageNumber.toString());
    params = params.append('pageSize', this.hParams.pageSize.toString());

    return params;
  }

  setParams(params: UserHistoryParams) {
    this.hParams = params;
  }
  
  getParams() {
    return this.hParams;
  }

  updateHistory(model: any) {
    return this.http.put(this.apiUrl + 'userHistory', model);
  }

  deleteHistory(historyid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'userHistory/' + historyid);
  }

  deleteHistoryItem(itemid: number) {
    return this.http.delete(this.apiUrl + 'userHistory/historyItemId/' + itemid);
  }

  
  /* getOrcreateNewHistory(): Observable<any> {
    var hparams = this.getHttpParams();
    return this.http.get<IUserHistoryDto>(this.apiUrl + 'userHistory/dto', {hparams});
  }
  */

}
