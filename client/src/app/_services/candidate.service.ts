import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { ICandidateCity } from '../_models/hr/candidateCity';
import { CandidateBriefDto, ICandidateBriefDto } from '../_dtos/admin/candidateBriefDto';
import { Pagination } from '../_models/pagination';
import { ParamsCandidate } from '../_models/params/hr/paramsCandidate';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { ICandidate } from '../_models/hr/candidate';
import { IApiReturnDto } from '../_dtos/admin/apiReturnDto';
import { ICVReviewDto, cvReviewDto } from '../_dtos/admin/cvReviewDto';
import { IUserAttachment } from '../_models/hr/userAttachment';
@Injectable({
  providedIn: 'root'
})
export class CandidateService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  cvParams: ParamsCandidate | undefined;
  pagination: Pagination | undefined;   // IPagination<ICandidateBriefDto[]>;
  paginationBrief: Pagination | undefined;    //<CandidateBriefDto[]>;
  cities: ICandidateCity[]=[];
  cache= new Map;
  cacheBrief = new Map;

  constructor(private http: HttpClient, private toastr: ToastrService, private router: Router) {}
  
  async onClickLoadDocument() {
    // get a document from the Web API endpoint 'LoadDocument'
    return this.http.get<any>(this.apiUrl + 'candidates/loaddocument');
  }

  getCandidates(cvParams: ParamsCandidate): Observable<any> { 
   
    const response = this.cacheBrief.get(Object.values(cvParams).join('-'));
    if(response) return of(response);

    let params = getPaginationHeaders(cvParams.pageNumber, cvParams.pageSize);

    if(cvParams.agentId !== 0) params = params.append('agendId', cvParams.agentId.toString());
    if(cvParams.city !== '') params = params.append('city', cvParams.city);
    if(cvParams.professionId !== 0) params = params.append('professionId', cvParams.professionId.toString());
    if(cvParams.search !== '') params = params.append('search', cvParams.search);
    if(cvParams.sort !== '') params = params.append('sort', cvParams.sort);

    return getPaginatedResult<CandidateBriefDto[]>(this.apiUrl + 
        'candidate/candidatepages', params, this.http).pipe(
      map(response => {
        this.cacheBrief.set(Object.values(cvParams).join('-'), response);
        return response;
      })
    )

  }

  checkEmailExists(email: string) {
    return this.http.get(this.apiUrl + 'account/emailexists?email=' + email);
  }

  checkPPExists(ppnumber: string) {
    return this.http.get(this.apiUrl + 'account/ppexists?ppnumber=' + ppnumber);
  }
  
  getCandidate(id: number) {
    return this.http.get<ICandidate>(this.apiUrl + 'candidate/byid/' + id);
  }

  getCandidateBrief(id: number) {
   
    const candidate = [...this.cacheBrief.values()]
      .reduce((arr,elem) => arr.concat(elem.result), [])
      .find((candidate: ICandidate) => candidate.id === id);
    
    if(candidate) {
      console.log('returned from cache');
      return of(candidate);
    }
    
    return this.http.get<ICandidate>(this.apiUrl + 'candidate/briefbyid/' + '/' + id);
  }

  getCandidateBriefDtoFromAppNo(id: number) {
    return this.http.get<ICandidateBriefDto>(this.apiUrl + 'candidate/byappno/' + id);
  }

  registerWithFiles(model: any) {
    return this.http.post<IApiReturnDto>(this.apiUrl + 'account/RegisterNewCandidate', model );
    
  }


  UpdateCandidateWithFiles(model: any) {
      return this.http.put<string>(this.apiUrl + 'candidate/updatecandidatewithfiles', model);
    }
    
  setCurrentUser(user: User) {
 
    localStorage.setItem('token', user.token);
    this.currentUserSource.next(user);
  }
  
  setCVParams(params: ParamsCandidate) {
    this.cvParams = params;
  }
  
  getCVParams() {
    return this.cvParams;
  }

  getCandidateCities() {
    if (this.cities.length > 0) {
      return of(this.cities);
    }
  
    return this.http.get<ICandidateCity[]>(this.apiUrl + 'candidate/cities' ).pipe(
      map(response => {
        this.cities = response;
        return response;
      })
    )
  }
  
  submitCVsForReview(itemIds: number[], cvids: number[]) {
      
      if (itemIds.length === 0 || cvids.length ===0) {
        this.toastr.warning("candidate Ids or item Ids data not provided");
        return;
      }

      var cvrvws: ICVReviewDto[]=[];
      cvids.forEach(c => {
        itemIds.forEach(i => {
          var cvrvw = new cvReviewDto();  
          cvrvw.candidateId=c;
          cvrvw.orderItemId=i;
          cvrvw.execRemarks='';
          cvrvws.push(cvrvw);
        })
      })

      return this.http.post(this.apiUrl + 'cvreviews', cvrvws);
  }
  viewDocument(id: number) {
    // get a document from the Web API endpoint 'LoadDocument'
    return this.http.get<any>(this.apiUrl + 'fileupload/viewdocument/' + id);
  }

  updateAttachments(model: IUserAttachment) {
    return this.http.post<IUserAttachment>(this.apiUrl + 'userattachment/attachment', model);
  }

  getAttachment(candidateid: number) {
    return this.http.get<IUserAttachment[]>(this.apiUrl + 'userattachment/' + candidateid);
  }

  deleteAttachment(attachmentId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'userattachment/' + attachmentId);
  }

  setPhoto(model: IUserAttachment) {
    return this.http.put<boolean>(this.apiUrl + 'userattachment/setPhoto', model);
  }
}
