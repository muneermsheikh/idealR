import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { ICustomerNameAndCity } from 'src/app/_models/admin/customernameandcity';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { Pagination } from 'src/app/_models/pagination';
import { employeeParams } from 'src/app/_models/params/Admin/employeeParams';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';
import { getHttpParamsForEmployees, getPaginatedResult } from '../paginationHelper';
import { IEmployeeBriefDto } from 'src/app/_dtos/admin/employeeBriefDto';
import { IEmployee } from 'src/app/_models/admin/employee';
import { IEmployeeToAddDto } from 'src/app/_dtos/admin/employeeToAddDto';

@Injectable({
  providedIn: 'root'
})

export class EmployeeService {
  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  agents: ICustomerNameAndCity[]=[];
  
  cache= new Map();
  paginaion: Pagination | undefined;

  constructor(private http: HttpClient) { }
  
  getEmployeeIdAndKnownAs() {
    return this.http.get<IEmployeeIdAndKnownAs[]>(this.apiUrl + 'employees/idandknownas');
  }
  

  getEmployeesPagnated(eParams: employeeParams) {

      const response = this.cache.get(Object.values(eParams).join('-'));
      if(response) return of(response);
  
      let params = getHttpParamsForEmployees(eParams);
  
      return getPaginatedResult<IEmployeeBriefDto[]>(this.apiUrl + 'Employees/employeespaged', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(eParams).join('-'), response);
          return response;
        })
      )
  }

  getEmployeeById(employeeid: number) {
    return this.http.get<IEmployee>(this.apiUrl + 'Employees/byId/' + employeeid);
    
  }

  updateEmployee(employee: IEmployee) {
    return this.http.put<boolean>(this.apiUrl + 'Employee', employee);
  }

  deleteEmployee(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Employees/' + id);
  }

  addNewEmployee(dto: IEmployeeToAddDto) {
    return this.http.post<IEmployee>(this.apiUrl + 'Employees/employee', dto);
    
  }
  
}
