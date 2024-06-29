import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { prospectiveCandidateParams } from 'src/app/_models/params/hr/prospectiveCandidateParams';
import { prospectiveSummaryParams } from 'src/app/_models/params/hr/prospectiveSummaryParams';
import { IProspectiveSummaryDto } from 'src/app/_dtos/hr/propectiveSummaryDto';
import { Pagination } from 'src/app/_models/pagination';
import { GetHttpParamsForCallItemCreate, GetHttpParamsForCallRecord, getHttpParamsForProspectiveCandidates, getHttpParamsForProspectiveSummary, getPaginatedResult } from '../paginationHelper';
import { IProspectiveCandidate } from 'src/app/_models/hr/prospectiveCandidate';
import { IProspectiveRegisterToAddDto } from 'src/app/_dtos/hr/prospectiveRegisterToAddDto';
import { IProspectiveUpdateDto } from 'src/app/_dtos/hr/prospectiveUpdateDto';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { ICallRecordParams } from 'src/app/_models/params/callRecordParams';
import { IProspectiveBriefDto } from 'src/app/_dtos/hr/prospectiveBriefDto';
import { CallRecordItemToCreateDto } from 'src/app/_dtos/hr/callRecordItemToCreateDto';


@Injectable({
  providedIn: 'root'
})
export class ProspectiveService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  oParams = new prospectiveCandidateParams();
  //callRecordParams = new CallRecordParams();
  sParams = new prospectiveSummaryParams();
  //paginationSummary = new PaginationProspectiveSummary();
  summaries: IProspectiveSummaryDto[]=[];
  pagination: Pagination | undefined;
  cache = new Map();
  cacheSummary = new Map();

  constructor(private http: HttpClient) { }

  getProspectiveCandidates(oParams: prospectiveCandidateParams) {
        
      const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = getHttpParamsForProspectiveCandidates(oParams);
    
      return getPaginatedResult<IProspectiveCandidate[]>(this.apiUrl + 'prospectivecandidates', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        }))

    }
  
    
  getOrAddCallRecord(callRecord: CallRecordItemToCreateDto) {
    
    var params = GetHttpParamsForCallItemCreate(callRecord);

    return this.http.get<ICallRecord>(this.apiUrl + 'CallRecord/getOrAddHistoryWithItems', {params});
  }
  
  
    getProspectivesPaged(dParams: prospectiveCandidateParams) {

      const response = this.cache.get(Object.values(dParams).join('-'));
      if(response) return of(response);
  
      //let params = GetHttpParamsForCallRecord(dParams);
      let params = getHttpParamsForProspectiveCandidates(dParams);
  
      return getPaginatedResult<IProspectiveBriefDto[]>(this.apiUrl + 
          'Prospectives/pagedlist', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(dParams).join('-'), response);
          return response;
        })
      )
    }
  
  
  getProspectiveSummary(useCache: boolean)
  {
    if (useCache === false) this.cacheSummary = new Map();
    
    if (this.cacheSummary.size > 0 && useCache === true) {
      if (this.cacheSummary.has(Object.values(this.sParams).join('-'))) {
        this.summaries= this.cacheSummary.get(Object.values(this.sParams).join('-'));
        return of(this.summaries);
      }
    }

    let params = getHttpParamsForProspectiveSummary(this.sParams);
    
    return this.http.get<IProspectiveSummaryDto[]>(this.apiUrl + 'ProspectiveCandidates/summary', {params})
      .pipe(
        map(response => {
          this.cache.set(Object.values(this.sParams).join('-'), response);
          this.summaries = response;
          return response;
        })
      )
  }
  
  setParams(params: prospectiveCandidateParams) {
    this.oParams = params;
    console.log('in prospective.sevice, params set to', this.oParams);
  }
  
  getParams() {
    return this.oParams;
  }

  setSummaryParams(params: prospectiveSummaryParams) {
    this.sParams = params;
    console.log('summaryParams set to:', params);
  }

  getSummaryParams() {
    return this.sParams;
  }

  displayModalUserContact() {
    
  }

  uploadProspectiveXLSFile(prospectivexlFile: any){
    //console.log('prosective upload in prospecive service');
    return this.http.post<string>(this.apiUrl + 'excel/uploadProspectiveFile', prospectivexlFile)
  }

  convertProspectiveXLStoDb(filename: string) {
    return this.http.post<string>(this.apiUrl + 'excel/convertToDb/' + filename, {});
  }
  createCandidateFromprospective(model: IProspectiveRegisterToAddDto) {
    return this.http.post(this.apiUrl + 'prospectivecandidates', model )
  }

  updateProspectives(model: IProspectiveUpdateDto[]) {
    return this.http.put(this.apiUrl + 'prospectivecandidates/prospectivelistedit', model);
  }

  updateCallRecord(model: any) {
    return this.http.put<ICallRecord>(this.apiUrl + 'CallRecord', model);
  }
}
