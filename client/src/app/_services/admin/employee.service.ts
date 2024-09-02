import { HttpClient, HttpParams } from '@angular/common/http';
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
import { ISkillData } from 'src/app/_models/hr/skillData';
import { IIndustryType } from 'src/app/_models/admin/industryType';
import { IEmployeeAttachment } from 'src/app/_models/admin/employeeAttachment';

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
  eParams: employeeParams = new employeeParams();

  constructor(private http: HttpClient) { }
  
  getEmployeeIdAndKnownAs() {
    return this.http.get<IEmployeeIdAndKnownAs[]>(this.apiUrl + 'employees/idandknownas');
  }
  

  getEmployeesPaged() {

      var eParams = this.eParams;

      const response = this.cache.get(Object.values(eParams).join('-'));
      if(response) return of(response);
  
      let params = getHttpParamsForEmployees(eParams);
  
      return getPaginatedResult<IEmployeeBriefDto[]>(this.apiUrl + 'Employees/employeepaged', params, this.http).pipe(
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
    return this.http.put<boolean>(this.apiUrl + 'Employees', employee);
  }

  updateOrAddEmployeeWithAttachments(model: any) {
    return this.http.post(this.apiUrl + 'Candidate/updateEmployeeWithAttachments', model);
  }

  updateOrAddEmployeeAttachments(model: any) {
    console.log('calling fileupload/updateanduploadattachments');
    return this.http.put<boolean[]>(this.apiUrl + 'Employees/updateAndUploadAttachments', model);
  }

  getEmployeeAttachments(employeeid: number) {
    return this.http.get<IEmployeeAttachment[]>(this.apiUrl + 'Employees/getEmployeeAttachments/' + employeeid);
  }

  /*updateEmployeeWithAttachments(employee: any) {
    //return this.http.put<IEmployee>(this.apiUrl + 'Employees/updateWithAttachments', employee);
    return this.http.put(this.apiUrl + 'Candidate/updatecandidatewithfiles', employee);
  }*/

  deleteEmployee(id: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Employees/' + id);
  }

  addNewEmployee(dto: IEmployeeToAddDto) {
    return this.http.post<IEmployee>(this.apiUrl + 'Employees/employee', dto);
    
  }

  /*addNewEmployeeWithAttachments(model: any) {
    return this.http.post<IEmployee>(this.apiUrl + 'Employees/addnewemployeewithattachments', model);
  }*/

  downloadEmployeeAttachmentFile(fullpath: string) {
    let params = new HttpParams();
    params = params.append('fullpath', fullpath);

    return this.http.get(this.apiUrl + 'FileUpload/downloadfile', {params, responseType: 'blob'});

  }

  getSkillDatas() {
    return this.http.get<ISkillData[]>(this.apiUrl + 'employees/skilldatas');
  }

  getIndustryList() {
    return this.http.get<IIndustryType[]>(this.apiUrl + 'employees/industrylist');
  }

  checkEmployeeEmailExists(email: string) {
      return this.http.get<string>(this.apiUrl + 'employees/checkemailexists/' + email);
  }

  checkAadharExists(aadharno: string) {
      return this.http.get<string>(this.apiUrl + 'employees/checkaadharnoexists/' + aadharno);
  }

  setParams(sParams: employeeParams) {
    this.eParams = sParams;
  }

  getParams() {
    return this.eParams;
  }
  
}
