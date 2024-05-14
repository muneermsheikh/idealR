import { Injectable } from '@angular/core';
import { environment } from 'src/app/environments/environment';
import { interviewParams } from '../../params/admin/interviewParams';
import { IPagination } from '../../models/pagination';
import { IInterview } from '../../models/hr/interview';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map, of } from 'rxjs';
import { IInterviewItemDto } from '../../models/hr/interviewItemDto';
import { IInterviewBrief } from '../../models/hr/interviewBrief';

@Injectable({
  providedIn: 'root'
})
export class InterviewService {
  
  apiUrl = environment.apiUrl;
  //private currentUserSource = new ReplaySubject<IUser>(1);
  //currentUser$ = this.currentUserSource.asObservable();
  params = new interviewParams();
  pagination?: IPagination<IInterviewBrief[]>;
  cache = new Map();

  constructor(private http: HttpClient) { }

  getInterviews(useCache: boolean=true): Observable<IPagination<IInterviewBrief[]>> { 
    
    if (useCache === false)  this.cache = new Map();
    
    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.params).join('-'))) {
        this.pagination = this.cache.get(Object.values(this.params).join('-'));
        if(this.pagination) return of(this.pagination);
      }
    }

    let params = new HttpParams();
    if (this.params.orderNo !== 0) params = params.append('orderNo', this.params.orderNo.toString());
    if (this.params.orderId !== 0) params = params.append('orderIdId', this.params.orderId!.toString());
    if (this.params.customerId !== 0) params = params.append('customerId', this.params.customerId!.toString());
    if (this.params.customerNameLike !== '') params = params.append('customerNameLike', this.params.customerNameLike);
    if (this.params.interviewVenue !== '') params = params.append('interviewVenue', this.params.interviewVenue);
    if (this.params.search) params = params.append('search', this.params.search);
    
    params = params.append('sort', this.params.sort);
    params = params.append('pageIndex', this.params.pageNumber.toString());
    params = params.append('pageSize', this.params.pageSize.toString());
    
    return this.http.get<IPagination<IInterviewBrief[]>>(this.apiUrl + 
        'interview/interviews', {params}).pipe(
      map(response => {
        this.cache.set(Object.values(this.params).join('-'), response)
        this.pagination = response;
        return this.pagination  // response;
      })
    )

  }

  /*
  getInterviews(useCache: boolean): Observable<IPagination<IInterviewBrief[]>> {

    if (useCache === false) {
       this.cache = new Map();
       this.params = new interviewParams();
    }

    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.params).join('-'))) {
        this.pagination!.data = this.cache.get(Object.values(this.params).join('-'));
        if(this.pagination) return of(this.pagination);
      }
    }

    let params = new HttpParams();
    if (this.params.orderNo !== 0) params = params.append('orderNo', this.params.orderNo.toString());
    if (this.params.orderId !== 0) params = params.append('orderId', this.params.orderId.toString());
    if (this.params.customerId !== 0) params = params.append('orderId', this.params.orderId.toString());
    if (this.params.customerNameLike !== '') params = params.append('customerNameLike', this.params.customerNameLike);
    if (this.params.interviewVenue !== '') params = params.append('interviewVenue', this.params.interviewVenue);
    if (this.params.search) params = params.append('search', this.params.search);
    
    params = params.append('sort', this.params.sort);
    params = params.append('pageIndex', this.params.pageNumber.toString());
    params = params.append('pageSize', this.params.pageSize.toString());
    
    return this.http.get<IPagination<IInterviewBrief[]>>(this.apiUrl + 
        'interview/interviews', {params}).pipe(
        map((response: IPagination<IInterviewBrief[]> | null) => {
         // if(response !==null) {
            this.cache.set(Object.values(this.params).join('-'), response);
            this.pagination = response!;
            return response;
         // } else {return null}
        })
      )
    }

  */
  getInterviewById(id: number) {
    return this.http.get<IInterview>(this.apiUrl + 'interview/interviewById/' + id);
  }
  
  addInterview(model: IInterview) {
    return this.http.post<IInterview>(this.apiUrl + 'interview/addinterview', model);
  }

  updateInterview(model: IInterview) {
    return this.http.put<IInterview>(this.apiUrl + 'interview/editInterview', model);
  }

  deleteInterview(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'interview/deleteInterviewbyid/' + id);
  }

  getInterviewItemCatAndCandidates(interviewItemId: number) {
    return this.http.get<IInterviewItemDto[]>(this.apiUrl + 'interview/catandcandidates/' + interviewItemId );
  }
  
  //GetOrCreateInterview
  //if the Interview data exists in DB, returns the same
  //if it does not exist, creates an Object and returns it
  getOrCreateInterview(orderid: number) { //returns itnerview + interviewItems
    return this.http.get<IInterview>(this.apiUrl + 'interview/getorcreateinterview/' + orderid);
  }
  
  getOrCreateInterviewFromOrderNo(orderno: number) { //returns itnerview + interviewItems
    return this.http.get<IInterview>(this.apiUrl + 'interview/getorcreateinterviewfromorderno/' + orderno);
  }

  getParams(){
    return this.params;
  }

  setParams(p: interviewParams) {
    this.params = p;
  }
}
