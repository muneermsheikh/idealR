import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/app/environments/environment';
import { IDLForwardToAgent } from '../../models/admin/dlforwardToAgent';
import { IForwardedCategoryDto } from '../../dtos/admin/forwardedCategoryDto';
import { IApplicationTask } from '../../models/admin/applicationTask';

@Injectable({
  providedIn: 'root'
})
export class DlForwardService {

  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

   //forward DL to agents
  forwardDLtoSelectedAgents(dlforward: IDLForwardToAgent) {
    return this.http.post<string>(this.apiUrl + 'DLForward', dlforward );
  }

  //get dlforwards of an orderid
  getDLForwardsOfAnOrderId(orderid: number) {
    return this.http.get<IDLForwardToAgent[]>(this.apiUrl + 'DLForward/byorderid/' + orderid );
  }

  getAssociatesForwardedForADL(orderid: number) {
    return this.http.get<IForwardedCategoryDto[]>(this.apiUrl + 'DLForward/associatesforwardedForOrderId/' + orderid);
  }

  forwardDLtoHRHead(orderid: number) {
    return this.http.post<IApplicationTask>(this.apiUrl + 'DLForward/addtaskdltohr/' + orderid, {});
  }
}
