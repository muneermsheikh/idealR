import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { Pagination } from '../_models/pagination';
import { GetHttpParamsForCallRecord, getPaginatedResult } from './paginationHelper';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CallRecordParams } from '../_models/params/callRecordParams';
import { ICallRecordItem } from '../_models/admin/callRecordItem';
import { ICallRecordDto } from '../_dtos/admin/callRecordDto';
import { ICallRecord } from '../_models/admin/callRecord';

@Injectable({
  providedIn: 'root'
})

export class CallRecordsService {
  
  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  hParams: CallRecordParams | undefined; // = new userHistoryParams();
  pagination: Pagination | undefined; //<IUserHistoryDto[]>;
  cache = new Map();
  

  constructor(private http: HttpClient) { }



  getHistories(hParams: CallRecordParams) {      //: Observable<IPagination<IUserHistoryDto[]>>

        const response = this.cache.get(Object.values(hParams).join('-'));
        if(response) return of(response);

        let params = GetHttpParamsForCallRecord(hParams);
        
        return getPaginatedResult<ICallRecordDto[]>
          (this.apiUrl + 'userHistory/pagedlist', params, this.http).pipe(
            map(response => {
              this.cache.set(Object.values(hParams).join('-'), response);
              return response;
            })
          )
    }

  getCallRecordWithItems(personid: string, callRecordId: number) {
    
    return this.http.get<ICallRecord>(this.apiUrl + 'CallRecord/callRecordWithItems/' + callRecordId + '/' + personid);
  }

  updateHistoryItem(item: ICallRecordItem) {
    return this.http.post<ICallRecordItem>(this.apiUrl + 'userHistory/userhistoryitem', item);
  }
  
 
  setParams(params: CallRecordParams) {
    this.hParams = params;
  }
  
  getParams() {
    return this.hParams;
  }

  updateCallRecord(model: any) {
    return this.http.put<ICallRecord>(this.apiUrl + 'CallRecord', model);
  }

  updateCallRecordObject(model: any) {
    return this.http.put<ICallRecord>(this.apiUrl + 'CallRecord/UpdateNewItem', model);
  }

  deleteHistory(historyid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'userHistory/' + historyid);
  }

  deleteHistoryItem(itemid: number) {
    return this.http.delete(this.apiUrl + 'userHistory/historyItemId/' + itemid);
  }

  
  
}
