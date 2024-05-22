import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { IQualification } from 'src/app/_models/hr/qualification';
import { qualificationParams } from 'src/app/_models/params/Admin/qualificationParams';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class QualificationService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  mParams = new qualificationParams();
  
  constructor(private http: HttpClient) { }

  getQualificationList(){
    return this.http.get<IQualification[]>(this.apiUrl + 'Qualification/qualificatinlist');
  }

 
  getQualification(id: number) {
    
    return this.http.get<IQualification>(this.apiUrl + 'Qualification/qById/' + id);
  }

  updateQualification(id: number, name: string) {
    var prof: IQualification = {id: id, name: name};
    return this.http.put<IQualification>(this.apiUrl + 'Qualification/edit', prof);
  }

  deleteQualification(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Qualification/delete/' + id);
  }
  
}
