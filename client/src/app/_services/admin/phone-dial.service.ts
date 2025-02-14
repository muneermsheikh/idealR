import { Injectable } from '@angular/core';
import { map, of, ReplaySubject } from 'rxjs';
import { Pagination } from 'src/app/_models/pagination';
import { audioMessageParams } from 'src/app/_models/params/audioMessageParams';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult } from '../paginationHelper';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IAudioMessageDto } from 'src/app/_dtos/hr/audioMessageDto';

@Injectable({
  providedIn: 'root'
})
export class PhoneDialService {

    apiUrl = environment.apiUrl;
    private currentUserSource = new ReplaySubject<User>(1);
    currentUser$ = this.currentUserSource.asObservable();
    audioParams: audioMessageParams | undefined; // = new userHistoryParams();
    pagination: Pagination | undefined; //<IUserHistoryDto[]>;
    cache = new Map();
    

  constructor(private http: HttpClient) { }

  getPendingAudioCallsPaged(oParams: audioMessageParams) { 
  
      const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = getHttpParamsForAudioMessages(oParams);
  
      return getPaginatedResult<IAudioMessageDto[]>(this.apiUrl + 
          'orders/pagedlist', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )
    }
    
}

export function getHttpParamsForAudioMessages(oParams: audioMessageParams) {

    let params = new HttpParams();

    params = params.append('pageNumber', oParams.pageNumber);
    params = params.append('pageSize', oParams.pageSize)

    if (oParams.recipientUsername !== "") params = params.append('recipientUsername', oParams.recipientUsername);
    if (oParams.senderUsername !== "") params = params.append('senderUsername', oParams.senderUsername);
    if (oParams.candidateName !== "") params = params.append('candidateName', oParams.candidateName);
    if (oParams.applicationNo !== 0) params = params.append('applicationNo', oParams.applicationNo.toString());
    
    return params;
      
  }