import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { paramsMasters } from '../_models/params/masters/paramsMasters';
import { IEmployeeIdAndKnownAs } from '../_models/admin/employeeIdAndKnownAs';
import { IProfession } from '../_models/masters/profession';
import { ICustomerNameAndCity } from '../_models/admin/customernameandcity';
import { IVendorFacility } from '../_models/admin/vendorFacility';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  mParams = new paramsMasters();
  
  professions: IProfession[]=[];
  
  agents: ICustomerNameAndCity[]=[];
  //customers: ICustomerNameAndCity[]=[];

  cache = new Map();
  
  constructor(private http: HttpClient) { }
  
  getCategoryList() {
      return this.http.get<IProfession[]>(this.apiUrl + 'masters/categories', {});
  }

  getVendorFacilityList() {
    return this.http.get<IVendorFacility[]>(this.apiUrl + 'masters/VendorFacilityList')
  }

  getCategoriesPaged(mParams: paramsMasters) { 

        const response = this.cache.get(Object.values(mParams).join('-'));
        if(response) return of(response);
    
        let params = getPaginationHeaders(mParams.pageNumber, mParams.pageSize);
        if (this.mParams.name !== '') params = params.append('name', this.mParams.name!);
        if (this.mParams.id !== 0) params = params.append('id', this.mParams.id!.toString());
        if (this.mParams.search) params = params.append('search', this.mParams.search);
          
        params = params.append('pageIndex', this.mParams.pageNumber.toString());
        params = params.append('pageSize', this.mParams.pageSize.toString());
        
        return getPaginatedResult<IProfession[]>(this.apiUrl + 'masters/cpaginated', params, this.http).pipe(
          map(response => {
            this.cache.set(Object.values(mParams).join('-'), response);
            return response;
          })
        )
  }

  
  getCategory(id: number) {
    let category: IProfession;
    this.cache.forEach((categories: IProfession[]) => {
      category = categories.find(p => p.id === id)!;
    })

    if (category!) {
      return of(category);
    }

    return this.http.get<IProfession>(this.apiUrl + 'masters/category/' + id);
  }

  deleteCategory(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'masters/deletecategory/' + id);
  }

  updateCategory(id: number, name: string) {
    var prof: IProfession = {id: id, name: name};
    return this.http.put<IProfession>(this.apiUrl + 'masters/editcategory', prof);
  }
  
    
  getEmployeeIdAndKnownAs() {
    return this.http.get<IEmployeeIdAndKnownAs[]>(this.apiUrl + 'employees/idandknownas');
  }
  getAgents() {
    if (this.agents.length > 0) {
      return of(this.agents);
    }
    
    return this.http.get<ICustomerNameAndCity[]>(this.apiUrl + 'customers/idandnames/associate').pipe(
      map(response => {
        this.agents = response;
        return response;
      })
    );
  }

  setParams(params: paramsMasters) {
    this.mParams = params;
  }
  
  getParams() {
    return this.mParams;
  }

  
}
