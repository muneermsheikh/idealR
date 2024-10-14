import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { UserParams } from '../_models/params/userParams';
import { AccountService } from './account.service';
import { map, of, take } from 'rxjs';
import { getHttpParamsForUserParams, getPaginatedResult} from './paginationHelper';
import { IAppUserReturnDto } from '../_dtos/admin/appUserReturnDto';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  user?: User | null;
  uParams = new UserParams(); // | undefined;

  cache = new Map();

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user)
          this.uParams = new UserParams();
          this.user = user;
      }
    })
   }


  getMembers(useCache: boolean=true) {

    var userparams = this.uParams;
    
    if(useCache) {
      const response = this.cache.get(Object.values(userparams).join('-'));
      if(response) return of(response);
    }
    
    let params = getHttpParamsForUserParams(userparams!);

    return getPaginatedResult<Member[]>(this.baseUrl + 'Admin/pagedlist', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(userparams).join('-'), response);
        return response;
      })
    )
  }

  
  getMember(username: string) {
    
    const member = [...this.cache.values()]
      .reduce((arr,elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === username);
    
    if(member) {
      console.log('returned from cache');
      return of(member);
    }
    
    //console.log('getting member deteail from api');
    return this.http.get<Member>(this.baseUrl + 'users/byusername' + '/' + username);
  }

  createNewMember(userType: string, idvalue: number) {
    return this.http.post<IAppUserReturnDto>(this.baseUrl + 'users/newappuser/' + userType + '/' + idvalue, {});
  }

  getUserParams() {
    return this.uParams;
  }

  setUserParams(userParams: UserParams) {
    this.uParams = userParams;
  }

  resetUserParams() {
    if (this.user) {
      this.uParams = new UserParams();
      return this.uParams;
    }
    return;
  }

  updateMember(member: Member) {
    return this.http.put<boolean>(this.baseUrl + 'admin', member);
    /*.pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member};
      })
    )*/
  }

  deleteMember(id: number) {

    return this.http.delete<boolean>(this.baseUrl + 'users/delete/' + id);
  }

}
