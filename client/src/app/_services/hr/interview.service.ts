import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { interviewParams } from 'src/app/_models/params/Admin/interviewParams';
import { Pagination } from 'src/app/_models/pagination';
import { getPaginatedResult, getPaginationHeadersInterviewParams } from '../paginationHelper';
import { IInterviewBrief } from 'src/app/_models/hr/interviewBrief';
import { IInterview } from 'src/app/_models/hr/interview';
import { IInterviewItemDto } from 'src/app/_models/hr/interviewItemDto';
import { IIntervw } from 'src/app/_models/hr/intervw';
import { CvsMatchingProfAvailableDto } from 'src/app/_dtos/hr/cvsMatchingProfAvailableDto';
import { IIntervwItem } from 'src/app/_models/hr/intervwItem';
import { IInterviewItemWithErrDto } from 'src/app/_dtos/admin/interviewItemWithErrDto';
import { defaultInterviewAddress } from 'src/app/_dtos/hr/defaultInterviewAddress';
import { IInterviewMatchingCategoryDto } from 'src/app/_dtos/hr/interviewMatchingCategoryDto';
import { IIntervwItemCandidate } from 'src/app/_models/hr/intervwItemCandidate';

@Injectable({
  providedIn: 'root'
})
export class InterviewService {
  
    apiUrl = environment.apiUrl;
    //private currentUserSource = new ReplaySubject<IUser>(1);
    //currentUser$ = this.currentUserSource.asObservable();
    iParams = new interviewParams();

    pagination: Pagination | undefined;
    cache = new Map();

    constructor(private http: HttpClient) { }

    getInterviewsPaged(useCache: boolean=true) { 
  
      var oParams = this.iParams; 

      if(this.cache.size > 0 || !useCache) {
      //if(useCache) {
        const response = this.cache.get(Object.values(oParams).join('-'));
          if(response) return of(response);
      }

      let params = getPaginationHeadersInterviewParams(oParams);

      return getPaginatedResult<IInterviewBrief[]>(this.apiUrl 
        + 'interview/pagedlist', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        }))
    }

    getDefaultInterviewAddress() {
      return new defaultInterviewAddress();
    }
    getOrGenerateinterview(orderno: number) {
      return this.http.get<IInterview>(this.apiUrl + 'interview/getorgenerate/' + orderno);
    }

    getOrGenerateinterviewnew(orderno: number) {
      return this.http.get<IIntervw>(this.apiUrl + 'interview/getorgeneratenew/' + orderno);
    }

    getOrGenerateinterviewschedule(orderno: number) {
      return this.http.get<IIntervw>(this.apiUrl + 'interview/getorgenerateschedule/' + orderno);
    }


    getInterviewWithItemsById(id: number) {
      return this.http.get<IIntervw>(this.apiUrl + 'interview/interviewById/' + id);
    }
    
    saveNewInterview(model: IIntervw) {
      return this.http.post<IIntervw>(this.apiUrl + 'interview/savenew', model);
    }

    updateInterviewHeader(model: IIntervw) {
      return this.http.put<IIntervw>(this.apiUrl + 'interview/interviewheader', model);
    }

    updateInterview(model: IIntervw) {
      return this.http.put<IIntervw>(this.apiUrl + 'interview/Intervw', model);
    }

    updateInterviewItem(model: IIntervwItem) {
      return this.http.put<IIntervwItem>(this.apiUrl + 'interview/intervwitem', model);
    }

    editOrInsertInterviewItemWithFile(model: any) {
      return this.http.post<IInterviewItemWithErrDto>(this.apiUrl + 'FileUpload/interviewitem', model);
    }
    
    insertInterviewItem(model: IIntervwItem) {
      return this.http.post<IIntervwItem>(this.apiUrl + 'interview/intervwitem', model);
    }

    insertInterviewItemWithFile(model: any) {
      return this.http.post<IIntervwItem>(this.apiUrl + 'interview/intervwitemwithfiles', model);
    }

    downloadInterviewerCommentFile(fullpath: string) {
      let params = new HttpParams();
      params = params.append('fullpath', fullpath);

      return this.http.get(this.apiUrl + 'FileUpload/downloadfile', {params, responseType: 'blob'});

    }

    deleteInterview(id: number) {
      return this.http.delete<boolean>(this.apiUrl + 'interview/interview/' + id);
    }

    deleteInterviewItem(interviewitemid: number) {
      return this.http.delete<boolean>(this.apiUrl + 'interview/deleteinterviewItem/' + interviewitemid);
    }

    deleteInterviewItemCandidate(interviewitemcandidateid: number) {
      return this.http.delete<boolean>(this.apiUrl + 'interview/deleteinterviewitemcandidate/' + interviewitemcandidateid);
    }

    getInterviewItemCatAndCandidates(interviewItemId: number) {
      return this.http.get<IInterviewItemDto[]>(this.apiUrl + 'interview/catandcandidates/' + interviewItemId );
    }
    
    getMatchingCandidates(professionid: number) {
      return this.http.get<CvsMatchingProfAvailableDto[]>(this.apiUrl + 'users/candidatesmatchingprof/' + professionid);
    }
    
    getInterviewItemsMatchingCategory(categoryName: string[]) {
      return this.http.get<IInterviewMatchingCategoryDto[]>(this.apiUrl + 'interview/interviewCategoryMatching/' + categoryName.flat());
    }
    
    insertInterviewItemCandidate(model: IIntervwItemCandidate) {
      console.log('insert interview item cndidate', model);
      return this.http.put<boolean>(this.apiUrl + 'Interview/addCandidateToInterviewItem', model);
    }
  
    getParams(){
      return this.iParams;
    }

    setParams(p: interviewParams) {
      this.iParams = p;
    }

    
}


