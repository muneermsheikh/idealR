import { Injectable } from '@angular/core';
import { ReplaySubject, map, of, take } from 'rxjs';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { CandidateBriefDto, ICandidateBriefDto } from '../_dtos/admin/candidateBriefDto';
import { Pagination } from '../_models/pagination';
import { AccountService } from './account.service';
import { IOrderItemAssessmentQ } from '../_models/admin/orderItemAssessmentQ';
import { ICandidateAssessment } from '../_models/hr/candidateAssessment';
import { ICandidateAssessmentWithErrorStringDto } from '../_dtos/hr/candidateAssessmentWithErrorStringDto';
import { IChecklistHRDto } from '../_dtos/hr/checklistHRDto';
import { ICandidateAssessmentAndChecklist } from '../_models/hr/candidateAssessmentAndChecklist';
import { ICandidateAssessedDto } from '../_dtos/hr/candidateAssessedDto';
import { CVBriefParam } from '../_models/params/hr/cvBriefParam';
import { GetHttpParamsForCVRefBrief, getPaginatedResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class CVAssessService {

 
  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  user?: User;
  header?: HttpHeaders;
  cvBriefs: ICandidateBriefDto[]=[];
  cache = new Map();
  cvParams = new CVBriefParam();

  pagination: Pagination | undefined;   //<CandidateBriefDto>; // = new PaginationCandidateBrief();

  constructor(private http: HttpClient, private accountService: AccountService
    ) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
  }

 
  getCVBriefData(cvParams: CVBriefParam) {

    const response = this.cache.get(Object.values(cvParams).join('-'));
    if(response) return of(response);

    let params = GetHttpParamsForCVRefBrief(cvParams);

    return getPaginatedResult<CandidateBriefDto[]>(this.apiUrl + 'Candidate', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(cvParams).join('-'), response);
        return response;
      })
    )

  }


  updateCVAssessment(model: ICandidateAssessment) {
    return this.http.put<boolean>(this.apiUrl + 'CandidateAssessment/assessment', model);
  }

  getOrderItemAssessmentQs(orderitemid: number) {
    return this.http.get<IOrderItemAssessmentQ[]>(this.apiUrl + 'OrderAssessment/orderitemassessmentQ/' + orderitemid);
  }

  getCVAssessmentObject(requireReview: boolean, candidateid: number, orderitemid: number, dt: Date) {
    return this.http.get<ICandidateAssessment>(this.apiUrl + 'CandidateAssessment/assessobject/' +  requireReview + '/' + candidateid + '/' + orderitemid);
  }


  insertCVAssessment(model: any) {
    return this.http.post(this.apiUrl + 'candidateassessment/assess', model);
  }

  getCVAssessment(cvid: number, orderitemid: number) {
    return this.http.get<ICandidateAssessment>(this.apiUrl + 'candidateassessment/' + orderitemid + '/' + cvid);
  }

  GetOrCreateNewCheckist(cvid: number, orderitemid: number) {
      return this.http.get<IChecklistHRDto>(this.apiUrl + 'candidateassessment/checklist/' + orderitemid + '/' + cvid);
  }
  

  getCVAssessmentAndChecklist(cvid: number, orderitemid: number) {
    return this.http.get<ICandidateAssessmentAndChecklist>(this.apiUrl + 
        'candidateassessment/assessmentandchecklist/' + orderitemid + '/' + cvid);
  }


  getCVAssessmentsOfACandidate(cvid: number) {
    return this.http.get<ICandidateAssessedDto[]>(this.apiUrl + 'candidateassessment/assessmentsofcandidateid/' +cvid);
  }


  deleteAssessment(assessmentid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'candidateassessment/assess/' + assessmentid);
  }

  setCVParams(params: CVBriefParam) {
    this.cvParams = params;
  }

  getCVParams() {
    return this.cvParams;
  }

  getCVBrief() { 
    
    var brief = this.cvBriefs.filter(x => x.id===this.cvParams.candidateId)[0];
    //console.log('in cvassess serice, cvparams.candidateid', this.cvParams.candidateId,'brief:', brief);
    return brief;

  }

  setCVBriefData(cvbriefs: ICandidateBriefDto[]) {
    this.cvBriefs = cvbriefs;
    this.cache.set(Object.values(this.cvParams).join('-'), cvbriefs);
    this.pagination!.totalItems=this.cvBriefs.length;
  }

}
