import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { ICustomerReview } from 'src/app/_models/admin/customerReview';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class CustomerReviewService {

  baseUrl = environment.apiUrl;
 
  constructor(private http: HttpClient) { }

  getCustomerReview(customerid: number){
    return this.http.get<ICustomerReview>(this.baseUrl + 'CustomerReview/customerreview/' + customerid);
  }

  getOrCreateCustomerReview(customerid: number) {
    return this.http.get<ICustomerReview>(this.baseUrl + 'CustomerReview/getOrCreateObject/' + customerid);
  }

  updateCustomerReview(model: ICustomerReview) {
    return this.http.put<boolean>(this.baseUrl + 'CustomerReview/customerreview', model)
  }

  postNewCustomerReview(model: ICustomerReview) {
    return this.http.post<boolean>(this.baseUrl + 'CustomerReview/insertnew', model);
  }

  approveCustomerReviewItem(reviewitemid: number) {
    return this.http.put<boolean>(this.baseUrl + 'CustomerReview/approveReviewItem/' + reviewitemid, {});
  }

  getCustomerReviewStatusData() {
    return this.http.get<string[]>(this.baseUrl + 'customerreview/customerReviewStatusData');
  }
}

