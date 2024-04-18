import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { UserParams } from '../_models/parameters/userParams';
import { AccountService } from './account.service';
import { map, of, take } from 'rxjs';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user?: User | null;
  userParams: UserParams | undefined;


  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user)
          this.userParams = new UserParams(user);
          this.user = user;
      }
    })
   }



  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users');
    /*const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    )
    */
  }

  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/byusername' + '/' + username);
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(userParams: UserParams) {
    this.userParams = userParams;
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {})
  }

}
