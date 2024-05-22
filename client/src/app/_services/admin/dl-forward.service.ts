import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IForwardedCategoryDto } from 'src/app/_dtos/admin/forwardedCategoryDto';
import { IOrderForwardToAgent } from 'src/app/_models/admin/orderForwardToAgent';
import { IOrderForwardToHR } from 'src/app/_models/orders/orderForwardToHR';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class DlForwardService {

  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

   //forward DL to agents
  forwardDLtoSelectedAgents(dlforward: IOrderForwardToAgent) {
    return this.http.post<string>(this.apiUrl + 'Orderforward', dlforward );
  }

  //get dlforwards of an orderid
  getDLForwardsOfAnOrderId(orderid: number) {
    return this.http.get<IOrderForwardToAgent[]>(this.apiUrl + 'Orderforward/byorderid/' + orderid );
  }

  getAssociatesForwardedForADL(orderid: number) {
    return this.http.get<IForwardedCategoryDto[]>(this.apiUrl + 'Orderforward/associatesforwardedForOrderId/' + orderid);
  }

  GenerateObjToForwarOrderToAgent(orderid: number) {
    return this.http.get<IOrderForwardToAgent>(this.apiUrl + 'OrderForward/generateOrderFwdToAgent/' + orderid, {});
  }


  UpdateForwarOrderToAgent(model: IOrderForwardToAgent) {
    return this.http.post<boolean>(this.apiUrl + 'OrderForward/updateOrderFwdToAgent', {model});
  }

  UpdateForwarOrderToHR(model: IOrderForwardToHR) {
    return this.http.post<boolean>(this.apiUrl + 'OrderForward/updateOrderFwdToHR', {model});
  }


  GenerateObjToForwardDLtoHR(orderid: number) {
    return this.http.get<IOrderForwardToHR>(this.apiUrl + 'OrderForward/generateOrderFwdToHR/' + orderid, {});
  }

}
