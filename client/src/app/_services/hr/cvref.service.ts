import { Injectable } from '@angular/core';
import { ReplaySubject, map, of, take } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { ICVRefWithDepDto } from 'src/app/_dtos/admin/cvReferredDto';
import { CVRefParams } from 'src/app/_models/params/Admin/cvRefParams';
import { User } from 'src/app/_models/user';
import { Pagination } from 'src/app/_models/pagination';
import { AccountService } from '../account.service';
import { getPaginatedResult, getPaginationHeadersCVRefParams } from '../paginationHelper';
import { ICVRefDto } from 'src/app/_dtos/admin/cvRefDto';
import { ISelPendingDto } from 'src/app/_dtos/admin/selPendingDto';
import { createSelDecisionDto } from 'src/app/_dtos/admin/createSelDecisionDto';
import { messageWithError } from 'src/app/_dtos/admin/messageWithError';
import { ISelectionStatusDto } from 'src/app/_dtos/admin/selectionStatusDto';
import { IReturnStringsDto } from 'src/app/_dtos/admin/returnStringsDto';
import { IProspectiveHeaderDto } from 'src/app/_dtos/hr/prospectiveHeaderDto';


@Injectable({
  providedIn: 'root'
})
export class CvrefService {

  apiUrl = environment.apiUrl;
  user?: User;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  pagination: Pagination | undefined;
  paginationRef: Pagination | undefined;

  cache = new Map();
  cacheReferred = new Map();

  cvRefParams = new CVRefParams();

  selectionStatuses: ISelectionStatusDto[] =[
    { name: 'Selected'}, {name: 'Rejected-High Salary Expectation'}, {name: 'Rejected-Low Exp'}, {name: 'Rejected-irrelevant Exp'},
    {name: 'Rejected-other reasons'}
  ]

  constructor(private activatedRoute: ActivatedRoute, 
      private accountService: AccountService,
      private http: HttpClient) {
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
  }

  
    referCVs(cvassessmentids: number[]) {
      return this.http.post<IReturnStringsDto>(this.apiUrl + 'CVRef', cvassessmentids);
    }

    referredCVsPaginated(useCache: boolean = false) { 
      
      if(!useCache) {
        this.cacheReferred = new Map()
      } else {
        const response = this.cacheReferred.get(Object.values(this.cvRefParams).join('-'));
        if(response) return of(response)
      }
    
      let params = getPaginationHeadersCVRefParams(this.cvRefParams);
       
      return getPaginatedResult<ISelPendingDto[]>(this.apiUrl 
        + 'cvref/cvsreferred', params, this.http).pipe(
      map(response => {
        this.cacheReferred.set(Object.values(this.cvRefParams).join('-'), response);
        return response;
      })
    )
    
    }

    deleteCVRef(cvrefid: number) {
      return this.http.delete<boolean>(this.apiUrl + 'CVRef/deleteCVRef/' + cvrefid);
    }

    getCVRefWithDeploys(cvrefid: number) {
      return this.http.get<ICVRefWithDepDto>(this.apiUrl + 'CVRef/cvrefwithdeploys/' + cvrefid);
    }

    getCVRefWithCVRefId(cvrefid: number) {
      return this.http.get<ICVRefDto>(this.apiUrl + 'CVRef/cvref/' + cvrefid);
    }

    
    registerSelectionDecisions(selDecisions: createSelDecisionDto[]) {
      return this.http.post<messageWithError>(this.apiUrl + 'Selection', selDecisions);
    }

    getCVReferredOrderNosDto(status: string) {
      return this.http.get<IProspectiveHeaderDto[]>(this.apiUrl + 'CVRef/headers/' + status);
    }

  
    downloadCV(candidateId: number) {
      return this.http.get(this.apiUrl + 'FileUpload/downloadcv/' + candidateId, {
        reportProgress: true,
        //observe: 'events',
        responseType: 'blob',
      });
    }
    

  setParams(params: CVRefParams) { 
    this.cvRefParams = params;
  }
  getParams() {
    return this.cvRefParams;
  }
    
   
}
