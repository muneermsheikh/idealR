import { Injectable } from '@angular/core';
import { ReplaySubject, take } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { assessmentParams } from 'src/app/_models/params/Admin/assessmentParam';
import { AccountService } from '../account.service';
import { IOrderAssessment } from 'src/app/_models/admin/orderAssessment';
import { IOrderAssessmentItem } from 'src/app/_models/admin/orderAssessmentItem';
import { IOrderAssessmentItemQ } from 'src/app/_models/admin/orderAssessmentItemQ';
import { IContractReviewItemDto } from 'src/app/_dtos/orders/contractReviewItemDto';
import { IAssessmentQBank } from 'src/app/_models/admin/assessmentQBank';

@Injectable({
  providedIn: 'root'
})
export class OrderAssessmentService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  qParams = new assessmentParams();
  routeId: string='';
  user?: User;
  
  constructor(private activatedRoute: ActivatedRoute, 
    private router: Router,
    private accountService:AccountService,
    private http: HttpClient) { 
      this.accountService.currentUser$.pipe(take(1))
        .subscribe(user => this.user = user!);
    }

    getOrderAssessmentItem(orderitemid: number) {
      return this.http.get<IOrderAssessmentItem>(this.apiUrl + 'OrderAssessment/orderassessmentitem/' + orderitemid);
    }
    
    getOrderAssessment(orderid: number) {
      return this.http.get<IOrderAssessment>(this.apiUrl + 'OrderAssessment/orderAssessment/' + orderid);
    }

    updateOrderAssessment(assessment: IOrderAssessment) {
      return this.http.put<string>(this.apiUrl + 'OrderAssessment/assessment', assessment);
    }

    deleteAssessmentQ(questionId: number) {
      return this.http.delete<boolean>(this.apiUrl + 'orderassessment/assessmentq/' + questionId);
    }

    deleteAssessment(orderitemid: number) {
      return this.http.delete<boolean>(this.apiUrl + 'orderassessment/assessment/' + orderitemid);
    }

    AddNewAssessment(assessment: IOrderAssessment) {
      return this.http.post<IOrderAssessment>(this.apiUrl + 'OrderAssessment/assessment', assessment);
    }
    
    getOrderAssessmentItemQs(orderitemid: number) {
      var item = this.http.get<IOrderAssessmentItemQ[]>(this.apiUrl + 'OrderAssessment/orderassessmentQs/' + orderitemid);
      return item;
    }

    //orderAssessmentItems
    
    updateOrderAssessmentItem(assessment: IOrderAssessmentItem) {
      return this.http.put<boolean>(this.apiUrl + 'OrderAssessment/assessmentitem', assessment);
    }

    deleteOrderAssessmentItem(assessmentItemId: number) {
      return this.http.delete<boolean>(this.apiUrl + 'OrderAssessment/assessmentitemq/' + assessmentItemId);
    }

    InsertOrderAssessmentItem(assessment: IOrderAssessmentItem) {
      return this.http.post<IOrderAssessmentItem>(this.apiUrl + 'OrderAssessment/assessmentitem', assessment);
    }

    //assessment questions
    getAssessmentQBankOfCategoryId(professionId: number) {

      return this.http.get<IAssessmentQBank>(this.apiUrl + 'AssessmentQBank/questionsFromQBank/' 
        + professionId);
    }

    getAssessmentQStandard() {
      return this.http.get<IOrderAssessmentItemQ[]>(this.apiUrl + 'OrderAssessment/assessmentStddQs');
    }
    
    getContractReviewItemDto(orderitemid: number) {
      return this.http.get<IContractReviewItemDto>(this.apiUrl + 'ContractReview/')
    }
}
