import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult } from './paginationHelper';
import { Message } from '../_models/message';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  
  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    
    params = params.append('Container', container);
    
    return getPaginatedResult<Message[]>(this.baseUrl + 'Messages', params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'Messages?username=' + username);
  }

  /* async sendMessage(username: string, content: string) {
    return this.hubConnection?.invoke('SendMessage', { recipientUsername: username, content })
      .catch(error => console.log(error));
  }
  */

  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', 
      {recipientUsername: username, content} );
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
