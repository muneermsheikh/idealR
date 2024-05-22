import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { paramsCustomer } from 'src/app/_models/params/Admin/paramsCustomer';
import { Pagination } from 'src/app/_models/pagination';
import { ICustomerBriefDto } from 'src/app/_dtos/admin/customerBriefDto';
import { getHttpParamsForCustomers, getPaginatedResult} from '../paginationHelper';
import { ICustomer } from 'src/app/_models/admin/customer';
import { IRegisterCustomerDto } from 'src/app/_dtos/admin/registerCustomerDto';
import { ICustomerCity } from 'src/app/_models/admin/customerCity';
import { ICustomerOfficialDto } from 'src/app/_models/admin/customerOfficialDto';
import { IClientIdAndNameDto } from 'src/app/_dtos/admin/clientIdAndNameDto';
import { ICustomerNameAndCity } from 'src/app/_models/admin/customernameandcity';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  custParams = new paramsCustomer();
  pagination: Pagination | undefined;
  customerType='';
  customersBrief: ICustomerBriefDto[]=[];

  cache = new Map();
  
  constructor(private http: HttpClient) { }

  getCustomerNameFromId(id: number) {
    if(this.customersBrief.length > 0) {
      var custBrief = this.customersBrief.find(x => x.id ===id);
      if(custBrief !==null) return of(custBrief?.knownAs);
    }
    return this.http.get<string>(this.apiUrl + 'customers/customernamefromId/' + id);
  }

  getCustomers(oParams: paramsCustomer) { 

      const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
    
      this.customerType = oParams.customerType ?? "customer";

      let params = getHttpParamsForCustomers(oParams);
    
      return getPaginatedResult<ICustomerBriefDto[]>(this.apiUrl + 'customers', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )

  }

  
  getClientIdAndNames() {
    
    return this.http.get<IClientIdAndNameDto[]>(this.apiUrl + 'customers/clientidandnames');
  }

  
 getCustomer(id: number) {
    return this.http.get<ICustomer>(this.apiUrl + 'customers/byid/' + id);
  }

  register(model: IRegisterCustomerDto) {
    return this.http.post<ICustomer>(this.apiUrl + 'customers', model )
  }
  
  getCustomerCities(customerType: string) {
    return this.http.get<ICustomerCity[]>(this.apiUrl + 'customers/customercities/' + customerType);
  }
  
  
  setCustParams(params: paramsCustomer) {
    this.custParams = params;
  }
  
  getCustParams() {
    return this.custParams;
  }

  updateCustomer(model: any) {
    return this.http.put(this.apiUrl + 'customers/edit', model);
  }


  //associates
  getAgents() {
    return this.http.get<ICustomerOfficialDto[]>(this.apiUrl + 'customers/agentdetails/associate');
  }

  getAgentIdAndNames() {
    return this.http.get<IClientIdAndNameDto[]>(this.apiUrl + 'customers/clientidandnames/associate');
  }

  getCustomerAndCities() {
    return this.http.get<ICustomerNameAndCity[]>(this.apiUrl + 'customers/customerCities/customer');
  }

  
  remindClientForSelections(customerId: number) {
    return this.http.get<boolean>(this.apiUrl + 'CVRef/selDecisionReminder/' + customerId);
  }

}
