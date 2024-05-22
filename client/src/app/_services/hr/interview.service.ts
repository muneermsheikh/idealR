import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { interviewParams } from 'src/app/_models/params/Admin/interviewParams';
import { Pagination } from 'src/app/_models/pagination';
import { getPaginatedResult, getPaginationHeadersInterviewParams } from '../paginationHelper';
import { IInterviewBrief } from 'src/app/_models/hr/interviewBrief';
import { IInterview } from 'src/app/_models/hr/interview';
import { IInterviewItemDto } from 'src/app/_models/hr/interviewItemDto';

@Injectable({
  providedIn: 'root'
})
export class InterviewService {
  
    apiUrl = environment.apiUrl;
    //private currentUserSource = new ReplaySubject<IUser>(1);
    //currentUser$ = this.currentUserSource.asObservable();
    params = new interviewParams();
    pagination: Pagination | undefined;
    cache = new Map();

    constructor(private http: HttpClient) { }

    getInterviews(oParams: interviewParams) { 
      
        const response = this.cache.get(Object.values(oParams).join('-'));
        if(response) return of(response);
      
        let params = getPaginationHeadersInterviewParams(oParams);

        return getPaginatedResult<IInterviewBrief[]>(this.apiUrl 
          + 'interview/interviews', params, this.http).pipe(
          map(response => {
            this.cache.set(Object.values(oParams).join('-'), response);
            return response;
          }))
    }

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
