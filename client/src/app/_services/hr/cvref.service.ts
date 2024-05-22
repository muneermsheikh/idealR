import { Injectable } from '@angular/core';
import { ReplaySubject, map, of, take } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { ICVRefWithDepDto } from 'src/app/_dtos/admin/cvReferredDto';
import { IMessagesDto } from 'src/app/_dtos/admin/messagesDto';
import { CVRefParams } from 'src/app/_models/params/Admin/cvRefParams';
import { User } from 'src/app/_models/user';
import { Pagination } from 'src/app/_models/pagination';
import { AccountService } from '../account.service';
import { getPaginatedResult, getPaginationHeadersCVRefParams } from '../paginationHelper';
import { ICVRefDto } from 'src/app/_dtos/admin/cvRefDto';


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

  constructor(private activatedRoute: ActivatedRoute, 
      private accountService: AccountService,
      private http: HttpClient) {
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
  }

  
    referCVs(cvassessmentids: number[]) {
      return this.http.post<IMessagesDto>(this.apiUrl + 'CVRef', cvassessmentids);
    }

    referredCVs(oParams: CVRefParams) { 
   
      const response = this.cacheReferred.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = getPaginationHeadersCVRefParams(oParams);
       
      return getPaginatedResult<ICVRefDto[]>(this.apiUrl 
        + 'cvref/cvsreferred', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(oParams).join('-'), response);
        return response;
      })
    )
    
  }

  getCVRefWithDeploys(cvrefid: number) {
    return this.http.get<ICVRefWithDepDto>(this.apiUrl + 'CVRef/cvrefwithdeploys/' + cvrefid);
  }

  getCVRefWithCVRefId(cvrefid: number) {
    return this.http.get<ICVRefDto>(this.apiUrl + 'CVRef/cvref/' + cvrefid);
  }

  setCVRefParams(params: CVRefParams) {
    this.cvRefParams = params;
  }
  getCVRefParams() {
    return this.cvRefParams;
  }
    
   
}
