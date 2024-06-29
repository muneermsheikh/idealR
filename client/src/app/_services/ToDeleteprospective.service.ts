import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { IProspectiveBriefDto } from '../_dtos/hr/prospectiveBriefDto';
import { Pagination } from '../_models/pagination';
import { prospectiveSummaryParams } from '../_models/params/hr/prospectiveSummaryParams';
import { HttpClient, HttpParams } from '@angular/common/http';
import { GetHttpParamsForCallItemCreate, GetHttpParamsForCallRecord, getPaginatedResult } from './paginationHelper';
import { CallRecordParams, ICallRecordParams } from '../_models/params/callRecordParams';
import { ICallRecord } from '../_models/admin/callRecord';
import { CallRecordItemToCreateDto } from '../_dtos/hr/callRecordItemToCreateDto';

@Injectable({
  providedIn: 'root'
})
export class ToDeleteProspectiveService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  prospectives: IProspectiveBriefDto[]=[];
  pagination: Pagination | undefined;   //<IDeploymentPendingDto[]>| undefined;
  
  cache = new Map();
  
  pParams = new CallRecordParams();
  
  constructor(private http: HttpClient) { }

  getOrAddCallRecord(callRecord: CallRecordItemToCreateDto) {
    
    var params = GetHttpParamsForCallItemCreate(callRecord);

    return this.http.get<ICallRecord>(this.apiUrl + 'CallRecord/getOrAddHistoryWithItems', {params});
  }
  
  setParams(params: ICallRecordParams) {
    this.pParams = params;
  }

  getParams() {
    return this.pParams;
  }

}

export function GetHttpParamsForProddspectives(dParams: prospectiveSummaryParams) {

  let params = new HttpParams();

  if(dParams.personType==='') dParams.personType='Candidate';
  params = params.append('pageNumber', dParams.pageNumber);
  params = params.append('pageSize', dParams.pageSize)
  params = params.append('personType', dParams.personType);
  console.log('prospectivesummaryParams:', params);
  
  if(dParams.categoryRef !== '') params = params.append('categoryRef', dParams.categoryRef);
  //if(!Number.isNaN(dParams.dateRegistered.getTime())) params=params.append('dateRegistered', dParams.dateRegistered.toString());
  if(dParams.status !== '') params = params.append('status', dParams.status);
  if(dParams.email !=='') params=params.append('email', dParams.email);
  if(dParams.phoneNo !=='') params=params.append('phoneNo', dParams.phoneNo);
  if(dParams.username !== '') params=params.append('username', dParams.username);
  
  return params;
}
