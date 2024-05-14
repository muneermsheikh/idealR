import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { Pagination } from 'src/app/_models/pagination';
import { paramsMasters } from 'src/app/_models/params/masters/paramsMasters';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult, getPaginationHeaders } from '../paginationHelper';
import { IIndustryType } from 'src/app/_models/admin/industryType';

@Injectable({
  providedIn: 'root'
})
export class IndustriesService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  mParams = new paramsMasters();
  
  pagination: Pagination | undefined;

  cache = new Map();
  
  constructor(private http: HttpClient) { }

  
  getIndustryPaged(mParams: paramsMasters) {
   
        const response = this.cache.get(Object.values(mParams).join('-'));
        if(response) return of(response);
    
        let params = getPaginationHeaders(mParams.pageNumber, mParams.pageSize);
    
        if (this.mParams.name !== '') params = params.append('name', this.mParams.name!);
        if (this.mParams.id !== 0) params = params.append('id', this.mParams.id!.toString());
        if (this.mParams.search) params = params.append('search', this.mParams.search);
        
        params = params.append('pageIndex', this.mParams.pageNumber.toString());
        params = params.append('pageSize', this.mParams.pageSize.toString());
        
        return getPaginatedResult<IIndustryType[]>(this.apiUrl + 'masters/indpaginated', params, this.http).pipe(
          map(response => {
            this.cache.set(Object.values(mParams).join('-'), response);
            return response;
          })
        )
    
    }

  getIndustry(id:number) {
    return this.http.get<IIndustryType>(this.apiUrl + 'masters/industry')
  }

  getIndustries() {
    return this.http.get<IIndustryType[]>(this.apiUrl + 'masters/industrieslist');
  }

  deleteIndustry(industryid: number) {
    return this.http.delete<boolean>(this.apiUrl+ 'masters/deleteindustry/' +  industryid);
  }

  updateIndustry(id: number, name: string) {
    var ind: IIndustryType = {id: id, name: name};
    return this.http.put<boolean>(this.apiUrl + 'masters/editindustry', ind);
  }
}
