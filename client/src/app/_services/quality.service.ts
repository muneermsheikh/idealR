import { Injectable } from '@angular/core';
import { MedicalParams } from '../_models/admin/objectives/medicalParams';
import { HttpClient, HttpParams } from '@angular/common/http';
import { getPaginatedResult } from './paginationHelper';
import { IMedicalObjective } from '../_models/admin/objectives/medicalObjective';
import { environment } from 'src/environments/environment.development';
import { map, of, ReplaySubject } from 'rxjs';
import { User } from '../_models/user';
import { Pagination } from '../_models/pagination';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from './account.service';
import { IHrObjective } from '../_models/admin/objectives/hrObjective';

@Injectable({
  providedIn: 'root'
})
export class QualityService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  cache = new Map();
  cacheHR = new Map();
  user?: User;
  
  hrPagination: Pagination | undefined;
  medPagination: Pagination|undefined;
  medParams = new MedicalParams();

  constructor(private http: HttpClient, private toastr:ToastrService, private accountService: AccountService) {
    accountService.currentUser$.subscribe({
      next: response => this.user = response!
    })
   }

  
  getPaginatedMedExecPerf(mParams: MedicalParams) {
    
    const response = this.cache.get(Object.values(mParams).join('-'));
      if(response) return of(response);

    var params = new HttpParams();
    params = params.append('fromdate', convertDateToDateOnly(mParams.fromDate));
    params = params.append('uptodate', convertDateToDateOnly(mParams.uptoDate));
    params = params.append('pageNumber', mParams.pageNumber.toString());
    params = params.append('pageSize', mParams.pageSize.toString());
    
    return getPaginatedResult<IMedicalObjective[]>(this.apiUrl + 
        'Quality/medicalObjectives', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(params).join('-'), response);
        return response;
      })
    )

    //return this.http.get(this.apiUrl + 'Deployment/medicalObectives');
  }
  
  createOrderAssignmentTasks(orderItemIds: number[]) {
    return this.http.post<string>(this.apiUrl + 'Quality/assignToHRExecs', orderItemIds);
  }

  
  getPaginatedHRPerf(mParams: MedicalParams) {
    const response = this.cacheHR.get(Object.values(mParams).join('-'));
      if(response) return of(response);

    var params = new HttpParams();
    params = params.append('fromdate', convertDateToDateOnly(mParams.fromDate));
    params = params.append('uptodate', convertDateToDateOnly(mParams.uptoDate));
    params = params.append('pageNumber', mParams.pageNumber.toString());
    params = params.append('pageSize', mParams.pageSize.toString());
        
    return getPaginatedResult<IHrObjective[]>(this.apiUrl + 
        'Quality/hrObjectives', params, this.http).pipe(
      map(response => {
        this.cacheHR.set(Object.values(params).join('-'), response);
        return response;
      })
    )

    //return this.http.get(this.apiUrl + 'Deployment/medicalObectives');
  }

  
  getPendingHRTasks(mParams: MedicalParams) {
    
    const response = this.cacheHR.get(Object.values(mParams).join('-'));
      if(response) return of(response);

    var params = new HttpParams();
    params = params.append('pageNumber', mParams.pageNumber.toString());
    params = params.append('pageSize', mParams.pageSize.toString());
        
    return getPaginatedResult<IHrObjective[]>(this.apiUrl + 
        'Quality/pendingHRObjectives', params, this.http).pipe(
      map(response => {
        this.cacheHR.set(Object.values(params).join('-'), response);
        return response;
      })
    )
  }

  initializeCacheHR() {
    this.cacheHR = new Map();
  }
  
  setMedParams(params: MedicalParams) {
    this.medParams = params;
  }
  
  getMedParams() {
    return this.medParams;
  }


}
  
export function convertDateToDateOnly(datepart: Date) {

  const milliseconds: number = +datepart; // Replace with your milliseconds value
  const date: Date = new Date(milliseconds);

  // Extract the date part
  const year: number = date.getFullYear();
  const month: number = date.getMonth() + 1; // Months are zero-based
  const day: number = date.getDate();

  // Format the date as YYYY-MM-DD
  const formattedDate: string = `${year}-${month.toString().padStart(2, '0')}-${day.toString().padStart(2, '0')}`;

  //return new Date(Date.UTC(year, month, day));
  return formattedDate;
}
