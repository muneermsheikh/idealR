import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, map, of, take } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ICandidateCity } from '../_models/hr/candidateCity';
import { CandidateBriefDto, ICandidateBriefDto } from '../_dtos/admin/candidateBriefDto';
import { Pagination } from '../_models/pagination';
import { getHttpParamsForCandidate, getPaginatedResult } from './paginationHelper';
import { ICandidate } from '../_models/hr/candidate';
import { IApiReturnDto } from '../_dtos/admin/apiReturnDto';
import { IUserAttachment } from '../_models/hr/userAttachment';
import { candidateParams } from '../_models/params/hr/candidateParams';
import { AccountService } from './account.service';
import { ICandidateAndErrorStringDto } from '../_dtos/hr/candidateAndErrorStringDto';
@Injectable({
  providedIn: 'root'
})

export class CandidateService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  cvParams: candidateParams | undefined;

  pagination: Pagination | undefined;   // IPagination<ICandidateBriefDto[]>;
  paginationBrief: Pagination | undefined;    //<CandidateBriefDto[]>;
  cities: ICandidateCity[]=[];
  cache= new Map;
  cacheBrief = new Map;
  user?: User | null;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user)
          this.cvParams = new candidateParams();
          this.user = user;
      }
    })
  }
  
  async onClickLoadDocument() {
    // get a document from the Web API endpoint 'LoadDocument'
    return this.http.get<any>(this.apiUrl + 'candidates/loaddocument');
  }

  getCandidatesPaged(cvParams: candidateParams, fromCache: boolean = true): Observable<any> { 
   
    if(!fromCache) {
      this.cacheBrief = new Map();
    } else {
      const response = this.cacheBrief.get(Object.values(cvParams).join('-'));
      if(response) return of(response);
    }

    let params = getHttpParamsForCandidate(cvParams);

    return getPaginatedResult<CandidateBriefDto[]>(this.apiUrl + 
        'candidate', params, this.http).pipe(
      map(response => {
        this.cacheBrief.set(Object.values(cvParams).join('-'), response);
        return response;
      })
    )

  }

  getCandidate(id: number) {
    
    return this.http.get<ICandidate>(this.apiUrl + 'candidate/byid/'+ id);
  }

  getCandidateBrief(id: number) {
   
    const candidate = [...this.cacheBrief.values()]
      .reduce((arr,elem) => arr.concat(elem.result), [])
      .find((candidate: ICandidate) => candidate.id === id);
    
    if(candidate) return of(candidate);
        
    return this.http.get<ICandidate>(this.apiUrl + 'candidate/briefdtobyid/' + id);
  }

  getCandidateBriefDtoFromAppNo(appno: number) {
    let params = new HttpParams();
    params.append('applicationNo', appno.toString());

    return this.http.get<ICandidateBriefDto>(this.apiUrl + 
        'Candidate/briefbyparams', {params});
  }

  getCandidateBriefDtoFromId(candidateId: number) {
    
    var params = new HttpParams();
    params  = params.append('candidateId', candidateId.toString());
    return this.http.get<ICandidateBriefDto>(this.apiUrl + 'Candidate/briefbyparams', {params});
  }

  registerNewWithFiles(model: any) {
    return this.http.post<IApiReturnDto>(this.apiUrl + 'Candidate/RegisterByUpload', model );
    
  }


  UpdateCandidateWithFiles(model: any) {
      return this.http.put<ICandidateAndErrorStringDto>(this.apiUrl + 'candidate/updatecandidatewithfiles', model);
    }
    
  setCurrentUser(user: User) {
 
    localStorage.setItem('token', user.token);
    this.currentUserSource.next(user);
  }
  
  setCVParams(params: candidateParams) {
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
  
  /*submitCVsForReview(itemIds: number[], cvids: number[]) {
      
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
  */
  downloadAttachmentFile(attachmentid: number) {
    // get a document from the Web API endpoint 'LoadDocument'
    return this.http.get<any>(this.apiUrl + 'fileupload/downloadattachmentfile/' + attachmentid);
  }

  updateAttachments(model: IUserAttachment) {
    var attachments: IUserAttachment[]=[];
    attachments.push(model);

    return this.http.post<IUserAttachment[]>(this.apiUrl + 'Candidate/attachment', attachments);
  }

  getAttachments(candidateid: number) {
    return this.http.get<IUserAttachment[]>(this.apiUrl + 'Candidate/userattachments' + candidateid);
  }

  deleteAttachment(attachmentId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Candidate/userattachment/' + attachmentId);
  }

  setPhoto(model: IUserAttachment) {
    return this.http.put<boolean>(this.apiUrl + 'userattachment/setPhoto', model);
  }
}