import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { IForwardedCategoryDto } from 'src/app/_dtos/admin/forwardedCategoryDto';
import { IOfficialIdsAndOrderItemIdsDto } from 'src/app/_dtos/admin/officialIdsAndOrderItemIdsDto';
import { IOrderForwardToAgentDto } from 'src/app/_dtos/orders/orderForwardToAgentDto';
import { IOrderForwardToHR } from 'src/app/_models/orders/orderForwardToHR';
import { Pagination } from 'src/app/_models/pagination';
import { OrderFwdParams } from 'src/app/_models/params/orders/orderFwdParams';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult } from '../paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class OrderForwardService {

  apiUrl = environment.apiUrl;

  orderFwds: IOrderForwardToAgentDto[]=[];
  fwdParams = new OrderFwdParams();

  cache = new Map();
  pagination: Pagination | undefined;

  constructor(private http: HttpClient) { }

  getForwardsBriefPaginated(oParams: OrderFwdParams) {
    
    const response = this.cache.get(Object.values(oParams).join('-'));
    if(response) return of(response);

    let params = getHttpParamsForOrders(oParams);

    return getPaginatedResult<IOrderForwardToAgentDto[]>(this.apiUrl + 
        'OrderForward/pagedlist', params, this.http).pipe(
          map(response => {
            this.cache.set(Object.values(oParams).join('-'), response);
            return response;
          })
        )
    
  }

   //forward DL to agents
  forwardDLtoSelectedAgents(ids: IOfficialIdsAndOrderItemIdsDto) {
    return this.http.post<string>(this.apiUrl + 'Orderforward', ids );
  }

  //get dlforwards of an orderid
  getOrderForwardOfAnOrder(orderid: number) {
    return this.http.get<IOrderForwardToAgentDto>(this.apiUrl + 'Orderforward/getOrGenerateOrderFwdToAgent/' + orderid );
  }

  getAssociatesForwardedForADL(orderid: number) {
    return this.http.get<IForwardedCategoryDto[]>(this.apiUrl + 'Orderforward/associatesforwardedForOrderId/' + orderid);
  }

  generateObjToForwarOrderToAgent(orderid: number) {
    return this.http.get<IOrderForwardToAgentDto>(this.apiUrl + 'OrderForward/generateOrderFwdToAgent/' + orderid, {});
  }

  insertForwarOrderToAgent(model: IOrderForwardToAgentDto) {
    return this.http.post<boolean>(this.apiUrl + 'OrderForward/insertOrderFwdToAgent', model);
  }

  updateForwarOrderToAgent(model: IOrderForwardToAgentDto) {
    return this.http.put<boolean>(this.apiUrl + 'OrderForward/updateOrderFwdToAgent', model);
  }

  forwarOrderToHR(orderid: number) {
    return this.http.post<boolean>(this.apiUrl + 'OrderForward/updateOrderFwdToHR/' + orderid, {});
  }


  generateObjToForwardDLtoHR(orderid: number) {
    return this.http.get<IOrderForwardToHR>(this.apiUrl + 'OrderForward/' + orderid, {});
  }

  setFwdParams(pParams: OrderFwdParams) {
    this.fwdParams = pParams;
  }

  getFwdParams() {
    return this.fwdParams;
  }

  deleteForward(orderid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'OrderForward/deleteOrderFwd/' + orderid);
  }

  deleteOrderFwdCategory(orderitemid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'OrderForward/deleteOrderFwdCategory/' + orderitemid);
  }

  deleteOrderFwdCatOfficial(offid: number) {
    return this.http.delete<boolean>(this.apiUrl + 'OrderForward/deleteOrderFwdCatOfficial/' + offid);
  }
} 

export function getHttpParamsForOrders(oParams: OrderFwdParams) {

  let params = new HttpParams();

  params = params.append('pageNumber', oParams.pageNumber);
  params = params.append('pageSize', oParams.pageSize)

  if(oParams.customerId !== 0) params = params.append('customerId', oParams.customerId.toString());
  if(oParams.orderId !== 0) params = params.append('orderId', oParams.orderId.toString());

  if(oParams.fwdDateFrom && oParams.fwdDateUpto ) {
    params = params.append('orderDateFrom', oParams.fwdDateFrom.toString());
    params = params.append('orderDateUpto', oParams.fwdDateUpto.toString());
  }

  if (oParams.search) params = params.append('search', oParams.search);
  params = params.append('sort', oParams.sort);

  return params;
    
}
