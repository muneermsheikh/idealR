import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { assessmentStddQParam } from 'src/app/_models/admin/assessmentStddQParam';
import { IAssessmentStandardQ } from 'src/app/_models/admin/assessmentStandardQ';
import { assessmentQBankParams } from 'src/app/_models/admin/assessmentQBankParams';
import { getPaginationHeaderAssessmentQBankParams } from '../paginationHelper';
import { IAssessmentQBankDto } from 'src/app/_dtos/hr/assessmentQBankDto';

@Injectable({
  providedIn: 'root'
})
export class StddqsService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  qParams = new assessmentStddQParam();
  stddqs: IAssessmentStandardQ[]=[];
  stddq?: IAssessmentStandardQ;

  routeId: string='';
  user?: User;
  cache = new Map();

  constructor(private http: HttpClient) { }

  getStddQsWithoutCache(aParams: assessmentQBankParams) {

    let params = getPaginationHeaderAssessmentQBankParams(aParams);

    return this.http.get<IAssessmentQBankDto[]> (this.apiUrl 
      + 'AssessmentQBank/assessmentstddqs', {params}).pipe(
    map(response => {
          return response;
    }))
    
  }

  getStddQ(id: number) {
    if(this.cache.size > 0) {
      const qparam = new assessmentStddQParam();
      qparam.id=id;
      if(this.cache.has(Object.values(qparam).join('-'))) {
        this.stddq=this.cache.get(Object.values(qparam).join('-'));
        //console.log('retrieved stdQ from cache');
        return of(this.stddq);
      }
    }
    return this.http.get<IAssessmentStandardQ>(this.apiUrl + 'assessmentstddq/byid/' + id);
  }

  deletestddq(questionid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'AssessmentQBank/stddq/' + questionid);
  }

  createStddQ(q:IAssessmentStandardQ) {
    return this.http.post<IAssessmentStandardQ>(this.apiUrl + 'AssessmentQBank/stddq', q);
  }

  updateStddQ(qs:IAssessmentStandardQ[]) {
    return this.http.put<boolean>(this.apiUrl + 'AssessmentQBank/stddq', qs);
  }
  
  setQParams(params: assessmentStddQParam) {
    this.qParams = params;
  }
  getQParams() {
    return this.qParams;
  }

}