import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';
import { getHttpParamsForIndustries, getPaginatedResult} from '../paginationHelper';
import { IIndustryType } from 'src/app/_models/admin/industryType';
import { IndustryParams } from 'src/app/_models/params/masters/industryParams';

@Injectable({
  providedIn: 'root'
})
export class IndustriesService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  mParams = new IndustryParams();
  
  pagination: Pagination | undefined;

  cache = new Map();
  
  constructor(private http: HttpClient) { }

  
  getIndustryPaged(mParams: IndustryParams) {
   
        const response = this.cache.get(Object.values(mParams).join('-'));
        if(response) return of(response);
    
        let params = getHttpParamsForIndustries(mParams);
    
        return getPaginatedResult<IIndustryType[]>(this.apiUrl + 'Industries/industryPaged', params, this.http).pipe(
          map(response => {
            this.cache.set(Object.values(mParams).join('-'), response);
            return response;
          })
        )
    
    }

  getIndustry(id:number) {
    return this.http.get<IIndustryType>(this.apiUrl + 'Industries/industrybyid/' + id );
  }

  getIndustryList() {
    return this.http.get<IIndustryType[]>(this.apiUrl + 'Industries/industrylist');
  }

  deleteIndustry(industryid: number) {
    return this.http.delete<boolean>(this.apiUrl+ 'Industries/deletebyid/' +  industryid);
  }

  updateIndustry(industry: IIndustryType) {
    if(industry.id===0) {
      return this.http.post<IIndustryType>(this.apiUrl + 'Industries/add', industry)
    } else {
      return this.http.put<boolean>(this.apiUrl + 'Industries/edit', industry);
    }
  }

  
  setParams(params: IndustryParams) {
    this.mParams = params;
  }
  
  getParams() {
    return this.mParams;
  }
}
