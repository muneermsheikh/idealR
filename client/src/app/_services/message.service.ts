import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { getPaginatedResult } from './paginationHelper';
import { Message } from '../_models/message';
import { HttpClient, HttpParams } from '@angular/common/http';
import { messageParams } from '../_models/params/Admin/messageParams';
import { map, of } from 'rxjs';
import { IMessage } from '../_models/admin/message';
import { IMessageToSendDto } from '../_dtos/admin/messageToSendDto';
import { MessageType } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;
  msgParams = new messageParams();

  cache = new Map();

  msgTypes = [
    {"MessageType" : "EmigrationClearanceAdviseByEmail"}, {"MessageType" : "MedicalFitnessAdviseByEmail"},
    {"MessageType" : "MedicalUnfitnessAdvise"}, {"MessageType" : "TravelAdviseToClient"},
    {"MessageType" : "VisaIssueAdviseByMail"}, {"MessageType" : "VisaRejectionAdviseByMail"},
    {"MessageType" : "CustomerFeedback"}, {"MessageType": "AdviseToHRDeptHead"}, {"MessageType": "AssignTaskToHRExec"},
    {"MessageType" : "CVFwdTask"}, {"MessageType": "OrderFwdToHR"}, {"MessageType": "InterviewInvitation"}
  ]

  constructor(private http: HttpClient) { }

  
  getMessageTypes() {
    return this.msgTypes
  }

  getMessages(useCache: boolean=true) {

    var msgparams = this.msgParams;
    
    if(useCache) {
      const response = this.cache.get(Object.values(msgparams).join('-'));
      if(response) return of(response);
    }

    let params = new HttpParams();
    params = params.append('pageNumber', msgparams.pageNumber.toString());
    params = params.append('pageSize',msgparams.pageSize.toString());
    params = params.append('Container', msgparams.container);

    if(msgparams.cvRefId !==0) params = params.append('cvRefId', msgparams.cvRefId.toString());
    if(msgparams.recipientEmail !== '') params = params.append('recipientEmail', msgparams.recipientEmail);
    if(msgparams.senderEmail !== '') params = params.append('senderEmail', msgparams.senderEmail);
    if(msgparams.recipientUsername !=='') params = params.append('recipientUsername', msgparams.recipientUsername);
    if(msgparams.senderUsername !=='') params = params.append('senderUsername', msgparams.senderUsername);
    if(msgparams.messageType !=='') params = params.append('messageType', msgparams.messageType);
    
    return getPaginatedResult<Message[]>(this.baseUrl + 'Messages', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(this.msgParams).join('-'), response);
        return response;
      })
    )
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'Messages?username=' + username);
  }

  /* async sendMessage(username: string, content: string) {
    return this.hubConnection?.invoke('SendMessage', { recipientUsername: username, content })
      .catch(error => console.log(error));
  }
  */

  sendMessage(message: IMessageToSendDto) {
    return this.http.post<Message>(this.baseUrl + 'messages/sendMessage', message)
      //{recipientUsername: username, content} );
  }

  deleteMessage(id: number) {
    return this.http.delete<string>(this.baseUrl + 'messages/' + id);
  }

  setParams(sParams: messageParams) {
    this.msgParams = sParams;
  }

  getParams() {
    return this.msgParams;
  }
}
