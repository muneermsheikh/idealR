import { Injectable } from '@angular/core';
import { environment } from 'src/app/environments/environment';
import { IClientIdAndNameDto } from '../../dtos/admin/clientIdAndNameDto';
import { paramsCustomerCache } from '../../params/admin/paramsCustomerCache';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, of } from 'rxjs';
import { paramsCustomer } from '../../params/admin/paramsCustomer';
import { IPagination } from '../../models/pagination';
import { ICustomer } from '../../models/admin/customer';
import { ICustomerCity } from '../../models/admin/customerCity';
import { ICustomerOfficialDto } from '../../models/admin/customerOfficialDto';
import { ICustomerNameAndCity } from '../../models/admin/customernameandcity';

@Injectable({
  providedIn: 'root'
})
export class ClientService {

  baseUrl = environment.apiUrl;
  customerType: string = 'customer';
  customerCacheDto: IClientIdAndNameDto[]=[];
  cParams= new paramsCustomerCache();

  cache = new Map();

  constructor(private http: HttpClient) { }

  getClientIdAndNames() {

    if(this.customerCacheDto !== undefined && this.customerCacheDto.length > 0) return of(this.customerCacheDto);
    
    return this.http.get<IClientIdAndNameDto[]>(this.baseUrl + 'customers/clientidandnames');
  }

  setParams(prm: paramsCustomerCache) {
    this.cParams = prm;
  }

  getparams(): paramsCustomerCache {
    return this.cParams;
  }
  
  getCustomers(custParams: paramsCustomer) {
    let params = new HttpParams();
    if (custParams.customerCityName !== "") {
      params = params.append('customerCityName', custParams.customerCityName!);
    }
    if (custParams.customerIndustryId !== 0) {
      params = params.append('customerIndustryId', custParams.customerIndustryId!.toString());
    }

    if (custParams.search) {
      params = params.append('search', custParams.search);
    }

    this.customerType = custParams.customerType ?? "customer";
    params = params.append('customerType', this.customerType);

    
    params = params.append('sort', custParams.sort);
    
    params = params.append('pageIndex', custParams.pageNumber.toString());
    params = params.append('pageSize', custParams.pageSize.toString());
    
    return this.http.get<IPagination<ICustomer>>(this.baseUrl + 'customers', {observe: 'response', params})
      .pipe(
        map(response => {
          return response.body;
        })
      )
  }

  getCustomer(id: number){
    return this.http.get<ICustomer>(this.baseUrl + 'customers/byid/' + id);
  }

  getCustomerCities() {
    return this.http.get<ICustomerCity[]>(this.baseUrl + 'customers/customerCities/' + this.customerType);
  }

  createCustomer(model: ICustomer) {
    return this.http.post(this.baseUrl + 'customers', model);
  }

  updateCustomer(model: any) {
    return this.http.put(this.baseUrl + 'customers', model);
  }

  //associates
  getAgents() {
    return this.http.get<ICustomerOfficialDto[]>(this.baseUrl + 'customers/agentdetails');
  }

  getAgentIdAndNames() {
    return this.http.get<IClientIdAndNameDto[]>(this.baseUrl + 'customers/idandnames/associate');
  }

  //**todo - implement procedure in contoller**/
  getCustomerAndCities() {
    return this.http.get<ICustomerNameAndCity[]>(this.baseUrl + 'customers/idandnames/customer');
  }

  getCustomerAndCitiesCustomers() {
    return this.http.get<ICustomerNameAndCity[]>(this.baseUrl + 'customers/customerCities/customer');
  }

}
