import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, map, of } from 'rxjs';
import { environment } from 'src/app/environments/environment';
import { IUser } from '../../models/admin/user';
import { IPagination } from '../../models/pagination';
import { ICustomerBriefDto } from '../../dtos/admin/customerBriefDto';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ICustomer } from '../../models/admin/customer';
import { IRegisterCustomerDto } from '../../dtos/admin/registerCustomerDto';
import { ICustomerCity } from '../../models/admin/customerCity';
import { ICustomerOfficialDto } from '../../models/admin/customerOfficialDto';
import { IClientIdAndNameDto } from '../../dtos/admin/clientIdAndNameDto';
import { paramsCustomer } from '../../params/admin/paramsCustomer';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  custParams = new paramsCustomer();
  pagination?: IPagination<ICustomerBriefDto[]>;
  //paginationBrief?: IPagination<ICustomerBriefDto[]>;
  customersBrief: ICustomerBriefDto[]=[];

  cache = new Map<string, IPagination<ICustomerBriefDto[]>>() // new Map();
  
  constructor(private http: HttpClient) { }

  getCustomerNameFromId(id: number) {
    if(this.customersBrief.length > 0) {
      var custBrief = this.customersBrief.find(x => x.id ===id);
      if(custBrief !==null) return of(custBrief?.knownAs);
    }
    console.log('geting custome rbrief from api');
    return this.http.get<string>(this.apiUrl + 'customers/customernamefromId/' + id);
  }

  getCustomers(useCache: boolean): Observable<IPagination<ICustomerBriefDto[]>> { 

    if (useCache === false)  this.cache = new Map();
    
    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.custParams).join('-'))) {
        this.pagination = this.cache.get(Object.values(this.custParams).join('-'))!;
        if(this.pagination) return of(this.pagination);
      }
    }

    let params = new HttpParams();
    if (this.custParams.customerType !== "") params = params.append('customerType', this.custParams.customerType);
    if (this.custParams.customerCityName !== '') params = params.append('customerCityName', this.custParams.customerCityName!);
    if (this.custParams.customerIndustryId !== 0) params = params.append('customerIndustryId', this.custParams.customerIndustryId!.toString());
    if (this.custParams.search) params = params.append('search', this.custParams.search);
    
    params = params.append('sort', this.custParams.sort);
    params = params.append('pageIndex', this.custParams.pageNumber.toString());
    params = params.append('pageSize', this.custParams.pageSize.toString());

    return this.http.get<IPagination<ICustomerBriefDto[]>>(this.apiUrl + 
        'customers/customersBrief', {params}).pipe(
      map(response => {
        this.cache.set(Object.values(this.custParams).join('-'), response)
        this.pagination = response;
        return response;
      })
    )
  }

  getCustomer(id: number) {
    return this.http.get<ICustomer>(this.apiUrl + 'customers/byid/' + id);
  }

  register(model: IRegisterCustomerDto) {
    return this.http.post<ICustomer>(this.apiUrl + 'customers', model )
  }
  
  getCustomerCities(customerType: string) {
    return this.http.get<ICustomerCity[]>(this.apiUrl + 'customers/customerCities/' + customerType);
  }
  
  
  setCustParams(params: paramsCustomer) {
    this.custParams = params;
  }
  
  getCustParams() {
    return this.custParams;
  }

  updateCustomer(model: any) {
    console.log('model in updatecustomer', model);
    return this.http.put(this.apiUrl + 'customers', model);
  }


  //associates
  getAgents() {
    return this.http.get<ICustomerOfficialDto[]>(this.apiUrl + 'customers/agentdetails');
  }


  getAgentIdAndNames() {
    return this.http.get<IClientIdAndNameDto[]>(this.apiUrl + 'customers/idandnames/associate');
  }
}
