import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { Pagination } from '../_models/pagination';
import { FeedbackParams } from '../_models/params/hr/feedbackParams';
import { GetHttpParamsForFeedback, getPaginatedResult } from './paginationHelper';
import { IFeedback, IFeedbackInput } from '../_models/hr/feedback';
import { IFeedbackDto } from '../_dtos/hr/feedbackDto';
import { IFeedbackHistoryDto } from '../_dtos/admin/feedbackAndHistoryDto';

@Injectable({
  providedIn: 'root'
})

export class FeedbackService {
  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  fParams = new FeedbackParams();

  pagination: Pagination | undefined; //<IUserHistoryDto[]>;
  cache = new Map();

  constructor(private http: HttpClient) { }

  getFeedbacks(fParams: FeedbackParams) {
    const response = this.cache.get(Object.values(fParams).join('-'));
    if(response) return of(response);

    let params = GetHttpParamsForFeedback(fParams);
    
    return getPaginatedResult<IFeedbackDto[]>
      (this.apiUrl + 'Feedback/pagedlist', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(fParams).join('-'), response);
          return response;
        })
      )
  }

  getFeedbackWithItems(feedbackId: number) {
    return this.http.get<IFeedback>(this.apiUrl + 'Feedback/FeedbackFromId/' + feedbackId);
  }

 
  updateFeedback(fdback: IFeedback) {
    return this.http.put<IFeedback>(this.apiUrl + 'Feedback', fdback);
  }

  deleteFeedback(fdbackId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Feedback/delete/' + fdbackId);
  }

  saveNewFeedback(feedback: IFeedbackInput) {
    return this.http.post<IFeedback>(this.apiUrl + 'Feedback/saveFeedback', feedback);
  }

  deleteFeedbackInput(inputId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Feedback/deleteinput/' + inputId);
  }

  getFeedbackObject(feedbackId: number, customerId: number) {
    
    return this.http.get<IFeedback>(this.apiUrl + 'Feedback/newfeedback/' + feedbackId + '/' + customerId  );
  }
  
  getFeedbackHistory(customerId: number) {
    return this.http.get<IFeedbackHistoryDto[]>(this.apiUrl + 'Feedback/history/' + customerId);
  }

  sendFeedbackEmailToClient(id: number) {

    var url ="sample string"; // "<a [routerLink]=[" + this.apiUrl + '/Feedback/' + id + "] >here</a>";
    console.log('reached feedbackservice, URL', url);
    //return this.http.get<string>(this.apiUrl + 'Feedback/sendfeedback/' + id + '/' + url);
    return this.http.get<IFeedbackHistoryDto[]>(this.apiUrl + 'Feedback/history/' + id);
  }

  setParams(params: FeedbackParams) {
    this.fParams = params
  }

  getParams() {
    return this.fParams;
  }

}
