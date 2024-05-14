import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, map, of } from 'rxjs';
import { environment } from 'src/app/environments/environment';
import { IUser } from '../../models/admin/user';
import { orderParams } from '../../params/admin/orderParams';
import { IPagination } from '../../models/pagination';
import { IOrder, Order } from '../../models/admin/order';
import { IOrderCity } from '../../models/admin/orderCity';
import { ICustomerNameAndCity } from '../../models/admin/customernameandcity';
import { IProfession } from '../../models/masters/profession';
import { IOrderBriefDto } from '../../dtos/admin/orderBriefDto';
import { IOpenOrderItemCategoriesDto } from '../../dtos/admin/openOrderItemCategriesDto';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IOrderItemBriefDto } from '../../dtos/admin/orderItemBriefDto';
import { IJDDto } from '../../dtos/admin/jdDto';
import { IRemunerationDto } from '../../dtos/admin/remunerationDto';
import { idAndDate } from '../../params/admin/idAndDate';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  oParams = new orderParams();
  pagination?: IPagination<IOrder[]>;
  cities: IOrderCity[]=[];
  customers: ICustomerNameAndCity[]=[];
  professions: IProfession[]=[];
  cache = new Map();
  cacheBrief = new Map();
  paginationBrief?: IPagination<IOrderBriefDto[]>;
  bParams = new orderParams();

  openOrderItems?: IOpenOrderItemCategoriesDto[];

  constructor(private http: HttpClient) { }

    getOrdersBrief(useCache: boolean=true): Observable<IPagination<IOrderBriefDto[]>> { 

      if (useCache === false) this.cacheBrief = new Map();
      
      if (this.cacheBrief.size > 0 && useCache === true) {
        if (this.cacheBrief.has(Object.values(this.bParams).join('-'))) {
          this.paginationBrief!.data = this.cacheBrief.get(Object.values(this.bParams).join('-'));
          if(this.paginationBrief) return of(this.paginationBrief);
        }
      }

      let params = new HttpParams();
      if (this.bParams.city !== "") {
        params = params.append('cityOfWorking', this.bParams.city);
      }
    
      if (this.bParams.search) {
        params = params.append('search', this.bParams.search);
      }
      
      params = params.append('sort', this.bParams.sort);
      params = params.append('pageIndex', this.bParams.pageNumber.toString());
      params = params.append('pageSize', this.bParams.pageSize.toString());

      return this.http.get<IPagination<IOrderBriefDto[]>>(this.apiUrl + 'orders/ordersbriefpaginated', {params}).pipe(
          map(response => {
            this.cacheBrief.set(Object.values(this.oParams).join('-'), response);
            this.paginationBrief = response;
            return response;
          })
        )
      }

    getOrders(useCache: boolean): Observable<IPagination<Order[]>> { 

      if (useCache === false) this.cache = new Map();
      
      if (this.cache.size > 0 && useCache === true) {
        if (this.cache.has(Object.values(this.oParams).join('-'))) {
          this.pagination = this.cache.get(Object.values(this.oParams).join('-'));
          if(this.pagination) return of(this.pagination);
        }
      }

      let params = new HttpParams();

      if (this.oParams.city !== "") params = params.append('cityOfWorking', this.oParams.city);
      if (this.oParams.categoryId !== 0) params = params.append('categoryId', this.oParams.categoryId.toString());
      if (this.oParams.search) params = params.append('search', this.oParams.search);
      
      params = params.append('sort', this.oParams.sort);
      params = params.append('pageIndex', this.oParams.pageNumber.toString());
      params = params.append('pageSize', this.oParams.pageSize.toString());

      return this.http.get<IPagination<Order[]>>(this.apiUrl + 'orders/ordersbriefpaginated', 
        {params})
        .pipe(
          map(response => {
            this.cache.set(Object.values(this.oParams).join('-'), response);
            this.pagination = response;
            return response;
          })
        )
      }

    getOrderItemsBriefDto() {
      return this.http.get<IOrderItemBriefDto[]>(this.apiUrl + 'orders/openorderitemlist');
    }

    acknowledgeOrderToClient(orderid: number) {
      return this.http.get<boolean>(this.apiUrl + 'ordersget/ackToClient/' + orderid);
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

    getOrderItemIdsAssessedForACandidate(candidateid: number) {
      return this.http.get<number[]>(this.apiUrl + 'orders/referredtoorderitemids/' + candidateid);
    }
    
    getOrder(id: number) {
      return this.http.get<IOrder>(this.apiUrl + 'orders/byid/' + id);
    }
    
    getOrderBrief(id: number) {
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

    register(model: any) {
      return this.http.post<IOrder>(this.apiUrl + 'orders', model);  // also composes email msg to customer
      }
    
    UpdateOrder(model: any) {
      return this.http.put(this.apiUrl + 'orders', model)
    }
      
    setOParams(params: orderParams) {
      this.oParams = params;
    }
    
    getOParams() {
      return this.oParams;
    }

    
    getOrderCities() {
      if (this.cities.length > 0) {
        return of(this.cities);
      }
    
      return this.http.get<IOrderCity[]>(this.apiUrl + 'orders/ordercities' ).pipe(
        map(response => {
          this.cities = response;
          return response;
        })
      )
    }

    updateOrderWithDLFwdToHROn(orderid: number, dt: Date) {
      var obj = new idAndDate();
      obj.orderId=orderid;
      obj.dateForwarded=dt;
      return this.http.put(this.apiUrl + 'orders/updatedlfwd' , obj);
    }

    deleteOrder(orderid: number) {
      return this.http.delete<boolean>(this.apiUrl + 'orders')
    }

    remindClientForSelections(customerId: number) {
      return this.http.get<boolean>(this.apiUrl + 'CVRef/selDecisionReminder/' + customerId);
    }
}
