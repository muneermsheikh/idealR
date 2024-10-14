import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { ICVRefAndDeployDto } from '../_dtos/process/cvRefAndDeployDto';
import { Pagination } from '../_models/pagination';
import { deployParams } from '../_models/params/process/deployParams';
import { IDeployStatusAndName } from '../_models/masters/deployStage';
import { IDeploymentDto } from '../_models/process/deploymentdto';
import { GetHttpParamsForCandidateFlightHdr, GetHttpParamsForDepProcess, getPaginatedResult } from './paginationHelper';
import { IDeploymentPendingDto } from '../_dtos/process/deploymentPendingDto';
import { IDepItem } from '../_models/process/depItem';
import { IDepItemToAddDto } from '../_dtos/process/depItemToAddDto';
import { ICVRefDto } from '../_dtos/admin/cvRefDto';
import { IDep } from '../_models/process/dep';
import { IDepItemWithFlightDto } from '../_models/process/depItemsWithFlight';
import { IFlightdata } from '../_models/process/flightData';
import { ICandidateFlight } from '../_models/process/candidateFlight';
import { IDeploymentStatus } from '../_models/masters/deployStatus';
import { IDeploymentPendingBriefDto } from '../_dtos/process/deploymentPendingBriefDto';
import { IDepItemsAndCandFlightsToAddDto } from '../_dtos/process/DepItemsToAddAndCandFlightsToAddDto';
import { ICandidateFlightGrp } from '../_models/process/candidateFlightGrp';
import { CandidateFlightParams } from '../_models/params/process/CandidateFlightParams';
import { ICandidateFlightGrpDto } from '../_dtos/process/candidateFlightGrpDto';
import { ICandiateFlightItem } from '../_models/process/candidateFlightItem';


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
  fCache = new Map();
  
  //for processDto list
  dParams = new deployParams();
  candFlightParams = new CandidateFlightParams();
  
  deployments: IDeploymentDto[]=[];

  deployStatuses= [
    { "StatusName": "Selected", "Sequence": 100,"NextSequence": 300,"WorkingDaysReqdForNextStage": 0},
    { "StatusName": "Document Certification Initiated","Sequence": 200,"NextSequence": 300,"WorkingDaysReqdForNextStage": 60},
    { "StatusName": "Refer for Medical Tests","Sequence": 300,"NextSequence": 400,"WorkingDaysReqdForNextStage": 5},
    { "StatusName": "Medically Fit","Sequence": 400,"NextSequence": 600,"WorkingDaysReqdForNextStage": 3},
    { "StatusName": "Medically Unfit","Sequence": 500,"NextSequence": 5000,"WorkingDaysReqdForNextStage": 0},
    { "StatusName": "Visa Documents submitted", "Sequence": 600,"NextSequence": 700,"WorkingDaysReqdForNextStage": 5},
    { "StatusName": "Visa Issued", "Sequence": 700, "NextSequence": 900,"WorkingDaysReqdForNextStage": 6},
    { "StatusName": "Visa Rejected","Sequence": 800,"NextSequence": 5000,"WorkingDaysReqdForNextStage": 0},
    { "StatusName": "Emigration application lodged","Sequence": 900,"NextSequence": 1000,"WorkingDaysReqdForNextStage": 2},
    { "StatusName": "Emigration Documents lodged","Sequence": 1000,"NextSequence": 1100, "WorkingDaysReqdForNextStage": 2},
    { "StatusName": "Emigration Cleared", "Sequence": 1100, "NextSequence": 1300, "WorkingDaysReqdForNextStage": 5},
    { "StatusName": "Emigration Denied","Sequence": 1200, "NextSequence": 5000,"WorkingDaysReqdForNextStage": 0},
    { "StatusName": "Ticket booked","Sequence": 1300,"NextSequence": 1500,"WorkingDaysReqdForNextStage": 0},
    { "StatusName": "Only Processing - Documents couriered to candidate","Sequence": 1400,"NextSequence": 5000,
      "WorkingDaysReqdForNextStage": 0},
    { "StatusName": "Traveled", "Sequence": 1500, "NextSequence": 5000, "WorkingDaysReqdForNextStage": 0},
    { "StatusName": "Arrival Acknowledged by Client","Sequence": 1600,"NextSequence": 5000,"WorkingDaysReqdForNextStage": 0},
    { "StatusName": "Concluded","Sequence": 5000,"NextSequence": 0,"WorkingDaysReqdForNextStage": 0}
  ]

  getDeployStatusOfASequence(seq:number): string {
    return this.deployStatuses.filter(x => x.Sequence===seq).map(x => x.StatusName)[0];
  }
  
  constructor(private http: HttpClient) { }

  setParams(params: deployParams) {
    this.dParams = params;
  }
  
  getParams() {
    return this.dParams;
  }

  getFlightParams() {
    return this.candFlightParams;
  }
  
  setFlightParams(params: CandidateFlightParams) {
    this.candFlightParams = params;
  }

  getCandidateFlightHeadersPagedList(fParams: CandidateFlightParams) {
    const response = this.cache.get(Object.values(fParams).join('-'));
    if(response) return of(response);

    let params = GetHttpParamsForCandidateFlightHdr(fParams);

    return getPaginatedResult<ICandidateFlightGrpDto[]>(this.apiUrl + 
        'Deployment/CandidateFlights', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(fParams).join('-'), response);
        return response;
      })
    )
  }

  getCandidateFlightCandidates(flightid: number) {
    
    return this.http.get<ICandiateFlightItem[]>(this.apiUrl + 'Deployment/CandidateFlightItems/' + flightid);
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

  uploadAttachmentForItem(model: any) {
    return this.http.post<string>(this.apiUrl + 'FileUpload/uploadDepAttachment', model);
  }

  insertDepItemsWithTravelBooked(depItemsWithBooking: IDepItemWithFlightDto[]) {
    return this.http.post<IDeploymentPendingDto[]>(this.apiUrl + 'Deployment/insertItemsWithFlight', depItemsWithBooking);
  }

  insertDepItemsWithFlightAttachment(depItemsWithBooking: any) {

    return this.http.put<string>(this.apiUrl + 'FileUpload/insertItemsWithFlightAttachment', depItemsWithBooking);
  }

  composeTravelAdviseForClient(flightid: number) {
    console.log('fightid', flightid);
    return this.http.get<string>(this.apiUrl + 'Deployment/traveladvise/' + flightid);
  }

  getFlightData() {
    return this.http.get<IFlightdata[]>(this.apiUrl + 'Deployment/flightdata');
  }

  getCandidateFlightFromId(id: number) {
    return this.http.get<ICandidateFlightGrp>(this.apiUrl + 'Deployment/candidateFlightGrp/' + id);
  }

  getCandidateFlightIds(cvrefids: number[]) {
    return this.http.put<number[]>(this.apiUrl + 'Deployment/candidateflightids', {cvrefids});
  }

  deleteCandidateFlight(candidateFlightId: number) {
    return this.http.delete<boolean>(this.apiUrl + 'Deployment/CandidateFlight/' + candidateFlightId);
  }

  editCandidateFlight(candidateflight: ICandidateFlight) {
    return this.http.put<string>(this.apiUrl + 'Deployment/CandidateFlight', candidateflight);
  }

  deleteFlightCandidate(flightCandidateId: number) {
    return this.http.delete<string>(this.apiUrl + 'Deployment/FlightCandidate/' + flightCandidateId);
  }

  InsertDepItems(deployItems: IDepItemToAddDto[] ){
    return this.http.post<IDeploymentPendingBriefDto[]>(this.apiUrl + 'Deployment/depItems', deployItems);
  }

  InsertDepItemsAndCandFlights(model: IDepItemsAndCandFlightsToAddDto) {
    console.log('calling api', model);
    return this.http.post<IDeploymentPendingBriefDto[]>(this.apiUrl + 'Deployment/depItemsAndCandFlight', model);
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


  downloadAttachment(fullpath: string) {
    let params = new HttpParams();
    params = params.append('fullpath', fullpath);

    return this.http.get(this.apiUrl + 'FileUpload/downloadfile', {params, responseType: 'blob'});
  }

  deleteAttachment(fullpath: string) {
    
    let params = new HttpParams();
    params = params.append('fullpath', fullpath);

    return this.http.get<string>(this.apiUrl + 'FileUpload/deleteattachmentbyfullpath', {params});
  }

}
