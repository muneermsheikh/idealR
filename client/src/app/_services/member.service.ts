import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { UserParams } from '../_models/params/userParams';
import { AccountService } from './account.service';
import { map, of, take } from 'rxjs';
import { getHttpParamsForUserParams, getPaginatedResult} from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  user?: User | null;
  userParams: UserParams | undefined;

  memberCache = new Map();

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user)
          this.userParams = new UserParams();
          this.user = user;
      }
    })
   }


  getMembers(userParams: UserParams) {

    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response) return of(response);

    let params = getHttpParamsForUserParams(userParams);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(
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
    
    if(member) {
      console.log('returned from cache');
      return of(member);
    }
    
    //console.log('getting member deteail from api');
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
      this.userParams = new UserParams();
      return this.userParams;
    }
    return;
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {})
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {

    var hParams = new UserParams();
    hParams.pageNumber = pageNumber;
    hParams.pageSize = pageSize;

    let params = getHttpParamsForUserParams(hParams);
    params = params.append('predicate', predicate);

    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member};
      })
    )
  }


}
