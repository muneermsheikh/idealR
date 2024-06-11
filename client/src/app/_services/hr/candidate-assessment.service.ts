import { Injectable } from '@angular/core';
import { ReplaySubject, map, of, take } from 'rxjs';
import { CandidateAssessmentParams } from 'src/app/_models/params/hr/candidateAssessmentParams';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';
import { AccountService } from '../account.service';
import { ICandidateAssessment } from 'src/app/_models/hr/candidateAssessment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Pagination } from 'src/app/_models/pagination';
import { getPaginatedResult } from '../paginationHelper';
import { ICandidateAssessedDto } from 'src/app/_dtos/hr/candidateAssessedDto';
import { ICandidateAssessmentWithErrorStringDto } from 'src/app/_dtos/hr/candidateAssessmentWithErrorStringDto';
import { ICandidateAssessmentAndChecklistDto } from 'src/app/_dtos/hr/candidateAssessmentAndChecklistDto';
import { ICandidateAssessmentDto } from 'src/app/_dtos/hr/candidateAssessmentDto';

@Injectable({
  providedIn: 'root'
})
export class CandidateAssessmentService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  qParams = new  CandidateAssessmentParams();
  routeId: string='';
  user?: User;
  pagination: Pagination | undefined;
  cache = new Map();
  
  constructor(private accountService:AccountService,
    private http: HttpClient) {
      this.accountService.currentUser$.pipe(take(1))
        .subscribe(user => this.user = user!);
    }

    candidateAssessments(oParams: CandidateAssessmentParams) {

      const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = getPaginationHeaders(oParams);
  
      return getPaginatedResult<ICandidateAssessedDto[]>
          (this.apiUrl + 'CandidateAssessment/assessmentspaged', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )
     }


    updateCandidateAssessment(assessment: ICandidateAssessment) {
        return this.http.put<boolean>(this.apiUrl + 'CandidateAssessment/assessment', assessment);
      }
 
    saveNewCandidateAssessment(assessment: ICandidateAssessment) {
        return this.http.post<ICandidateAssessment>(this.apiUrl + 'CandidateAssessment/assessment', assessment);
    }

    deleteCandidateAssessment(assessmentId: number) {
      return this.http.delete<boolean>(this.apiUrl + 'CandidateAssessment/assessment/' + assessmentId);
    }

    getCandidateAssessment(candidateid: number, orderitemid: number) {

      return this.http.get<ICandidateAssessmentAndChecklistDto> (this.apiUrl + 
          'CandidateAssessment/assessmentandchecklist/' + candidateid + '/' + orderitemid).pipe(
        map(response => {
          return response;
        })
      )
    }

    getCandidateAssessmentById(candidateAssessmentId: number) {
      return this.http.get<ICandidateAssessment>(this.apiUrl + 'CandidateAssessment/assessmentById/' + candidateAssessmentId);
    }

    getCandidateAssessmentDto(candidateid: number, orderitemid: number) {

      return this.http.get<ICandidateAssessmentDto> (this.apiUrl + 
          'CandidateAssessment/candidateAssessmentDto/' + candidateid + '/' + orderitemid)
      
    }
    
    
    insertNewCVAssessment(requireReview: boolean, candidateId: number, orderItemId: number) {
      return this.http.post<ICandidateAssessmentWithErrorStringDto>(this.apiUrl + 
        'CandidateAssessment/assess/' + requireReview + '/' + candidateId + '/' + orderItemId, {});
    }

    
    deleteAssessment(assessmentid: number) {
      return this.http.delete<boolean>(this.apiUrl + 'candidateassessment/assessment/' + assessmentid);
    }

  
}

export function getPaginationHeaders(oParams: CandidateAssessmentParams) {
  let params = new HttpParams();

  params = params.append('pageNumber', oParams.pageNumber);
  params = params.append('pageSize', oParams.pageSize);
  params = params.append('orderId', oParams.orderId.toString());
  params = params.append('orderItemId', oParams.orderItemId.toString());
  params = params.append('candidateId', oParams.candidateId.toString());

  return params;
}
