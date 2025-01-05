import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { ChecklistHRDto, IChecklistHRDto } from 'src/app/_dtos/hr/checklistHRDto';
import { IChecklistHR } from 'src/app/_models/hr/checklistHR';
import { IChecklistAndCandidateAssessmentDto } from 'src/app/_dtos/hr/checklistAndCandidateAssessmentDto';
import { IRemunerationDto } from 'src/app/_dtos/admin/remunerationDto';

@Injectable({
  providedIn: 'root'
})
export class ChecklistService {

  apiUrl = environment.apiUrl;
  //private currentUserSource = new ReplaySubject<IUser>(1);
  //currentUser$ = this.currentUserSource.asObservable();
  cl?: IChecklistHRDto;
  
  private checklistSource = new BehaviorSubject<IChecklistHRDto|null>(null) ;
  checklist$ = this.checklistSource.asObservable();

  constructor(private http: HttpClient) { }

  getChecklist(candidateid: number, orderitemid: number) {
    return this.http.get<IChecklistHRDto>(this.apiUrl + 'checklist/generateorget/' + candidateid  + '/' + orderitemid);
  }

  getOrComposeChecklist(candidateid: number, orderitemid: number) {
    return this.http.get<IChecklistHRDto>(this.apiUrl + 'checklist/')
  }
  updateChecklist(checklist: IChecklistHR) {
    return this.http.put<string>(this.apiUrl + 'Checklist/checklisthr', checklist);
  }

  generateChecklistHR(candidateid: number, orderitemid: number) {
      return this.http.get<IChecklistHR>(this.apiUrl + 'Checklist/' + candidateid + '/' + orderitemid)
  }

  saveNewChecklistHR(checklisthr: IChecklistHR) {
    return this.http.post<IChecklistAndCandidateAssessmentDto>(this.apiUrl + 'CandidateAssessment/newchecklist', checklisthr);
  }
  
  deleteChecklistHR(checklistid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Checklist/checklist/' + checklistid);
  }

  getRemuneration(orderitemid: number) {
        return this.http.get<IRemunerationDto>(this.apiUrl + 'orders/remuneration/' + orderitemid);
  }
      
}
