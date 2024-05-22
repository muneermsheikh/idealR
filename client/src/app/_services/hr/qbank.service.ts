import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { assessmentQBankParams } from 'src/app/_models/admin/assessmentQBankParams';
import { IAssessmentQBank } from 'src/app/_models/admin/assessmentQBank';
import { Pagination } from 'src/app/_models/pagination';
import { getPaginationHeadersAssessmentQBankParams } from '../paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class QbankService {
 
  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  qParams = new assessmentQBankParams();
  qs: IAssessmentQBank[]=[];
  q?: IAssessmentQBank;
  pagination: Pagination | undefined;
  routeId: string='';
  user?: User;
  cache = new Map();

  constructor(private http: HttpClient, private toastr: ToastrService) { }

  getQs(oParams: assessmentQBankParams) {
    
      const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = getPaginationHeadersAssessmentQBankParams(oParams);

      return this.http.get<IAssessmentQBank[]>(this.apiUrl + 'assessmentqbank/qbank', {params})
      .pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          this.qs = response;
          return response;
        })
      )
   
  }

  setQParams(params: assessmentQBankParams) {
    this.qParams = params;
  }

  getQParams() {
    return this.qParams;
  }

  getQ(id: number) {
    var i=0;
    let q: IAssessmentQBank|undefined;
    if (id === 0 ) {
      return this.http.get<IAssessmentQBank>(this.apiUrl + 'assessmentqbank/byid/' + id);  
    }

    this.cache.forEach((qs: IAssessmentQBank[]) => {
      q = qs.find(p => p.categoryId === id);
      i++;
    })

    console.log('total attempts', i);
    if (q) {
      return of(q);
    }
    
    return this.http.get<IAssessmentQBank>(this.apiUrl + 'assessmentqbank/byid/' + id);
  }

  update(model: IAssessmentQBank) {
    return this.http.put<IAssessmentQBank>(this.apiUrl + 'assessmentqbank', model);
  }

  insert(model: IAssessmentQBank) {
    return this.http.post<IAssessmentQBank>(this.apiUrl + 'assessmentqbank', model);
  }
}
