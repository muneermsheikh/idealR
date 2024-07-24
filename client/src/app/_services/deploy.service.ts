import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { ICVRefAndDeployDto } from '../_dtos/process/cvRefAndDeployDto';
import { Pagination } from '../_models/pagination';
import { deployParams } from '../_models/params/process/deployParams';
import { IDeployStage, IDeployStatusAndName } from '../_models/masters/deployStage';
import { IDeploymentDto } from '../_models/process/deploymentdto';
import { GetHttpParamsForDepProcess, getPaginatedResult } from './paginationHelper';
import { IDeploymentPendingDto } from '../_dtos/process/deploymentPendingDto';
import { IDepItem } from '../_models/process/depItem';
import { IDepItemToAddDto } from '../_dtos/process/depItemToAddDto';
import { ICVRefDto } from '../_dtos/admin/cvRefDto';
import { IDep } from '../_models/process/dep';
import { IDepItemWithFlightDto } from '../_models/process/depItemsWithFlight';
import { IFlightdata } from '../_models/process/flightData';
import { ICandidateFlight } from '../_models/process/candidateFlight';
import { IDeploymentStatus } from '../_models/masters/deployStatus';


@Injectable({
  providedIn: 'root'
})
export class DeployService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  deployStages: IDeploymentStatus[]=[];
  depSeqAndNames: IDeployStatusAndName[]=[];

  deploys: ICVRefAndDeployDto[]=[];
  pagination: Pagination | undefined;   //<IDeploymentPendingDto[]>| undefined;
  
  cache = new Map();
  
  //for processDto list
  dParams = new deployParams();
  
  deployments: IDeploymentDto[]=[];
  
  constructor(private http: HttpClient) { }

  setParams(params: deployParams) {
    this.dParams = params;
  }
  
  getParams() {
    return this.dParams;
  }
  
  getDeploymentPagedList(dParams: deployParams) {

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

  getDepStatusAndNames() {
    if(this.depSeqAndNames.length > 0) return of(this.depSeqAndNames);

    return this.http.get<IDeployStatusAndName[]>(this.apiUrl + 'Deployment/deployStatusSeqAndNames')
      .pipe(
        map(response => {
          this.depSeqAndNames = response;
          return response;
        })
      )
  }

  getDeployStatuses() {
    
    if(this.deployStages.length > 0) return of(this.deployStages);

    return this.http.get<IDeploymentStatus[]>(this.apiUrl + 'Deployment/deployStatusData')
      .pipe(
        map(response => {
          this.deployStages = response;
          return response;
        })
      )
  }

  getDeploymentWithItems(id: number) {
    return this.http.get<IDep>(this.apiUrl + 'Deployment/deployment/' + id);
  }

  updateDeployment(deploy: IDep) {
    return this.http.put<string>(this.apiUrl + 'Deployment/deployment', deploy);
  }

  updateDeploymentItem(deploy: IDepItem) {
    return this.http.put<boolean>(this.apiUrl + 'Deployment/depItem', deploy);
  }

  insertDepItemsWithTravelBooked(depItemsWithBooking: IDepItemWithFlightDto[]) {
    return this.http.post<IDeploymentPendingDto[]>(this.apiUrl + 'Deployment/insertItemsWithFlight', depItemsWithBooking);
  }

  getFlightData() {
    return this.http.get<IFlightdata[]>(this.apiUrl + 'Deployment/flightdata');
  }

  getCandidateFlight(cvrefid: number) {
    return this.http.get<ICandidateFlight>(this.apiUrl + 'Deployment/candidateflight/' + cvrefid);
  }

  deleteCandidateFlight(candidateFlightId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Deployment/CandidateFlight/' + candidateFlightId);
  }

  editCandidateFlight(candidateflight: ICandidateFlight) {
    return this.http.put<string>(this.apiUrl + 'Deployment/CandidateFlight', candidateflight);
  }
  InsertDepItems(deployItems: IDepItemToAddDto[] ){
    return this.http.post<IDeploymentPendingDto[]>(this.apiUrl + 'Deployment/depItems', deployItems);
  }


  getCVReferredDto(cvrefid: number) {
    return this.http.get<ICVRefDto>(this.apiUrl + 'Deployment/bycvrefid/' + cvrefid);
  }

  getNextDepItemFromCVRefId(cvrefid: number) {
    return this.http.get<IDepItem>(this.apiUrl + 'Deployment/nextDepItemFromCVRef/' + cvrefid );
  }

  getNextSequence(sequence: number, ecnr: boolean) {
    
    if(sequence===700) return ecnr ? 1300: 900;
    
    return this.deployStages.find(x => x.sequence==sequence)?.nextSequence;
  }

  NextSequenceProposedIsOkay(sequence: number, nextSequence: number, ecnr: boolean): boolean {
    
    var nextSequenceShdBe = this.getNextSequence(sequence, ecnr);
    if(nextSequence === nextSequenceShdBe) return true;

    var nextStageShdBe = this.deployStages.find(x => x.sequence === nextSequence);
    if(nextStageShdBe?.isOptional && (nextSequence - sequence === 200)) return true;
    //except for emigration, all other optional processes are one stage after,
    //meaning diff in sequences is 200

    return false;
  }

  getDepStage(stageId: number): IDeploymentStatus | null {

    if(this.deployStages.length > 0) {
      return this.deployStages.filter(x => x.sequence == stageId)[0];
    } 

    return null;
  }

  getNextStage(sequence: number, ecnr: boolean): IDeploymentStatus | null {

    if(this.deployStages.length > 0) {
      var nextSequence = this.getNextSequence(sequence, ecnr);
      return this.deployStages.filter(x => x.sequence === nextSequence)[0];
      
    }

    return null;
  }

  getNextStageDate(stageId: number): Date {
    var dayToCompleteStage = this.deployStages.find(x => x.id==stageId)?.workingDaysReqdForNextStage;
       
    if(dayToCompleteStage !== undefined) {
      return new Date(new Date().setDate(new Date().getDate()+ dayToCompleteStage));
    } else {
      return new Date();
    }
    
  }

  deleteDeployment(deployId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Deployment/dep/' + deployId);
  }

}
