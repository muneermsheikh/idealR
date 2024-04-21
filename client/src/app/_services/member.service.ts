import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { UserParams } from '../_models/params/userParams';
import { AccountService } from './account.service';
import { map, of, take } from 'rxjs';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  //memberCache = new Map();
  user?: User | null;
  userParams: UserParams | undefined;

  memberCache = new Map();

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user)
          this.userParams = new UserParams(user);
          this.user = user;
      }
    })
   }


  getMembers(userParams: UserParams) {

    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response) return of(response);

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    if(userParams.minAge > 0) params = params.append('minAge', userParams.minAge);
    if(userParams.maxAge > 0) params = params.append('maxAge', userParams.maxAge);
    if(userParams.gender !== '') params = params.append('gender', userParams.gender);
    if(userParams.orderBy !== '') params = params.append('orderBy', userParams.orderBy);
    
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    )
  }

  
  getMember(username: string) {
    
    const member = [...this.memberCache.values()]
      .reduce((arr,elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === username);
    
    if(member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/byusername' + '/' + username);
  }


  private getPaginatedResult<T>(url: string, params: HttpParams) {

    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>

    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) paginatedResult.result = response.body;

        const pagination = response.headers.get('Pagination');

        if (pagination) paginatedResult.pagination = JSON.parse(pagination);

        return paginatedResult;
      })
    );
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

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member};
      })
    )
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number){
    let params = new HttpParams();

      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);


    return params;
  }
}
