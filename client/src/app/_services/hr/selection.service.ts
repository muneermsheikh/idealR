import { Injectable } from '@angular/core';
import { ReplaySubject, map, of, take } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { SelDecisionParams } from 'src/app/_models/params/Admin/selDecisionParams';
import { Pagination } from 'src/app/_models/pagination';
import { AccountService } from '../account.service';
import { ISelPendingDto } from 'src/app/_dtos/admin/selPendingDto';
import { getPaginatedResult, getPaginationHeadersSelectionParams} from '../paginationHelper';
import { createSelDecisionDto } from 'src/app/_dtos/admin/createSelDecisionDto';
import { ISelectionDecision } from 'src/app/_models/admin/selectionDecision';
import { IEmployment } from 'src/app/_models/admin/employment';
import { ISelDecisionDto } from 'src/app/_dtos/admin/selDecisionDto';
import { IOfferConclusioDto } from 'src/app/_dtos/admin/offerConclusionDto';
import { messageWithError } from 'src/app/_dtos/admin/messageWithError';

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

  constructor(private http: HttpClient, 
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

  getPendingSelections(useCache: boolean=true) {   //: Observable<IPagination<ISelPendingDto[]>> {

    var oParams = this.sParams;

    if(useCache) {
      const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
    }
    console.log('params in service:', oParams);
    let params = getPaginationHeadersSelectionParams(oParams);

    return getPaginatedResult<ISelPendingDto[]>(this.apiUrl + 'CVRef/cvsreferred', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(oParams).join('-'), response);
        return response;
      })
    )
 
  }


 /* registerSelectionDecisions(selDecisions: createSelDecisionDto[]) {
    return this.http.post<messageWithError>(this.apiUrl + 'Selection', selDecisions);
  }
*/

  getSelectionRecords(useCache: boolean=true)
  { 
    var sParams = this.sParams;

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

  deleteSelectionDecisions(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'selection/'+ id);
  }

  getEmployment(cvrefid: number) {
    return this.http.get<IEmployment>(this.apiUrl + 'selection/employmentbycvrefid/' + cvrefid);
  }

  getEmploymentFromEmploymentId(employmentid: number) {
    return this.http.get<IEmployment>(this.apiUrl + 'selection/employment/' + employmentid);
    
  }

  getEmploymentDtoFromSelectionId(id: number) {
    return this.http.get<IEmployment>(this.apiUrl + 'selection/employmentfromSelId/' + id);
  }

  getorGenerateEmploymentFromSelDecisionId(id: number) {
    return this.http.get<IEmployment>(this.apiUrl + 'employment/employmentfromSelId/' + id);
  }

  updateEmploymentWithUploads(emp: any) {
    return this.http.post<string>(this.apiUrl + 'employment/savewithupload', emp);
  }


  getSelectionDtoByOrderNo(orderno: number) {
    
  }

  registerOfferAccepted(acceptedDto: IOfferConclusioDto[]) {
    return this.http.put<boolean>(this.apiUrl + 'selection/offeraccepted', acceptedDto);
  }

  getSelectionBySelDecisionId(selDecisionId: any) {
    return this.http.get<ISelectionDecision>(this.apiUrl + 'selection/selectionBySelDecisionId/' + selDecisionId);
  }

  remindCandidatesForAcceptance(cvrefids: number[]) {
    return this.http.post<string>(this.apiUrl + 'selection/acceptancereminders', cvrefids);
  }

  
  Housekeeping() {
    return this.http.get<string>(this.apiUrl + 'selection/housekeepingcvrefandsel');
  }

}
