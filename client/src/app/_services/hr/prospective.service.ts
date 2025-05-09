import { Injectable } from '@angular/core';
import { ReplaySubject, map, of, take } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
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
import { CallRecordStatusReturnDto } from 'src/app/_dtos/admin/callRecordStatusReturnDto';
import { IProspectiveHeaderDto } from 'src/app/_dtos/hr/prospectiveHeaderDto';
import { AccountService } from '../account.service';
import { IComposeCallRecordMessageDto } from 'src/app/_dtos/hr/composeCallRecordMessageDto';


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

  user?: User;

  constructor(private http: HttpClient, private accountService: AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
          next: user => {
            if (user)
              this.user = user;
          }
        })
   }

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
  
    
  getOrAddCallRecord(personType: string, personId: string) {
    
    return this.http.get<ICallRecord>(this.apiUrl + 'CallRecord/getOrAddHistoryWithItems/' + personType + '/' + personId);
  }
  
  
  getProspectivesPaged(dParams: prospectiveCandidateParams) {

      const response = this.cache.get(Object.values(dParams).join('-'));
      if(response) return of(response);

      let params = getHttpParamsForProspectiveCandidates(dParams);

      console.log('prospective service getprospectivespaged params:', params);
  
      return getPaginatedResult<IProspectiveBriefDto[]>(this.apiUrl + 
          'Prospectives/pagedlist', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(dParams).join('-'), response);
          return response;
        })
      )
    }
  
    getProspectivesList(orderno: string, status: string) {

      return this.http.get<IProspectiveBriefDto[]>(this.apiUrl + 'Prospectives/list/' + orderno + '/' + status);
        
    }

    getProspectiveHeadersDto(st: string) {
      return this.http.get<IProspectiveHeaderDto[]>(this.apiUrl + 'Prospectives/headers/' + st);
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
      
      return this.http.get<IProspectiveSummaryDto[]>(this.apiUrl + 'Prospectives/summary', {params})
        .pipe(
          map(response => {
            this.cache.set(Object.values(this.sParams).join('-'), response);
            this.summaries = response;
            return response;
          })
        )
    }
    
    composeCallRecordMessage(compose: IComposeCallRecordMessageDto[]) {
      return this.http.post<IComposeCallRecordMessageDto[]>(this.apiUrl + 'Prospectives/ComposeMessages', compose);
    }

    setParams(params: prospectiveCandidateParams) {
    this.oParams = params;
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

  convertProspectiveToCandidate(prospectiveId: number) {
    return this.http.put<number>(this.apiUrl + 'Prospectives/convertProspective/' + prospectiveId, {});
  }

  createCandidateFromprospective(model: IProspectiveRegisterToAddDto) {
    return this.http.post(this.apiUrl + 'Prospectives', model )
  }

  updateProspectives(model: IProspectiveUpdateDto[]) {
    return this.http.put(this.apiUrl + 'Prospectives/prospectivelistedit', model);
  }

  updateCallRecord(model: ICallRecord) {
    return this.http.put<CallRecordStatusReturnDto>(this.apiUrl + 'CallRecord', model);
  }

  deleteProspectiveRecord(prospectiveId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Prospectives/delete/' + prospectiveId);
  }

  
}
