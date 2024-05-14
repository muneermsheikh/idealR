import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { UserHistoryParams } from '../_models/params/userHistoryParams';
import { ICategoryRefDto } from '../_dtos/admin/categoryRefDto';
import { categoryRefParams } from '../_models/params/categoryRefParams';
import { IUserHistory } from '../_models/admin/userHistory';
import { Pagination } from '../_models/pagination';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IUserHistoryDto } from '../_dtos/admin/userHistoryDto';
import { ICandidateBriefDto } from '../_dtos/admin/candidateBriefDto';
import { userHistoryHeaderParams } from '../_models/params/Admin/UserHistoryHeaderParams';
import { IUserHistoryHeader } from '../_models/admin/userHistoryHeader';
import { IUserHistoryReturnDto } from '../_dtos/admin/userHistoryReturnDto';
import { IUserHistoryItem } from '../_models/admin/userHistoryItem';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class CandidateHistoryService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  histParams: UserHistoryParams | undefined;  // = new userHistoryParams();
  categoryRefParams: categoryRefParams | undefined; 
  categoryRefDtos: ICategoryRefDto[]=[];
  categoryRefDto?: ICategoryRefDto;

  cache = new Map();

  pagination: Pagination | undefined;   // IPagination<IUserHistory>;

  //for headers
  //headerParams = new userHistoryHeaderParams();

  constructor(private http: HttpClient, private toastr: ToastrService) { }

  setParams(params:categoryRefParams) {
    this.categoryRefParams = params;
  }
  
  getParams() {
    return this.categoryRefParams;
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
    //console.log('hParams in candidaatehistoryservice.getHistory', hParams);
    if(hParams.emailId===''&& hParams.mobileNo==='' && hParams.personName==='' && hParams.applicationNo===0) {
      this.toastr.info('Params null');
      return null;
    }
    
    let params = new HttpParams();
    if (hParams.emailId !== "") params = params.append('emailId', hParams.emailId);
    if (hParams.mobileNo !== '' ) params = params.append('mobileNo', hParams.mobileNo);
    if (hParams.personName !== '' ) params = params.append('personName', hParams.personName);
    if (hParams.applicationNo! > 0) params = params.append('applicationNo', hParams.applicationNo!);
  
    return this.http.get<IUserHistoryDto>(this.apiUrl + 'userhistory/dto', {observe: 'response', params} ) ;
    
  }

  getHistoryFromCallerNamePhone(callername: string, mobileno: string): any {
    
    return this.http.get<ICandidateBriefDto>(this.apiUrl + 'userhistory/dtofromnameandphone/' + callername + '/' + mobileno);
    
  }

  getUserHistoryHeaders(hParams: userHistoryHeaderParams) {

    let params = getPaginationHeaders(hParams.pageNumber, hParams.pageSize);
    //let params = new HttpParams();
    if (hParams.assignedToId !== 0) params = params.append('assignedToId', hParams.assignedToId!.toString());
    if (hParams.assignedById !== 0) params = params.append('assignedById', hParams.assignedById!.toString());
    if (hParams.status !== '' ) params = params.append('status', hParams.status!);
    if(hParams.status==='' && hParams.assignedToId===0 && hParams.assignedById===0) {
      this.toastr.info('Params null');
      return null;
    }

    return getPaginatedResult<IUserHistoryHeader[]>(this.apiUrl + 'userHistory/headers', params, this.http).pipe(
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
