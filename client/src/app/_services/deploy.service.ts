import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { ICVRefAndDeployDto } from '../_dtos/process/cvRefAndDeployDto';
import { Pagination } from '../_models/pagination';
import { deployParams } from '../_models/params/process/deployParams';
import { IDeployStage } from '../_models/masters/deployStage';
import { IDeploymentDto } from '../_models/process/deploymentdto';
import { GetHttpParamsForDepProcess, getPaginatedResult } from './paginationHelper';
import { IDeploymentPendingDto } from '../_dtos/process/deploymentPendingDto';
import { IDeploymentDtoWithErrorDto } from '../_dtos/process/deploymentDtoWithErrorDto';
import { IDepItem } from '../_models/process/depItem';
import { IDepItemToAddDto } from '../_dtos/process/depItemToAddDto';
import { ICVRefDto } from '../_dtos/admin/cvRefDto';

@Injectable({
  providedIn: 'root'
})
export class DeployService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  pageSize=5;
  pageIndex=1;
  pParams = new deployParams();
  deployStages: IDeployStage[]=[];

  deploys: ICVRefAndDeployDto[]=[];
  pagination: Pagination | undefined;   //<IDeploymentPendingDto[]>| undefined;
  
  cache = new Map();
  
  //for processDto list
  dParams = new deployParams();
  cacheDep = new Map();
  deployments: IDeploymentDto[]=[];
  
  constructor(private http: HttpClient) { }

  setOParams(params: deployParams) {
    this.pParams = params;
  }
  
  getOParams() {
    return this.pParams;
  }

  setDParams(params: deployParams) {
    this.dParams = params;
  }
  
  getDParams() {
    return this.dParams;
  }

  
  getCachedProcessesList(dParams: deployParams) {
    //const token = localStorage.getItem("token");
    //if (token==='') return;
    
    const response = this.cache.get(Object.values(dParams).join('-'));
    if(response) return of(response);

    let params = GetHttpParamsForDepProcess(dParams);

    return getPaginatedResult<IDeploymentPendingDto[]>(this.apiUrl + 
        'Deployment/deployments', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(dParams).join('-'), response);
        return response;
      })
    )
  }

  getDeployStatus() {
    //return this.http.get<IDeploymentStatus[]>(this.apiUrl + 'deploy/depstatus');
    if(this.deployStages.length > 0) return of(this.deployStages);

    return this.http.get<IDeployStage[]>(this.apiUrl + 'Deployment/depStatusData')
      .pipe(
        map(response => {
          this.deployStages = response;
          return response;
        })
      )
  }

  updateDeploymentItem(deploy: IDepItem) {
    
    return this.http.put<boolean>(this.apiUrl + 'Deplyment/depItem', deploy);
  }

  InsertDeployTransactions(deployItems: IDepItemToAddDto[] ){
    return this.http.post<IDeploymentDtoWithErrorDto>(this.apiUrl + 'deploy/depItems', deployItems);
  }

  getCVReferredDto(cvrefid: number) {
    return this.http.get<ICVRefDto>(this.apiUrl + 'Deployment/bycvrefid/' + cvrefid);
  }

  
  getNextSequence(cvrefid: number) {
    if(this.deployments.length===0 || this.dParams.cvrefid !== cvrefid){
      this.dParams.cvrefid=cvrefid;
      this.getCachedProcessesList(this.dParams);

    }
    var arr = new Array(this.deployments.length).map(x => x.sequence );

    var mx = Math.max(...arr);
    if(this.deployments !== undefined) {
      this.deployments.forEach(x => {
        if (x.sequence==mx) {
          
          return x.nextSequence;
        } else {
          return 0;
        }
      })
      return 0;
    } else {
      return 0;
    }
    
  }
  
  getNextNextStageId(stageId: number) {
    return this.deployStages.find(x => x.id==stageId)?.nextSequence;
  }

  getNextStageDate(stageId: number): Date {
    var dayToCompleteStage = this.deployStages.find(x => x.id==stageId)?.estimatedDaysToCompleteThisStage;
       
    if(dayToCompleteStage !== undefined) {
      return new Date(new Date().setDate(new Date().getDate()+ dayToCompleteStage));
    } else {
      return new Date();
    }
    
  }
}
