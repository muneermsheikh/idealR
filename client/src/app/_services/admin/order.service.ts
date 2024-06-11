import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { orderParams } from 'src/app/_models/params/Admin/orderParams';
import { Pagination } from 'src/app/_models/pagination';
import { IOrderCity } from 'src/app/_models/admin/orderCity';
import { ICustomerNameAndCity } from 'src/app/_models/admin/customernameandcity';
import { IProfession } from 'src/app/_models/masters/profession';
import { IOpenOrderItemCategoriesDto } from 'src/app/_dtos/admin/openOrderItemCategriesDto';
import { getHttpParamsForOrderItems, getHttpParamsForOrders, getPaginatedResult } from '../paginationHelper';
import { IOrderBriefDto } from 'src/app/_dtos/admin/orderBriefDto';
import { IOrderItemBriefDto } from 'src/app/_dtos/admin/orderItemBriefDto';
import { IOrder } from 'src/app/_models/admin/order';
import { IJDDto } from 'src/app/_dtos/admin/jdDto';
import { IRemunerationDto } from 'src/app/_dtos/admin/remunerationDto';
import { OpenOrderItemsParams } from 'src/app/_models/params/Admin/openOrderItemsParams';
import { IOrderToCreateDto } from 'src/app/_dtos/orders/orderToCreateDto';

@Injectable({
  providedIn: 'root'
})

export class OrderService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  oParams = new orderParams();
  pagination: Pagination | undefined; 
  cities: IOrderCity[]=[];
  customers: ICustomerNameAndCity[]=[];
  professions: IProfession[]=[];
  cache = new Map();
  cacheItems = new Map();
 
  openOrderItems?: IOpenOrderItemCategoriesDto[];

  constructor(private http: HttpClient) { }

    getOrdersBrief(oParams: orderParams) { 

        const response = this.cache.get(Object.values(oParams).join('-'));
        if(response) return of(response);
    
        let params = getHttpParamsForOrders(oParams);
    
        return getPaginatedResult<IOrderBriefDto[]>(this.apiUrl + 
            'orders/pagedlist', params, this.http).pipe(
          map(response => {
            this.cache.set(Object.values(oParams).join('-'), response);
            return response;
          })
        )
    }

    getOpenOrderItemsBriefListDto() {
        return this.http.get<IOrderItemBriefDto[]>(this.apiUrl + 'Orders/openorderitemcategorylist');
    }

    getOrderItemsBriefDto(oParams: OpenOrderItemsParams) {
            
      const response = this.cacheItems.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = getHttpParamsForOrderItems(oParams);
  
      return getPaginatedResult<IOrderItemBriefDto[]>(this.apiUrl + 'Orders/openorderitemcategories', params, this.http).pipe(
        map(response => {
          this.cacheItems.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )
    }

    acknowledgeOrderToClient(orderid: number) {
      return this.http.get<boolean>(this.apiUrl + 'Orders/ackToClient/' + orderid);
    }

    getOpenOrderItemCategoriesDto() {
      if(this.openOrderItems !==undefined || this.openOrderItems !== null) return of(this.openOrderItems);
      //return this.http.get<IOpenOrderItemCategoriesDto[]>(this.apiUrl + 'OrdersGet/openorderitemcategorylist');
      
      return this.http.get<IOpenOrderItemCategoriesDto[]>(this.apiUrl + 'OrdersGet/openorderitemcategorylist', 
        {observe: 'response'})
        .pipe(
          map(response => {
            this.openOrderItems = response.body!;
            return response.body || undefined;
          })
        )
    }

    //why is following required
    getOrderItemIdsAssessedForACandidate(candidateid: number) {
      return this.http.get<number[]>(this.apiUrl + 'orders/referredtoorderitemids/' + candidateid);
    }
    
    getOrder(id: number) {
      return this.http.get<IOrder>(this.apiUrl + 'orders/orderWithItems/' + id);
    }
    
    //why is following required
    getOrderBrief(id: number) {
        const cand = [...this.cache.values()]
        .reduce((arr,elem) => arr.concat(elem.result), [])
        .find((cand: IOrderBriefDto) => cand.id === id);
      
        if(cand) {
          //console.log('returned from cache');
          return of(cand);
        }
        
        return this.http.get<IOrderBriefDto>(this.apiUrl + 'orders/orderbriefdto/' + id);
    }

    getJD(orderitemid: number) {
      return this.http.get<IJDDto>(this.apiUrl + 'orders/jd/' + orderitemid);
    }
    
    updateJD(model: any) {
      return this.http.put(this.apiUrl + 'orders/jd', model);

    }

    getRemuneration(orderitemid: number) {
      return this.http.get<IRemunerationDto>(this.apiUrl + 'orders/remuneration/' + orderitemid);
    }
    
    updateRemuneration(model: any) {
      return this.http.put(this.apiUrl + 'orders/remuneration', model);
    }

    deleteRemuneration(remunId: number) {
      return this.http.delete<boolean>(this.apiUrl + 'orders/' + remunId);
    }

    register(model: IOrderToCreateDto) {    //model: OrderToCreateDto
      return this.http.post<IOrder>(this.apiUrl + 'orders/neworder', model);  // also composes email msg to customer
      }
    
    UpdateOrder(model: IOrder) {
      return this.http.put<boolean>(this.apiUrl + 'orders/editorder', model)
    }
    
    deleteOrder(orderid: number) {
      return this.http.delete<boolean>(this.apiUrl + 'orders/deleteorder/' + orderid);
    }

    setOParams(params: orderParams) {
      this.oParams = params;
    }
    
    getOParams() {
      return this.oParams;
    }

}
