import { Injectable } from '@angular/core';
import { ReplaySubject, map, of, take } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { SelDecisionParams } from 'src/app/_models/params/Admin/selDecisionParams';
import { Pagination } from 'src/app/_models/pagination';
import { AccountService } from '../account.service';
import { ISelPendingDto } from 'src/app/_dtos/admin/selPendingDto';
import { getPaginatedResult, getPaginationHeadersSelectionParams} from '../paginationHelper';
import { createSelDecisionDto, selDecisionsToAddParams } from 'src/app/_dtos/admin/createSelDecisionDto';
import { ISelectionMsgsAndEmploymentsDto } from 'src/app/_dtos/admin/selectionMsgsAndEmploymentsDto';
import { ISelectionDecision } from 'src/app/_models/admin/selectionDecision';
import { IEmployment } from 'src/app/_models/admin/employment';
import { ISelDecisionDto } from 'src/app/_dtos/admin/selDecisionDto';
import { IOfferConclusioDto } from 'src/app/_dtos/admin/offerConclusionDto';

@Injectable({
  providedIn: 'root'
})
export class SelectionService {

  apiUrl = environment.apiUrl;
  user?: User;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  sParams = new SelDecisionParams();

  pagination: Pagination | undefined;

  cache = new Map();
  cacheSel = new Map();

  constructor(private activatedRoute: ActivatedRoute,
      private http: HttpClient, 
      private accountService: AccountService) {
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
          this.user = user!;
        })
  }

  setParams(params: SelDecisionParams) {
    this.sParams = params;
   }

   getParams(): SelDecisionParams {
     return this.sParams;
   }

 

  getPendingSelections(oParams: SelDecisionParams) {   //: Observable<IPagination<ISelPendingDto[]>> {

    const response = this.cache.get(Object.values(oParams).join('-'));
    if(response) return of(response);

    let params = getPaginationHeadersSelectionParams(oParams);
    
    return getPaginatedResult<ISelPendingDto[]>(this.apiUrl + 'selection/pendingselections', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(oParams).join('-'), response);
        return response;
      })
    )
 
  }


  registerSelectionDecisions(selDecisions: createSelDecisionDto) {
    return this.http.post<boolean>(this.apiUrl + 'Selection', selDecisions);
  }

  getSelectionDecisions(sParams: SelDecisionParams)
  {
    const response = this.cacheSel.get(Object.values(sParams).join('-'));
    if(response) return of(response);

    let params = getPaginationHeadersSelectionParams(sParams);

    return getPaginatedResult<ISelDecisionDto[]>(this.apiUrl 
      + 'Selection', params, this.http).pipe(
    map(response => {
      this.cacheSel.set(Object.values(sParams).join('-'), response);
      return response;
    }))

  }

  editSelectionDecision(seldecision: ISelectionDecision) {
    return this.http.put<boolean>(this.apiUrl + 'selection', seldecision);
  }

  deleteSelectionDecision(id: number) {
    
    return this.http.delete<boolean>(this.apiUrl + 'selection/' + id);
  }

  getEmployment(cvrefid: number) {
    return this.http.get<IEmployment>(this.apiUrl + 'selection/employmentbycvrefid/' + cvrefid);
  }

  getEmploymentFromEmploymentId(employmentid: number) {
    return this.http.get<IEmployment>(this.apiUrl + 'selection/employment/' + employmentid);
    
  }

  getEmploymentFromSelectionId(id: number) {
    return this.http.get<IEmployment>(this.apiUrl + 'selection/employmentfromSelId/' + id);
    
  }

  updateEmployment(emp: IEmployment) {
    return this.http.put<boolean>(this.apiUrl + 'selection/employment', emp);
  }

  getSelectionDtoByOrderNo(orderno: number) {
    
  }

  registerOfferAccepted(acceptedDto: IOfferConclusioDto[]) {
    return this.http.put<boolean>(this.apiUrl + 'selection/offeraccepted', acceptedDto);
  }

  

}
