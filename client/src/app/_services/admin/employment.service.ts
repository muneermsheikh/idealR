import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { employmentParams } from 'src/app/_models/params/Admin/employmentParam';
import { Pagination } from 'src/app/_models/pagination';
import { getHttpParamsForEmployment, getPaginatedResult } from '../paginationHelper';
import { IEmployment } from 'src/app/_models/admin/employment';

@Injectable({
  providedIn: 'root'
})
export class EmploymentService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  eParams = new employmentParams();
  pagination: Pagination | undefined; 
  cache = new Map();

  constructor(private http: HttpClient) { }

  getEmployments( oParams: employmentParams) { 

      const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = getHttpParamsForEmployment(oParams);
  
      return getPaginatedResult<IEmployment[]>(this.apiUrl + 'Employment', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )

    }

  GetEmploymentObject(selDecisionId: number) {
      return this.http.get(this.apiUrl + 'employment/generate/' + selDecisionId);
    }
  
  updateEmployment(model: IEmployment) {
    return this.http.put(this.apiUrl + 'employment/employment', model);
  }

  
  deleteEmployment(empid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'employment/' + empid);
  }

  setEParams(params: employmentParams) {
    this.eParams = params;
  }
  
  getEParams() {
    return this.eParams;
  }


}
