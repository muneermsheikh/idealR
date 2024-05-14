import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { ICustomerNameAndCity } from '../_models/admin/customernameandcity';
import { ISkillData } from '../_models/hr/skillData';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  apiUrl = environment.apiUrl;
  agents: ICustomerNameAndCity[]=[];
  customers: ICustomerNameAndCity[]=[];

  cacheEmp = new Map();
  cacheQ = new Map();   //qualifications

  constructor(private http: HttpClient) { }
  
  
  getCustomerList(customerType: string) {
    if (this.customers.length > 0) {
      return of(this.customers);
    }
    return this.http.get<ICustomerNameAndCity[]>(this.apiUrl + 'customers/idandnames/' + customerType).pipe(
      map(response => {
        this.customers = response;
        return response;
      })
    );
  }

  
  getSkillData() {
    return this.http.get<ISkillData[]>(this.apiUrl + 'masters/skillDatalist');
  }

 
  checkAadharNoExists(aadharNo: string) {
    return this.http.get<boolean>(this.apiUrl + 'account/' + aadharNo);
  }


  getCustomerOfficialIds() {
    
  }

  /*
  getDropDownText(id: number, object: any){
    const selObj = _.filter(object, function (o: any) {
        return ( _.includes(id,o.id));
    });
    return selObj;
  }
*/
}
