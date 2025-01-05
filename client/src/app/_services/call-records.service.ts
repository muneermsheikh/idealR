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
import { ICallRecordResult } from '../_dtos/admin/callRecordResult';
import { CallRecordStatusReturnDto } from '../_dtos/admin/callRecordStatusReturnDto';
import { ICallRecordItemToAddDto } from '../_dtos/admin/callRecordItemToAddDto';
import { ICallRecordItemAddedReturnValueDto } from '../_dtos/admin/callRecordItemAddedReturnValueDto';
import { IUserHistoryBriefDto } from '../_dtos/admin/useHistoryBriefDto';

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
  
  callRecordStatus: ICallRecordResult[] = [{status: "wrong number"}, {status: "Not Responding"}, {status: "Will Revert later"},
    {status: "Declined-Family issues"}, {status: "Declined for overseas"}, {status: "Declined-Low remuneration"},
    {status: "Declined - SC Not agreed"}, {status: "Interested - to negotiate remuneration"},
    {status: "Interested, and keen"}, {status: "Interested, but doubtful"}, {status: "Declined - other reasons"}]
    
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

  getCallRecordReport(hParams:CallRecordParams) {

    let params = GetHttpParamsForCallRecord(hParams);
    return this.http.get<IUserHistoryBriefDto[]>(this.apiUrl + 'CallRecord/report', {params} )
  }

  getCallRecordWithItems(personType: string, personid: string) {
    
    return this.http.get<ICallRecord>(this.apiUrl + 
        'CallRecord/callRecordWithItems/' +  personType  + '/' + personid ) ;
  }

  getCallRecordStatus () {
      return this.callRecordStatus
  }

  updateHistoryItem(item: ICallRecordItem) {
    return this.http.post<ICallRecordItem>(this.apiUrl + 'userHistory/userhistoryitem', item);
  }
  
  insertNewCallRecordItem(item: ICallRecordItem) {
    return this.http.post<CallRecordStatusReturnDto>(this.apiUrl + 'CallRecord/CallRecorditem', item);
  }
  setParams(params: CallRecordParams) {
    this.hParams = params;
  }
  
  getParams() {
    return this.hParams;
  }

  updateCallRecord(model: ICallRecord) {
    return this.http.put<CallRecordStatusReturnDto>(this.apiUrl + 'CallRecord', model);
  }

  updateCallRecordObject(model: any) {
    return this.http.put<ICallRecord>(this.apiUrl + 'CallRecord/UpdateNewItem', model);
  }

  updateCallRecordItem(model: ICallRecordItemToAddDto) {
    console.log('updatecallrecorditem, model');
    return this.http.put<ICallRecordItemAddedReturnValueDto>(this.apiUrl + 'CallRecord/InsertCallRecordItem', model);
  }

  deleteHistory(historyid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'userHistory/' + historyid);
  }

  deleteHistoryItem(itemid: number) {
    return this.http.delete(this.apiUrl + 'userHistory/historyItemId/' + itemid);
  }

  getCallRecordFromPhoneNo(phoneno: string) {
    return this.http.get<ICallRecord>(this.apiUrl + 'callRecord/callRecordFromPhoneNo/' + phoneno);
  }

  getCallRecordsOfACandidate(persontype: string, personid: string)
  {
    return this.http.get<ICallRecordDto[]>(this.apiUrl + 'callRecord/CallRecordSummary/' + persontype + '/' + personid);
  }
     
}
