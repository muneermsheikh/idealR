import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { IOfficialAndCustomerNameDto } from 'src/app/_dtos/admin/client/oficialAndCustomerNameDto';
import { Pagination } from 'src/app/_models/pagination';
import { SelectOfficialParams } from 'src/app/_models/params/Admin/selectOfficialParams';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult } from '../paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class SelectCustomersService {

  apiUrl = environment.apiUrl;
  cParams = new SelectOfficialParams();
  pagination: Pagination | undefined;
  
  associates: IOfficialAndCustomerNameDto[]=[];

  cache = new Map();

  constructor(private http: HttpClient) {}

  getOfficialAndAgentName() {
    return this.http.get<IOfficialAndCustomerNameDto[]>(this.apiUrl + 'customers/officialidandcustomernames/associate');
  }

  getOfficialsAndCustomers() {
    
    const response = this.cache.get(Object.values(this.cParams).join('-'));
    if(response) return of(response);
  
    let params = new HttpParams();
    params = params.append('customerType', this.cParams.custType);
    params = params.append('pageNumber', this.cParams.pageNumber.toString());
    params = params.append('pageSize', this.cParams.pageSize.toString())
  
    return getPaginatedResult<IOfficialAndCustomerNameDto[]>(this.apiUrl + 
        'customers/officialidandcustomernames/' + this.cParams.custType, params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(this.cParams).join('-'), response);
        return response;
      })
    )
  }

   
  setParams(params: SelectOfficialParams) {
    this.cParams = params;
  }
  
  getParams() {
    return this.cParams;
  }


}
