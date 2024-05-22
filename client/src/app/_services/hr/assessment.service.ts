import { Injectable } from '@angular/core';
import { ReplaySubject, take } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { assessmentParams } from 'src/app/_models/params/Admin/assessmentParam';
import { AccountService } from '../account.service';
import { IOrderAssessment } from 'src/app/_models/admin/orderAssessment';
import { ICandidateAssessment } from 'src/app/_models/hr/candidateAssessment';
import { IOrderItemAssessment } from 'src/app/_models/admin/orderItemAssessment';

@Injectable({
  providedIn: 'root'
})
export class AssessmentService {

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

    getOrderItemAssessment(orderitemid: number) {

      var item = this.http.get<IOrderItemAssessment>(this.apiUrl + 'OrderAssessment/orderitemassessment/' + orderitemid);
      return item;
    }
    
    getOrderAssessment(orderid: number) {
      return this.http.get<IOrderAssessment>(this.apiUrl + 'OrderAssessment/orderassessment/' + orderid);
    }

    /* getAssessmentQBankOfCategoryId(orderitemid: number, professionId: number) {
      console.log('orderitemid', orderitemid);
      return this.http.get<IAssessmentQ[]>(this.apiUrl + 'AssessmentQBank/catqsbycategoryid/' 
        + orderitemid + '/' + professionId);
    }
    */

    updateOrderAssessment(assessment: IOrderAssessment) {
      return this.http.put<boolean>(this.apiUrl + 'OrderAssessment/assessment', assessment);
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
}
