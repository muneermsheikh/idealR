import { HttpClient } from '@angular/common/http';
import { computed, Injectable } from '@angular/core';
import { BehaviorSubject, map, take } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment.development';
import { IReturnStringsDto } from '../_dtos/admin/returnStringsDto';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }
  
  login(model: any) {
    
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
          this.setCurrentUser(user);
        }
      })
    )
    
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
        //return user;
      })
    )
  }

  roles = computed(() => {
    this.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user)
          if (user && user.token) {
            const role = JSON.parse(atob(user.token.split('.')[1])).role;
            return Array.isArray(role) ? role : [role];
          }
          return [];      
      }
    })

  })


  copyProspectiveNaukriXLSFileToDB(model: any) {
      return this.http.post<IReturnStringsDto>(this.baseUrl + 'FileUpload/naukriprospectiveXLS', model);
  }

  copyProspectiveXLSFileToDB(model: any) {
    return this.http.post<IReturnStringsDto>(this.baseUrl + 'FileUpload/prospectiveXLS', model);
  }

  copyCustomerXLSFileToDB(model: any) {
    return this.http.post<string>(this.baseUrl + 'FileUpload/customerXLS', model);
  }

  copyCandidateXLSFileToDB(model: any) {
    return this.http.post<string>(this.baseUrl + 'FileUpload/candidateXLS', model);
  }

  copyEmployeeXLSFileToDB(model: any) {
    return this.http.post<string>(this.baseUrl + 'FileUpload/employeeXLS', model);
  }

  copyOrderXLSFileToDB(model: any) {
    return this.http.post<string>(this.baseUrl + 'OrdersExport/orderexcelconversion', model);
  }



  checkEmailExists(email: string) {
    return this.http.get(this.baseUrl + 'account/emailexists?email=' + email);
  }

  
  checkPPExists(ppnumber: string) {
    return this.http.get(this.baseUrl + 'Candidate/ppexists/' + ppnumber);
  }



  setCurrentUser(user: User) {
    user.roles=[];
    const roles = this.getDecodedToken(user.token);
    //console.log('decoded token atob: ', roles.includes('admin'));
    Array.isArray(roles) ? user.roles = roles.flat() : user.roles.push(roles);
    
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);

  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  getDecodedToken(token: string) {
    var s = JSON.parse(atob(token.split('.')[1]));
    return s;
  }
}
