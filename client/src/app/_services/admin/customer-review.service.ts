import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { environment } from 'src/app/environments/environment';
import { ICustomerReview } from '../../models/admin/customerReview';
import { ICustomerReviewData } from '../../models/admin/customerReviewData';

@Injectable({
  providedIn: 'root'
})
export class CustomerReviewService {

  baseUrl = environment.apiUrl;
 
  constructor(private http: HttpClient) { }

  getCustomerReview(id: number){
    return this.http.get<ICustomerReview>(this.baseUrl + 'customerreview/' + id);
  }

  updateCustomerReview(model: any) {
    return this.http.put(this.baseUrl + 'CustomerReview', model)
  }

  getCustomerReviewStatusData() {
    return this.http.get<ICustomerReviewData[]>(this.baseUrl + 'customerreview/customerReviewData');
  }
}

