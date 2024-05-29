import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { ChecklistHRDto, IChecklistHRDto } from 'src/app/_dtos/hr/checklistHRDto';
import { IChecklistHR } from 'src/app/_models/hr/checklistHR';

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
  updateChecklist(checklist: IChecklistHRDto) {
    return this.http.put(this.apiUrl + 'Checklist/checklisthr', checklist);
  }

  generateChecklistHR(candidateid: number, orderitemid: number) {
      return this.http.get<IChecklistHR>(this.apiUrl + 'Checklist/' + candidateid + '/' + orderitemid)
  }

  saveNewChecklistHR(checklisthr: IChecklistHR) {
    return this.http.post<IChecklistHR>(this.apiUrl + 'Checklist', checklisthr);
  }
  
  deleteChecklistHR(checklistid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Checklist/checklist' + checklistid);
  }
}
