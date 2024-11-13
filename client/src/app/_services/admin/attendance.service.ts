import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of, ReplaySubject } from 'rxjs';
import { IInterviewAttendanceDto } from 'src/app/_dtos/admin/interviewAttendanceDto';
import { Pagination } from 'src/app/_models/pagination';
import { attendanceParams } from 'src/app/_models/params/Admin/attendanceParams';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult } from '../paginationHelper';
import { IInterviewAttendanceToUpdateDto } from 'src/app/_dtos/hr/interviewAttendanceToUpdateDto';
import { IIntervwAttendance } from 'src/app/_models/hr/intervwAttendance';
import { AppId } from 'src/app/_dtos/admin/appId';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  aParams = new attendanceParams();
  pagination: Pagination | undefined; 
  cache = new Map();
  
  constructor(private http: HttpClient) { }
    
  getAttendancePaged() { 

    const response = this.cache.get(Object.values(this.aParams).join('-'));
    if(response) return of(response);

    let params = new HttpParams();  // getHttpParamsForOrders(oParams);
    params = params.append('orderId', this.aParams.orderId.toString());
    params = params.append('pageNumber', this.aParams.pageNumber.toString());
    params = params.append('pageSize', this.aParams.pageSize.toString());

    return getPaginatedResult<IInterviewAttendanceDto[]>(this.apiUrl + 
        'interview/pagedAttendance', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(this.aParams).join('-'), response);
        return response;
      })
    )
  }

  updateAttendances(dtos: IInterviewAttendanceToUpdateDto[]) {
    return this.http.put<IInterviewAttendanceToUpdateDto[]>(this.apiUrl + 'interview/updateAttendance', dtos);
  }

  UploadInterviewerNote(model: any) {
    return this.http.post<string>(this.apiUrl + 'FileUpload/InterviewerNote', model);
  }

  downloadInterviewerNote(fullpath: string) {
    let params = new HttpParams();
    params = params.append('fullpath', fullpath);

    return this.http.get(this.apiUrl + 'FileUpload/downloadfile', {params, responseType: 'blob'});

  }

  getAttendanceStatus(candidateId: number) {
    return this.http.get<IIntervwAttendance[]>(this.apiUrl + 'interview/candidateattendances/' + candidateId);
  }

  composeInvitationMsgs(candidateIds: number[])
  {
    //console.log('candidateids:', candidateIds);
    return this.http.post<AppId[]>(this.apiUrl + 'Help/insertIds', candidateIds); 
  }

  deleteAttendance(InterviewItemCandidateId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'interview/deleteinterviewItemCandidate/' + InterviewItemCandidateId);
  }

  setAParams(params: attendanceParams) {
    this.aParams = params;
  }
  
  getAParams() {
    return this.aParams;
  }

}
