import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { environment } from 'src/app/environments/environment';
import { IUser } from '../../models/admin/user';
import { assessmentStddQParam } from '../../models/admin/assessmentStddQParam';
import { IAssessmentStandardQ } from '../../models/admin/assessmentStandardQ';
import { HttpClient } from '@angular/common/http';
import { IAssessmentQ } from '../../models/admin/assessmentQ';

@Injectable({
  providedIn: 'root'
})
export class StddqsService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  qParams = new assessmentStddQParam();
  stddqs: IAssessmentStandardQ[]=[];
  stddq?: IAssessmentStandardQ;

  routeId: string='';
  user?: IUser;
  cache = new Map();

  constructor(private http: HttpClient) { }

  getStddQsWithoutCache() {
    return this.http.get<IAssessmentStandardQ[]>(this.apiUrl + 'assessmentstddq')
    .pipe(
      map(response => {
        this.cache.set(Object.values(this.qParams).join('-'), response);
        this.stddqs = response;
        return response;
      })
    );
  }

  getStddQs() {   //useCache: boolean=false) {
      return this.http.get<IAssessmentQ[]>(this.apiUrl + 'assessmentstddq');
  }

  getStddQ(id: number) {
    if(this.cache.size > 0) {
      const qparam = new assessmentStddQParam();
      qparam.id=id;
      if(this.cache.has(Object.values(qparam).join('-'))) {
        this.stddq=this.cache.get(Object.values(qparam).join('-'));
        console.log('retrieved stdQ from cache');
        return of(this.stddq);
      }
    }
    console.log('retrieving stddQ from api');
    return this.http.get<IAssessmentStandardQ>(this.apiUrl + 'assessmentstddq/byid/' + id);
  }

  deletestddq(id: number) {
    return this.http.delete(this.apiUrl + 'assessmentstddq/' + id);
  }

  createStddQ(q:IAssessmentStandardQ) {
    return this.http.post<IAssessmentStandardQ>(this.apiUrl + 'assessmentstddq', q);
  }

  updateStddQ(qs:IAssessmentStandardQ[]) {
    console.log('stddqs.ervice');
    return this.http.put<boolean>(this.apiUrl + 'assessmentstddq', qs);
  }
  
  setQParams(params: assessmentStddQParam) {
    this.qParams = params;
  }
  getQParams() {
    return this.qParams;
  }

}