import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../../_models/user';
import { ICategoryRefDto } from '../../_dtos/admin/categoryRefDto';
import { Pagination } from '../../_models/pagination';
import { HttpClient } from '@angular/common/http';
import { IUserHistoryReturnDto } from '../../_dtos/admin/userHistoryReturnDto';
import { GetHttpParamsForCallRecord, getPaginatedResult} from '../paginationHelper';
import { IUserHistoryBriefDto } from '../../_dtos/admin/useHistoryBriefDto';
import { IContactResult } from 'src/app/_models/admin/contactResult';
import { CallRecordParams, ICallRecordParams } from 'src/app/_models/params/callRecordParams';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { ICallRecordDto } from 'src/app/_dtos/admin/callRecordDto';

@Injectable({
  providedIn: 'root'
})
export class UserHistoryService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  histParams = new CallRecordParams();
  categoryRefDtos: ICategoryRefDto[]=[];
  categoryRefDto?: ICategoryRefDto;

  cache = new Map();

  pagination: Pagination | undefined;   // IPagination<IUserHistory>;

  
  contactResults: IContactResult[] = [{status: "Not Reachable"}, {status: "Wrong Number"}, 
    {status: "Declined-Family Issues"}, {status: "Declined-Low Remuneration"}, 
    {status: "Declined-Other Reasons"}, {status: "Will Call later"}, 
    {status: "Not interested for overseas"}, {status: "Declined-Service Charges"}
  ]
  constructor(private http: HttpClient, private toastr: ToastrService) { }

  getContactResults() {return this.contactResults};

  setParams(params:ICallRecordParams) {
    this.histParams = params;
  }
  
  getParams() {
    return this.histParams;
  }

  getCategoryRefDetails() {
    
    return this.http.get<ICategoryRefDto[]>(this.apiUrl + 'categoryrefdetails')
      .pipe(map(response => {
        //this.cache.set(Object.values(this.categoryRefParams).join('-'), response);
        this.categoryRefDtos=response;
        return response;
      }))
  }

  getCandidateHistory(id: number) {
    return this.http.get<ICallRecord>(this.apiUrl + 'CallRecord/bycandidateid/'+id);
  }

  getOrAddCallRecord(userParams: ICallRecordParams) {
    var params = GetHttpParamsForCallRecord(userParams);

    return this.http.get<ICallRecord>(this.apiUrl + 'CallRecord/getOrAddHistoryWithItems', {observe: 'response', params});
  }
  
  getHistoryByParams(hParams: ICallRecordParams) {
    
    let params = GetHttpParamsForCallRecord(hParams);
  
    return this.http.get<ICallRecordDto>(this.apiUrl + 'CallRecord/dto', {observe: 'response', params} ) ;
  }

  getUserHistoryPaged(hParams: ICallRecordParams) {

    const response = this.cache.get(Object.values(hParams).join('-'));
    if(response) return of(response);

    let params = GetHttpParamsForCallRecord(hParams);
        
    return getPaginatedResult<IUserHistoryBriefDto[]>(this.apiUrl + 
        'CallRecord/pagedlist', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(hParams).join('-'), response);
        return response;
      })
    )
    
  }
  
  deleteUserHistoryById(userHistoryId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'CallRecord/deletehist/' + userHistoryId);
  }

  updateUserHistory(model: any) {
    return this.http.put<IUserHistoryReturnDto>(this.apiUrl + 'UserHistory', model);
  }

  composeEmailMessageOfConsent() {

  }
}
