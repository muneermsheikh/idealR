import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { UserHistoryParams } from '../_models/params/userHistoryParams';
import { ICategoryRefDto } from '../_dtos/admin/categoryRefDto';
import { IUserHistory } from '../_models/admin/userHistory';
import { Pagination } from '../_models/pagination';
import { HttpClient } from '@angular/common/http';
import { IUserHistoryDto } from '../_dtos/admin/userHistoryDto';
import { ICandidateBriefDto } from '../_dtos/admin/candidateBriefDto';
import { IUserHistoryReturnDto } from '../_dtos/admin/userHistoryReturnDto';
import { IUserHistoryItem } from '../_models/admin/userHistoryItem';
import { getHttpParamsForUserHistoryParams, getPaginatedResult} from './paginationHelper';
import { IUserHistoryBriefDto } from '../_dtos/admin/useHistoryBriefDto';

@Injectable({
  providedIn: 'root'
})
export class UserHistoryService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  histParams: UserHistoryParams | undefined;  // = new userHistoryParams();
  categoryRefDtos: ICategoryRefDto[]=[];
  categoryRefDto?: ICategoryRefDto;

  cache = new Map();

  pagination: Pagination | undefined;   // IPagination<IUserHistory>;

  //for headers
  //headerParams = new userHistoryHeaderParams();

  constructor(private http: HttpClient, private toastr: ToastrService) { }

  setParams(params:UserHistoryParams) {
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
    return this.http.get<IUserHistory>(this.apiUrl + 'UserHistory/bycandidateid/'+id);
  }
  
  getHistory(hParams: UserHistoryParams) {
    
    if(hParams.emailId===''&& hParams.mobileNo==='' && hParams.personName==='' && hParams.applicationNo===0) {
      this.toastr.info('Params null');
      return null;
    }

    let params = getHttpParamsForUserHistoryParams(hParams);
    
    return this.http.get<IUserHistoryDto>(this.apiUrl + 'UserHistory/dto', {observe: 'response', params} ) ;
    
  }

  getHistoryFromCallerNamePhone(callername: string, mobileno: string): any {
    
    return this.http.get<ICandidateBriefDto>(this.apiUrl + 'userhistory/dtofromnameandphone/' + callername + '/' + mobileno);
    
  }

  getUserHistoryHeaders(hParams: UserHistoryParams) {

    const response = this.cache.get(Object.values(hParams).join('-'));
    if(response) return of(response);

    let params = getHttpParamsForUserHistoryParams(hParams);
        
    return getPaginatedResult<IUserHistoryBriefDto[]>(this.apiUrl + 
        'userHistory/pagedlist', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(hParams).join('-'), response);
        return response;
      })
    )
    
  }
  
   getCandidateHistoryByHistoryId(id: number) {
    return this.http.get<IUserHistory>(this.apiUrl + 'UserHistory/byhistoryid/' + id);
  }

  getCandidateHistoryByCandidateId(id: number) {
    return this.http.get<IUserHistory>(this.apiUrl + 'UserHistory/bycandidateid/' + id);
  }

  getUserHistoriesByPhoneNo(phoneno: string) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromphoneno/' + phoneno);
  }

  getUserHistoriesByOfficialId(officialid: number) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromofficialid/' + officialid);
  }

  getUserHistoriesByAadharNo(aadharno: string) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromaadharno/' + aadharno);
  }

  getUserHistoriesByEmail(email: string) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromemail/' + email);
  }

  getUserHistoriesByCandidateId(candidateid: number) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromcandidateid/' + candidateid);
  }
  
  getUserHistoriesByApplicationNo(applicationno: number) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromapplicationno/' + applicationno);
  }
  
  getUserHistoryFromProspectiveId(id: number) {
    return this.http.get<IUserHistoryDto[]>(this.apiUrl + 'UserHistory/fromapplicationno/' + id);
  }

  updateCandidateHistory(model: any) {
    return this.http.put<IUserHistoryReturnDto>(this.apiUrl + 'UserHistory', model);
  }

  updateCandidateHistoryItems(items: IUserHistoryItem[]) {
    return this.http.put(this.apiUrl + 'UserHistory/items', items);
  }

  setUserParams(params: UserHistoryParams) {
      this.histParams = params;
    }
    
  getUserParams() {
      return this.histParams;
    }

  composeEmailMessageOfConsent() {

  }
}
