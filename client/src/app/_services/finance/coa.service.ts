import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { ICOA } from 'src/app/_models/finance/coa';
import { ParamsCOA } from 'src/app/_models/params/finance/paramsCOA';
import { CandidateCOAParamsDto } from 'src/app/_dtos/finance/candidateCOAParamsDto';
import { Pagination } from 'src/app/_models/pagination';
import { GetHttpParamsForCOA, getPaginatedResult } from '../paginationHelper';

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

    
  getCoas(useCache: boolean=true) {

    var oParams = this.sParams;  
    
      if(useCache) {
          const response = this.cache.get(Object.values(oParams).join('-'));
          if(response) return of(response);
      }

      let params = GetHttpParamsForCOA(oParams);
      console.log('sParams', params);
      
      return getPaginatedResult<ICOA[]>(this.apiUrl + 'Finance/coapagedlist', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )
   
    }

  editCOA(coa : ICOA | undefined)
  {
      if(coa!.id === 0) {
        return this.http.post<ICOA>(this.apiUrl + 'finance/coa', coa)
      } else {
        return this.http.put<ICOA>(this.apiUrl + 'finance/coa', coa);
      }
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


  
  setParams(params: ParamsCOA) {
    this.sParams = params;
  }
  
  getParams() {
    return this.sParams;
  }
}
