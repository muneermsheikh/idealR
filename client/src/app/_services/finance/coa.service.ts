import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { ICOA } from 'src/app/_models/finance/coa';
import { ParamsCOA } from 'src/app/_models/params/finance/paramsCOA';
import { CandidateCOAParamsDto } from 'src/app/_dtos/finance/candidateCOAParamsDto';
import { Pagination } from 'src/app/_models/pagination';
import { IPendingDebitApprovalDto } from 'src/app/_dtos/finance/pendingDebitApprovalDto';
import { COADto } from 'src/app/_dtos/finance/coaDto';
import { getPaginatedResult, getPaginationHeaders } from '../paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class COAService {
 
  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  sParams = new ParamsCOA();

  coalists: ICOA[]=[];
  pagination?: Pagination | undefined;
  cache = new Map();
  candidateCOAParams = new CandidateCOAParamsDto();

  constructor(private http: HttpClient) { }

  getCoaList() {
    return this.http.get<ICOA[]>(this.apiUrl + 'finance/coalist');
  }

  createCandidateCOA(appno: number) {
    return this.http.get<ICOA>(this.apiUrl + 'finance/candidateCOA/' + appno + '/' + true);
  }

  
  /*getCandidateCOAWithClBal(appno: number) {
    return this.http.get<COADto>(this.apiUrl + 'finance/coaforcandidate/' + appno + '/' + false);
  }
  */

  /*
  getCoas(oParams: ParamsCOA) {

    const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = this.populateCOAParams(oParams);
        
      return getPaginatedResult<ICOA[]>(this.apiUrl + 'users', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )
   
    }
*/
  editCOA(coa : ICOA | undefined)
  {
      return this.http.put<ICOA>(this.apiUrl + 'finance/coa', coa);
  }

  addNewCOA(coa: ICOA) {
    //console.log('sending to api, coa:', coa);
    return this.http.post<ICOA>(this.apiUrl + 'finance/coa', coa);
  }
  
  deleteCOA(id: number)
  {
    return this.http.delete<boolean>(this.apiUrl + 'finance/coa/' + id);
  }

  updateCOA(coa:ICOA) {
    return this.http.put<boolean>(this.apiUrl + 'finance/coa', coa);
  }

  getMatchingCOAs(coaname: string) {
    return this.http.get<string[]>(this.apiUrl + 'finance/matchingcoas/' + coaname);
  }
    
  deleteFromCache(id: number) {
    this.cache.delete(id);
    this.pagination!.totalItems--;
  }

  
  getGroupOfCOAs(group: string) {
    return this.http.get<ICOA[]>(this.apiUrl + 'Finance/coabygroup/'+  group);
  }

  getCandidateCOAs(appno: number) {
    return this.http.get<ICOA[]>(this.apiUrl + 'Finance/coasforpayment/' + appno);
  }

  
  populateCOAParams(oParams: ParamsCOA) {
    let params = getPaginationHeaders(oParams.pageNumber, oParams.pageSize);
  
      if (oParams.search) params = params.append('search', oParams.search);
      if (oParams.accountName !== '' )  params = params.append('coaId', oParams.accountName);
      if (oParams.sort !== '') params = params.append('sort', oParams.sort);
      if (oParams.accountId !== 0) params = params.append('accountId', oParams.accountId.toString());
      if (oParams.accountType !== '') params = params.append('accountType', oParams.accountType);
      if (oParams.accountClass !== '') params = params.append('accountClass', oParams.accountClass);
      
    return params;
  }

  setParams(params: ParamsCOA) {
    this.sParams = params;
  }
  
  getParams() {
    return this.sParams;
  }
}
