import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, of, take } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Pagination } from '../_models/pagination';
import { AccountService } from './account.service';
import { getPaginatedResult } from './paginationHelper';
import { IAudioMessageDto } from '../_dtos/hr/audioMessageDto';
import { audioMessageParams } from '../_models/params/audioMessageParams';
import { SetAudioText } from '../_dtos/admin/setAudioText';

@Injectable({
  providedIn: 'root'
})
export class AudioService {

    apiUrl = environment.apiUrl;
    private currentUserSource = new BehaviorSubject<User | null>(null);
    currentUser$ = this.currentUserSource.asObservable();
    audioParams: audioMessageParams | undefined;
    pagination: Pagination | undefined; //<IUserHistoryDto[]>;
    cache = new Map();
    user?: User | null;
  
    constructor(private http: HttpClient, private accountService: AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user => {
          if (user)
            this.audioParams = this.audioParams;
            this.user = user;
        }
      })
    }

      getAudioMessagesPaged(fromCache: boolean = true): Observable<any> { 
      
          if(!fromCache) {
            this.cache = new Map();
          } else {
            const response = this.cache.get(Object.values(this.audioParams!).join('-'));
            if(response) return of(response);
          }
      
          let params = getHttpParamsForAudio(this.audioParams!);
      
          return getPaginatedResult<IAudioMessageDto[]>(this.apiUrl + 
              'prospectives/audioMessagePagedlist', params, this.http).pipe(
            map(response => {
              this.cache.set(Object.values(this.audioParams!).join('-'), response);
              return response;
            })
          )
      
        }

        setAudioText(txt: SetAudioText) {
          return this.http.put<boolean>(this.apiUrl + 'prospectives/setAudioText', txt)
        }

        setParams(aParams: audioMessageParams) {
          this.audioParams = aParams;
        }

        getParams() {
          return this.audioParams;
        }
    
}

 export function getHttpParamsForAudio(audioParams: audioMessageParams)
  {
    let params = new HttpParams();

    params = params.append('pageNumber', audioParams.pageNumber.toString());
    params = params.append('pageSize', audioParams.pageSize.toString());

    if(audioParams.candidateName !== '') params = params.append('candidateName', audioParams.candidateName);
    if(audioParams.feedbackReceived !== 0) params = params.append('feedbackReceived', audioParams.feedbackReceived.toString());
    if(audioParams.dateComposed.getFullYear() > 2000) params = params.append('dateComposed', audioParams.dateComposed.toDateString());
    if(audioParams.recipientUsername !== '') params = params.append('recipientUsername', audioParams.recipientUsername);

    return params;
  }