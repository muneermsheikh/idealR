import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of, ReplaySubject } from 'rxjs';
import { Pagination } from 'src/app/_models/pagination';
import { visaParams } from 'src/app/_models/params/Admin/visaParams';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult } from '../paginationHelper';
import { IVisaBriefDto } from 'src/app/_dtos/admin/visaBriefDto';
import { IVisaHeader } from 'src/app/_models/admin/visaHeader';
import { IVisaItem } from 'src/app/_models/admin/visaItem';
import { IVisaTransaction } from 'src/app/_models/admin/visaTransaction';
import { ICustomerBriefDto } from 'src/app/_dtos/admin/customerBriefDto';
import { IVisaItemBriefDto } from 'src/app/_dtos/admin/visaItemBriefDto';
import { IOrderItemForVisaAssignmentDto } from 'src/app/_dtos/admin/orderItemForVisaAssignmentDto';
import { IVisaAssignment } from 'src/app/_models/admin/visaAssignment';

@Injectable({
  providedIn: 'root'
})

export class VisaService {

    apiUrl = environment.apiUrl;
    private currentUserSource = new ReplaySubject<User>(1);
    currentUser$ = this.currentUserSource.asObservable();
    vParams = new visaParams();
    pagination: Pagination | undefined; 
    cache = new Map();
    cacheT = new Map(); //for visa transactions
  
    constructor(private http: HttpClient) { }

    getPagedVisasBrief(vParams: visaParams)
    {
        const response = this.cache.get(Object.values(vParams).join('-'));
        if(response) return of(response);
    
        let params = getHttpParamsForVisas(vParams);
    
        return getPaginatedResult<IVisaBriefDto[]>(this.apiUrl + 
            'visas/PagedList', params, this.http).pipe(
          map(response => {
            this.cache.set(Object.values(vParams).join('-'), response);
            return response;
          })
        )
    }

    getPagedVisaTransactions(vParams: visaParams)
    {
        const response = this.cacheT.get(Object.values(vParams).join('-'));
        if(response) return of(response);
    
        let params = getHttpParamsForVisas(vParams);
    
        return getPaginatedResult<IVisaTransaction[]>(this.apiUrl + 
            'visas/TransactionsPagedList', params, this.http).pipe(
          map(response => {
            this.cacheT.set(Object.values(vParams).join('-'), response);
            return response;
          })
        )
    }
   
    getVisaHeader(visaId: number) {
      return this.http.get<IVisaHeader>(this.apiUrl + 'Visas/VisaHeader/' + visaId)
    }

    insertNewVisa(visaToAdd: IVisaHeader) {
      return this.http.post<IVisaHeader>(this.apiUrl + 'Visas/insertNewVisa', visaToAdd)
    }

    updateVisa(visaToEdit: IVisaHeader) {
      return this.http.put<IVisaHeader>(this.apiUrl + 'Visas/editVisa', visaToEdit)
    }

    insertNewVisaItem(visaItemToAdd: IVisaItem) {
      return this.http.post<IVisaItem>(this.apiUrl + 'Visas/insertNewVisaitem', visaItemToAdd)
    }
  
    deleteVisaItem(visaItemId: number) {
      return this.http.delete<boolean>(this.apiUrl + 'Visa/visaItem/' + visaItemId)
    }

    deleteVisa(visaId: number) {
      return this.http.delete<boolean>(this.apiUrl + 'Visas/visa/' + visaId)
    }

    insertNewVisaTransaction(vTransaction: IVisaTransaction) {
      return this.http.post<IVisaTransaction>(this.apiUrl + 'Visas/transaction', vTransaction)
    }

    editVisaTransaction(vTransaction: IVisaTransaction) {
      return this.http.put<IVisaTransaction>(this.apiUrl + 'Visas/editTransaction', vTransaction)
    }

    deleteVisaTransaction(id: number) {
      return this.http.delete<boolean>(this.apiUrl + 'Visas/ItemTransaction/' + id)
    }

    checkVisaExists(visano: string) {
      return this.http.get<string>(this.apiUrl + 'Visas/VisaNoExists/' + visano);
    }
  
    getCustomerList(custType: string) {         
        custType = custType ?? 'Customer';
        return this.http.get<ICustomerBriefDto[]>(this.apiUrl + 'customers/list/' + custType);        
    }

    getOpenVisasForCompany(customerId: number) {
      return this.http.get<IOrderItemForVisaAssignmentDto[]>(this.apiUrl + 'visas/OpenVisaItemsForcustomerId/' + customerId)
    }

    getOrderItemsForCustomer(customerId: number) {
      return this.http.get<IOrderItemForVisaAssignmentDto[]>(this.apiUrl + 'visas/OpenOrderItemsForCustomer/' + customerId);
    }

    assignVisaItemToOrderItem(dtos: IVisaAssignment[]) {
      return this.http.post<IVisaAssignment[]>(this.apiUrl + 'visas/assignVisas', dtos);
    }

    setParams(visaParams: visaParams) {
      this.vParams = visaParams;
    }

    getParams() {
      return this.vParams;
    }
}

export function getHttpParamsForVisas(vParams: visaParams) {

    let params = new HttpParams();

    params = params.append('pageNumber', vParams.pageNumber);
    params = params.append('pageSize', vParams.pageSize)
    var ExpYr = new Date(vParams.visaExpiryG);
    if (vParams.id !== 0) params = params.append('id', vParams.id.toString());
    if (vParams.visaNo !== '') params = params.append('visaNo', vParams.visaNo);
    if (vParams.customerId !== 0) params = params.append('customerId', vParams.customerId.toString());
    if (vParams.customerName !== '') params = params.append('customerName', vParams.customerName);
    if (vParams.visaExpiryH !== '') params = params.append('visaExpiryH', vParams.visaExpiryH);
    if (ExpYr.getFullYear() > 2000) params = params.append('visaExpiryG', 
      ExpYr.getFullYear()+'/'+(ExpYr.getMonth()+1)+'/'+ExpYr.getDate());
    if (vParams.visaSponsorName !== '') params = params.append('visaSponsorName', vParams.visaSponsorName);
    if (vParams.visaCategory !== '') params = params.append('visaCategory', vParams.visaCategory);
    if (vParams.visaConsulate !== '') params = params.append('visaConsulate', vParams.visaConsulate);
    if (vParams.visaCanceled === true) params = params.append('visaCanceled', true);
    if (vParams.depItemId !== 0) params = params.append('depItemId', vParams.depItemId.toString());
    if (vParams.visaApproved ===true) params = params.append('visaApproved', true);

    params = params.append('sort', vParams.sort);

    return params;
      
  }

