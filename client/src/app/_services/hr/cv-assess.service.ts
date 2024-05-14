import { Injectable } from '@angular/core';
import { environment } from 'src/app/environments/environment';
import { IChecklistHRDto } from '../../dtos/hr/checklistHRDto';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ICandidateAssessmentAndChecklist } from '../../models/hr/candidateAssessmentAndChecklist';
import { ICandidateAssessment } from '../../models/hr/candidateAssessment';
import { ICandidateAssessmentWithErrorStringDto } from '../../dtos/hr/candidateAssessmentWithErrorStringDto';

@Injectable({
  providedIn: 'root'
})

export class CvAssessService {

  apiUrl = environment.apiUrl;
  //private currentUserSource = new ReplaySubject<IUser>(1);
  //currentUser$ = this.currentUserSource.asObservable();
  cl?: IChecklistHRDto;
  
  private checklistSource = new BehaviorSubject<IChecklistHRDto|null>(null) ;
  checklist$ = this.checklistSource.asObservable();
 
  constructor(private http: HttpClient) { }

  getCVAssessmentAndChecklist(candidateId: number, orderItemId: number) {
    return this.http.get<ICandidateAssessmentAndChecklist>(this.apiUrl + 
        'CandidateAssessment/assessmentandchecklist/' + orderItemId + '/' + candidateId)
  }
 
  getChecklist(candidateid: number, orderitemid: number) {
    return this.http.get<IChecklistHRDto>(this.apiUrl + 'checklist/checklisthr/' + candidateid  + '/' + orderitemid);
  }

  updateChecklist(checklist: IChecklistHRDto) {
    //console.log(checklist);
    return this.http.put(this.apiUrl + 'checklist/checklisthr', checklist);
  }

  insertNewCVAssessmentHeader(requireReview: boolean, candidateId: number, orderItemId: number) {
    return this.http.post<ICandidateAssessmentWithErrorStringDto>(this.apiUrl + 
      'CandidateAssessment/assess/' + requireReview + '/' + candidateId + '/' + orderItemId, {});
  }

}
