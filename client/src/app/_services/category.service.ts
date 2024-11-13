import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { IProfession } from '../_models/masters/profession';
import { IVendorFacility } from '../_models/admin/vendorFacility';
import { getHttpParamsForProfession, getPaginatedResult } from './paginationHelper';
import { User } from '../_models/user';
import { professionParams } from '../_models/params/masters/ProfessionParams';

@Injectable({
  providedIn: 'root'
})

export class CategoryService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  mParams = new professionParams();
  
  professions: IProfession[]=[];
  

  //customers: ICustomerNameAndCity[]=[];

  cache = new Map();
  
  constructor(private http: HttpClient) { }
  
  getCategoryList() {
      return this.http.get<IProfession[]>(this.apiUrl + 'Profession/professionlist');
  }

  getVendorFacilityList() {
    return this.http.get<IVendorFacility[]>(this.apiUrl + 'masters/VendorFacilityList')
  }

  getCategoriesPaged(aParams: professionParams, fromCache: boolean=true): Observable<any> { 

    //var aParams = this.mParams;
    console.log('params:', aParams);
    if(!fromCache) {
      this.cache = new Map();
    } else {
      console.log('categories getting from cache');
      const response = this.cache.get(Object.values(aParams).join('-'));
      if(response) return of(response);
    }

    console.log('categories getting from api');
    
      let params = getHttpParamsForProfession(aParams);
      
      return getPaginatedResult<IProfession[]>(this.apiUrl + 
        'Profession', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(aParams).join('-'), response);
          return response;
        })
      )
  }

  
  getCategory(professionid: number) {
    let category: IProfession;
    this.cache.forEach((categories: IProfession[]) => {
      category = categories.find(p => p.id === professionid)!;
    })

    if (category!) {
      return of(category);
    }

    return this.http.get<IProfession>(this.apiUrl + 'Profession/profession/' + professionid);
  }

  deleteCategory(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Profession/deletebyid/' + id);
  }

  updateCategory(id: number, name: string) {
    if(id===0) {
        return this.http.post<IProfession>(this.apiUrl + 'Profession/add/' + name, {});
    } else {
      var prof: IProfession = {id: id, professionName: name};
      return this.http.put<IProfession>(this.apiUrl + 'Profession/edit', prof);
    }
  }
  
  setParams(params: professionParams) {
    this.mParams = params;
  }
  
  getParams() {
    return this.mParams;
  }

  
}
