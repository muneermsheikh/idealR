import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { environment } from 'src/app/environments/environment';
import { IUser } from '../../models/admin/user';
import { HttpClient } from '@angular/common/http';
import { IPendingDebitApprovalDto } from '../../dtos/finance/pendingDebitApprovalDto';
import { IUpdatePaymentConfirmationDto } from '../../dtos/finance/updatePaymentConfirmationDto';

@Injectable({
  providedIn: 'root'
})
export class ConfirmReceiptsService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  
  constructor(private http: HttpClient) { }


  getPendingConfirmations() {
    return this.http.get<IPendingDebitApprovalDto[]>(this.apiUrl + 'finance/paymentapprovalspending');
  }

  updatePaymentReceipts(confirmations: IUpdatePaymentConfirmationDto[]) {
    return this.http.put<boolean>(this.apiUrl + 'finance/confirmdebitapprovals', confirmations);
  }
}
