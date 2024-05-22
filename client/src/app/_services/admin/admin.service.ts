import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/params/userParams';
import { environment } from 'src/environments/environment.development';
import { getHttpParamsForUserParams, getPaginatedResult } from '../paginationHelper';
import { User } from 'src/app/_models/user';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { ICustomerOfficialDto } from 'src/app/_models/admin/customerOfficialDto';
import { IOrderItemsAndAgentsToFwdDto } from 'src/app/_dtos/admin/orderItemsAndAgentsToFwdDto';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.apiUrl;
  users: User[] = [];
  
  cache = new Map();
  userParams: UserParams | undefined;

  pagination?: Pagination;

  constructor(private http: HttpClient) { }

  getUsersPaginated(usrParams: UserParams){ 

    //if (useCache === false)  this.cache = new Map();

    if (this.cache.size > 0 ) {
      const response = this.cache.get(Object.values(usrParams).join('-'));
      if(response) return of(response);
    }

    let params = new HttpParams();
    /*
    if (this.usrParams.email !== "") params = params.append('email', this.usrParams.email);
    if (this.usrParams.username !== '') params = params.append('username', this.usrParams.username);
    if (this.usrParams.search) params = params.append('search', this.usrParams.search);
    */
    //params = params.append('sort', usrParams.sort);
    params = params.append('pageIndex', usrParams.pageNumber.toString());
    params = params.append('pageSize', usrParams.pageSize.toString());

    return getPaginatedResult<User[]>(this.baseUrl + 'users', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(usrParams).join('-'), response);
        return response;
      })
    )

  }

   
  setUserParams(params: UserParams) {
    this.userParams = params;
  }
  
  getUserParams() {
    return this.userParams;
  }

  getEmployeeIdAndKnownAs() {
    return this.http.get<IEmployeeIdAndKnownAs[]>(this.baseUrl + 'employees/idandknownas');
  }
  
  getUsersWithRolesPaginated(pageNumber: number, pageSize: number) {
    //console.log('calling api for getuserswithroles');
    //return this.http.get<Pagination<User[]> | undefined | null>(this.baseUrl + 'admin/users-with-roles-paginated');
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString() );
    params = params.append('pageSize', pageSize.toString());
    
    //params = params.append('predicate', predicate);

    return getPaginatedResult<User[]>(this.baseUrl + 'admin/"users-with-roles', params, this.http);
  }

  updateUserRoles(email: string, roles: string[]) {
    //return this.http.post(this.baseUrl + 'admin/edit-roles/' + email + '?roles=' + roles, {});
    return this.http.post(this.baseUrl + 'admin/edit-roles/'+ email + '?roles=' + roles,{});
  }

  
  getIdentityRoles() {
    return this.http.get<string[]>(this.baseUrl + 'admin/identityroles');
  }



  addNewRole(newRoleName: string) {

    return this.http.post(this.baseUrl + 'admin/role/' + newRoleName, {});
  }

  deleteRole(roleName: string) {
    return this.http.delete(this.baseUrl + 'admin' + roleName);
  }

  editrRole(existingRoleName: string, newRoleName: string) {
    return this.http.put(this.baseUrl + 'role/' + existingRoleName + '/' + newRoleName, {});
  }
}
