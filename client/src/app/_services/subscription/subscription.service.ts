import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.development';

interface SubscriptionStatus {
  subscriptionType: string;
  startDate: string;
  endDate: string;
  status: string;
  isActive: boolean;
}

interface IExtendTrialRequest {
  additionalDays: number;
  appUserId: number;
  vendorId: number;
}

interface ConvertToPaidRequest {
  transactionId: string;
}

@Injectable({
  providedIn: 'root'
})

export class SubscriptionService {

  baseUrl = environment.apiUrl;
  vendorId = environment.subscriptionUserId;
  appUserId = environment.subscriptionUserId;

  constructor(private http: HttpClient) {}

  getSubscriptionStatus(): Observable<SubscriptionStatus> {
    return this.http.get<SubscriptionStatus>(`${this.baseUrl}/status`);
  }

  extendTrial(additionalDays: number): Observable<any> {
    const request: IExtendTrialRequest = {additionalDays: additionalDays, vendorId: this.vendorId, appUserId: this.appUserId };
    return this.http.post(`${this.baseUrl}/extend-trial`, request);
  }

  convertToPaid(transactionId: string): Observable<any> {
    const request: ConvertToPaidRequest = { transactionId };
    return this.http.post(`${this.baseUrl}/convert-to-paid`, request);
  }
  
}
