import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { orderItemParams } from 'src/app/_models/params/Admin/orderItemParams';
import { IOrderItemBriefDto } from 'src/app/_dtos/admin/orderItemBriefDto';

@Injectable({
  providedIn: 'root'
})
export class OrderitemsService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  oParams = new orderItemParams();
  cache = new Map();
  orderitems: IOrderItemBriefDto[]=[];


  constructor(private http: HttpClient) { }

    getOrderItems(orderid: number) {

      return this.http.get<IOrderItemBriefDto[]>(this.apiUrl + 'orders/orderWithItems/'+orderid)
        .pipe(
          map(response => {
            this.cache.set(Object.values(this.oParams).join('-'), response);
            this.orderitems = response;
            return response;
          })
        )
    }
  
    getOrderItem(orderitem: number) {
    let item: IOrderItemBriefDto|undefined;
    this.cache.forEach((items: IOrderItemBriefDto[]) => {
      item = items.find(p => p.orderItemId === orderitem);
    })
    
    if (item) return of(item);
    
    var oitem = this.http.get<IOrderItemBriefDto>(this.apiUrl + 'orders/itembrief/' + orderitem);
    return oitem;
  }

  getOrderItemRefCode(orderitemid: number) {
    
    return this.http.get<string>(this.apiUrl + 'orders/orderitemrefcode/' + orderitemid);

  }

}
