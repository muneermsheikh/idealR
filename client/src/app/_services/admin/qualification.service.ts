import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { IQualification } from 'src/app/_models/hr/qualification';
import { Pagination } from 'src/app/_models/pagination';
import { paramsMasters } from 'src/app/_models/params/masters/paramsMasters';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class QualificationService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  mParams = new paramsMasters();
  
  constructor(private http: HttpClient) { }

  getQualificationList(){
    return this.http.get<IQualification[]>(this.apiUrl + 'masters/qualificationList');
  }

 
  getQualification(id: number) {
    
    return this.http.get<IQualification>(this.apiUrl + 'masters/qualification/' + id);
  }

  updateQualification(id: number, name: string) {
    var prof: IQualification = {id: id, name: name};
    return this.http.put<IQualification>(this.apiUrl + 'masters/editqualification', prof);
  }

  deleteQualification(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'masters/deletequalification/' + id);
  }
  
}
