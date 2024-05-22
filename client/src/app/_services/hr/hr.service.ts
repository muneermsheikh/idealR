import { Injectable } from '@angular/core';
import { ReplaySubject, map, of, take } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';
import { assessmentQBankParams } from 'src/app/_models/admin/assessmentQBankParams';
import { Pagination } from 'src/app/_models/pagination';
import { IProfession } from 'src/app/_models/masters/profession';
import { AccountService } from '../account.service';
import { HttpClient } from '@angular/common/http';
import { IAssessmentQBank } from 'src/app/_models/admin/assessmentQBank';
import { getPaginatedResult, getPaginationHeadersAssessmentQBankParams } from '../paginationHelper';
import { IChecklistHRDto } from 'src/app/_dtos/hr/checklistHRDto';

@Injectable({
  providedIn: 'root'
})
export class HrService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  qParams = new assessmentQBankParams();
  pagination: Pagination | undefined;
  existingCats: IProfession[]=[];
  routeId: string;
  user?: User;
  cache = new Map();

  constructor(private activatedRoute: ActivatedRoute, 
    private router: Router,
    private accountService:AccountService,
    private http: HttpClient) {
      this.routeId = this.activatedRoute.snapshot.params['id'];
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
    }
  
   
    getExistingProfFromQBank() {
      return this.http.get<IProfession[]>(this.apiUrl + 'AssessmentQBank/existingqbankprofs');
    }

    getAssessmentQBank() {
      return this.http.get<IAssessmentQBank[]>(this.apiUrl + 'assessmentqbank/assessmentbankqs');
    }

    getQBank(oParams: assessmentQBankParams) {
        const response = this.cache.get(Object.values(oParams).join('-'));
        if(response) return of(response);
    
        let params = getPaginationHeadersAssessmentQBankParams(oParams);
    
        return getPaginatedResult<IAssessmentQBank[]>(this.apiUrl + 
          'assessmentqbank/assessmentbankqs', params, this.http).pipe(
            map(response => {
              this.cache.set(Object.values(oParams).join('-'), response);
              return response;
            })
        )
    }

    getQs(categoryName: string) {
      return this.http.get<IAssessmentQBank>(this.apiUrl + 'assessmentQBank/catqs/' + categoryName);
    }

    getQBankByOrderItemId(orderitemid: number) {
      return this.http.get<IAssessmentQBank>(this.apiUrl + 'assessmentQBank/catqsbyorderitem');
    }

    
    setQParams(params: assessmentQBankParams) {
      this.qParams = params;
    }
    
    getQParams() {
      return this.qParams;
    }

    getQBankCategories() {
      if (this.existingCats.length > 0) return of(this.existingCats);

      return this.http.get<IProfession[]>(this.apiUrl + 'assessmentQBank/existingqbankcategories')
        .pipe(
          map(response => {
            this.existingCats = response;
            return response;
          })
        )
    }

    insertQBank(model: any) {
      return this.http.post(this.apiUrl + 'assessmentQBank', model);
    }

    updateQBank(model: any) {
      return this.http.put(this.apiUrl + 'assessmentQBank', model);
    }

    //checklist
    getChecklistHRDto(candidateid: number, orderitemid: number) {
      return this.http.get<IChecklistHRDto>(this.apiUrl + 'checklist/checklisthr/' + candidateid + '/' + orderitemid);
    }
}
