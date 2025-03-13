import { Component, ElementRef, EventEmitter, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IDeploymentPendingDto } from 'src/app/_dtos/process/deploymentPendingDto';
import { IDeployStatusAndName } from 'src/app/_models/masters/deployStage';
import { Pagination } from 'src/app/_models/pagination';
import { deployParams } from 'src/app/_models/params/process/deployParams';
import { IDep } from 'src/app/_models/process/dep';
import { User } from 'src/app/_models/user';
import { DeployService } from 'src/app/_services/deploy.service';
import { DeployEditModalComponent } from '../deploy-edit-modal/deploy-edit-modal.component';
import { Subject, catchError, distinct, filter, of, switchMap, tap } from 'rxjs';
import { DeployAddModalComponent } from '../deploy-add-modal/deploy-add-modal.component';
import { IDepItem } from 'src/app/_models/process/depItem';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { IDepItemToAddDto } from 'src/app/_dtos/process/depItemToAddDto';
import { ChooseFlightModalComponent } from '../choose-flight-modal/choose-flight-modal.component';
import { IDeploymentStatus } from 'src/app/_models/masters/deployStatus';
import { DepAttachmentModalComponent } from '../dep-attachment-modal/dep-attachment-modal.component';
import { IDeploymentPendingBriefDto } from 'src/app/_dtos/process/deploymentPendingBriefDto';
import { IFlightdata } from 'src/app/_models/process/flightData';
import { IDepItemsAndCandFlightsToAddDto } from 'src/app/_dtos/process/DepItemsToAddAndCandFlightsToAddDto';
import { ICandidateFlightGrp } from 'src/app/_models/process/candidateFlightGrp';
import { ICandiateFlightItem } from 'src/app/_models/process/candidateFlightItem';
import { IReturnStringsDto } from 'src/app/_dtos/admin/returnStringsDto';


@Component({
  selector: 'app-deploy-listing',
  templateUrl: './deploy-listing.component.html',
  styleUrls: ['./deploy-listing.component.css']
})
export class DeployListingComponent implements OnInit{

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  @Input() EditDepEvent = new EventEmitter();

  user?: User;
  returnUrl = '';
  title = '';
  currentSearchValue = '';
  currentOption = '';
  currentStatus='';
  deployStatus = '';

  deploys: IDeploymentPendingBriefDto[]=[];
  deploysSelected: IDeploymentPendingBriefDto[]=[]; 
  FlightSelected: IFlightdata | undefined;
  candFlight: ICandidateFlightGrp | undefined;
  pagination: Pagination | undefined;

  routeId: string='';
  totalCount: number=0;
  
  depStatuses: IDeploymentStatus[]=[];
  statusNameAndSeq: IDeployStatusAndName[]=[];
  transDate: Date=new Date;
  sequenceSelected: string='';

  bsModalRef: BsModalRef | undefined;

  currentCVRefId: number=0;

  dParams = new deployParams();

  newSequence: Subject<number> = new Subject<number>();
  bsValueDate: Date = new Date();

  optionSelected='';
  
  searchOptions = [
      {name:'Application', value:'applicationNo'},
      {name:'Candidate Name', value:'candidateName'},  
      {name:'Order No', value:'orderNo'},  
      {name:'Category Name', value:'categoryName'},  
      {name:'Date Selected', value:'selectedon'},  
      {name:'Employer Name', value:'customerName'},
      {name:'Current Status', value:'currentStatus'}
  ]

  sortOptions = [
    {name:'By Application No Asc', value:'appno'},
    {name:'By Application No Desc', value:'apppnodesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Employer', value:'employer'},
    {name:'By Employer Desc', value:'employerdesc'}
  ]



 SEQUENCE_SELECTED = 100; SEQUENCE_SELECTED_NAME="Selected";
 SEQUENCE_DOCUMENTS_ATTESTATION=200; SEQUENCE_DOCUMENTS_ATTESTATION_NAME="Document Certification Initiated";
  SEQUENCE_MED_REFERRED=300; SEQUENCE_MED_REFERRED_NAME="Referred For Medical Test";
  SEQUENCE_MED_FIT=400; SEQUENCE_MED_FIT_NAME = "Medically Fit";
 SEQUENCE_MED_UNFIT=500; SEQUENCE_MED_UNFIT_NAME = "Medically Unfit";
 SEQUENCE_VISA_DOC_SUBMITTED=600; SEQUENCE_VISA_DOC_SUBMITTED_NAME = "Visa Documents submitted";
 SEQUENCE_VISA_ISSUED=700;SEQUENCE_VISA_ISSUED_NAME = "Visa Issued";
 SEQUENCE_VISA_REJECTED=800; SEQUENCE_VISA_REJECTED_NAME = "Visa Rejected";
 SEQUENCE_EMIG_APP_LODGED=900; SEQUENCE_EMIG_APP_LODGED_NAME = "Emigration application lodged";
 SEQUENCE_EMIG_DOC_LODGED=1000;SEQUENCE_EMIG_DOC_LODGED_NAME = "Emigration Documents lodged";
 SEQUENCE_EMIG_CLEARED=1100; SEQUENCE_EMIG_CLEARED_NAME = "Emigration Cleared";
 SEQUENCE_EMIG_DENIED=1200; SEQUENCE_EMIG_DENIED_NAME = "Emigration Denied";
 SEQUENCE_TRAVEL_BOOKED=1300; SEQUENCE_TRAVEL_BOOKED_NAME = "Ticket booked";
 SEQUENCE_TRAVELED=1500;SEQUENCE_TRAVELED_NAME = "Traveled";

  constructor(
      private toastr: ToastrService, private confirm: ConfirmService
      , private bsModalService: BsModalService, private activatedRoute: ActivatedRoute
      , private service: DeployService, 
      private router: Router) 
    { 
      
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
            
        }
    }

  ngOnInit(): void {
    this.deploysSelected = [];
    this.title = "active";

    this.activatedRoute.data.subscribe(data => {
      this.deploys = data['deps'].result,
      this.pagination = data['deps'].pagination,
      this.totalCount = data['deps'].count,
      this.statusNameAndSeq = data['statusNameAndSeq'],
      this.depStatuses = data['depStatuses']
    })
  }

  onPageChanged(event: any){
    const params = this.service.getParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event.page;
      this.service.setParams(params);

      this.getDeployments();
    }
  }

  getDeployments() {
  
    var params = this.service.getParams();

    this.service.getDeploymentPagedList(params)?.subscribe({
      next: response => {
        if(response !== undefined && response !== null) {
          this.deploys = response.result;
          this.totalCount = response?.count;
          this.pagination = response.pagination;
        } else {
          console.log('response is undefined');
        }
      },
      error: error => console.log(error)
    })
  }

  close() {
    this.currentCVRefId=0;    //closes the app-deployments selector
  }
  
  deletDeployment(event: any) {
    
    var id = event;

    const observableOuter = this.confirm.confirm('confirm Delete', 
      'Are you sure you want to delete this deployment?  All related transactions will also be deleted');

    observableOuter.pipe(
      filter((response) => response),

      switchMap(confirmed => this.service.deleteDeployment(id).pipe(
        catchError(err => {
          this.toastr.error(err, 'failed to delete');
          return of();
        }),
        tap(res => {
          this.toastr.error('Deleted the deployment', 'success in deletion')
        }),
        catchError(err => {
          this.toastr.error(err, 'Failed to delete the deployment record');
          return of();
        })
      ))
    ).subscribe(
      () => this.toastr.success('deployment deleted in subscribe', 'success')
    )
  }
      
  onSearch() {
    const params = this.service.getParams();
    var search = this.searchTerm!.nativeElement.value;
   
    if(this.currentSearchValue == search && this.optionSelected == this.currentOption
        && this.currentStatus == this.deployStatus ) {
          this.toastr.warning('No valid selection', 'Bad Request');
          return;
    }

    this.currentStatus = this.deployStatus;
   
    //if(this.currentSearchValue === search) return;
    this.currentSearchValue = search;
    
    switch (this.optionSelected) {
      case "applicationNo":
        params.applicationNo=search;  
        break;
      case "orderNo":
        params.orderNo = search;
        break;
      case "selectedOn":
        params.selectedOn = new Date(search);
        break;
      case "customerName":
        params.customerName = search;
        break;
      case "categoryName":
        params.categoryName = search;
        break;
      case "candidateName":
        params.candidateName = search;
        break;
      case "currentStatus":
        params.currentStatus = search;
        break;
      default:
        break;
    }

    this.title = this.optionSelected + "=" + search;
    if(this.deployStatus ==='') this.deployStatus = "Concluded";
    params.status=this.deployStatus;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.getDeployments();
  }

  onOptionSelected(option: any) {
      if(this.optionSelected === option) return;

      this.optionSelected = option;
      this.currentOption = option;

  }

  optionCleared() {
    this.optionSelected = '';
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sequenceSelected = '';
    this.dParams = new deployParams();
    this.service.setParams(this.dParams);
    this.getDeployments();
  }
  
  editDeploymentModal(dep: any, item: IDeploymentPendingBriefDto){

    if(dep === null) {
      this.toastr.warning('No Deployment object returned from the modal form');
      return;
    }  

    const config = {
        class: 'modal-dialog-centered modal-md',
        initialState: {
          dep,
          depStatusAndNames: this.statusNameAndSeq,
          candidateName: item.applicationNo + '-' + item.candidateName,
          categoryRef: item.categoryName,
          companyName: item.customerName
        }
      }

      this.bsModalRef = this.bsModalService.show(DeployEditModalComponent, config);

      const observableOuter =  this.bsModalRef.content.updateDep;
      
      observableOuter.pipe(
        filter((response: IDep) => response !==null),
        switchMap((response: IDep) =>  {
          return this.service.updateDeployment(response)
        })
      ).subscribe((response: IDeploymentPendingBriefDto) => {
  
        if(response !== null) {
          this.toastr.success('Deployment updated', 'Success');
          var index=this.deploys.findIndex(x => x.depId===response.depId);
          if(index !== -1) this.deploys[index]=response;
  
        } else {
          this.toastr.warning(response, 'Failure');
        }
        
      })
        
  }
  
  adddNewDeployment(dto: IDeploymentPendingDto)
  {
      const config = {
          class:'modal-dialog-centered modal-lg',
          initialState: {
            cvRefId: dto.cvRefId,
            depStatuses: this.depStatuses,
            candidateName: dto.applicationNo + '-' + dto.customerName,
            categoryRef: dto.categoryName,
            companyName : dto.customerName,
            ecnr: dto.ecnr
          }
        };
    
        this.bsModalRef = this.bsModalService.show(DeployAddModalComponent, config);
        const observableOuter =  this.bsModalRef.content.updateDep;

        observableOuter.pipe(
          filter((response: IDepItem) => response !==null),
          switchMap((response: IDepItem) =>  {
            return this.service.updateDeploymentItem(response)
          })
        ).subscribe((response: string) => {
  
        if(response==='') {
          this.toastr.success('Deployment Item inserted', 'Success');
  
        } else {
          this.toastr.warning(response, 'Failure');
        }
        
      })
  }
  
  editAttachmentModal(dep: any, item: IDeploymentPendingBriefDto){

    if(dep === null) {
      this.toastr.warning('No Deployment object returned from the modal form');
      return;
    }  

    const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          dep,
          depStatusAndNames: this.statusNameAndSeq,
          candidateName: item.applicationNo + '-' + item.candidateName,
          categoryRef: item.categoryName,
          companyName: item.customerName
        }
      }

    this.bsModalRef = this.bsModalService.show(DepAttachmentModalComponent, config);

    this.bsModalRef.content.updateDep.subscribe({
      next: () => this.toastr.info('attachment process completed', 'Process Over')
    })
    
}

  verifyNextSequence(existingSeq: string, nextSeqProposed: string): string {
    
    var thisStage = this.depStatuses.filter(x => x.statusName == existingSeq)[0];
    var nextStageProposed = this.depStatuses.filter(x => x.statusName == nextSeqProposed)[0];
    var ecnrs = this.deploysSelected.filter(x => x.ecnr);
    
    var err="";
    if(thisStage==null) return "Cannot find this.Stage";
    
    switch(thisStage.sequence) {
        case this.SEQUENCE_MED_REFERRED:
            err = nextSeqProposed===this.SEQUENCE_MED_FIT_NAME || nextSeqProposed===this.SEQUENCE_MED_UNFIT_NAME
              ? "" : "Valid Sequence after Medical Referred is: MEDICALLY FIT or MEDICALLY UNFIT";
            break;
          
        case this.SEQUENCE_MED_FIT:
          err = nextSeqProposed === this.SEQUENCE_VISA_DOC_SUBMITTED_NAME 
            ? "Sequence after Medical Fitness is Visa Documents submision": "";
          break;
          
        case this.SEQUENCE_VISA_ISSUED:  //next: Emig or Tkt
            err = nextSeqProposed===this.SEQUENCE_EMIG_APP_LODGED_NAME && ecnrs.length > 0 
              ? "Atleast one of the selected candidates are ECNR, and does not need Emigration formalities"
              : nextSeqProposed===this.SEQUENCE_TRAVEL_BOOKED_NAME && ecnrs.length === 0 
              ? "Emigration Clearance can be bye-passed only for ECNR candidates"
              : "";

            break;
          
        case this.SEQUENCE_EMIG_APP_LODGED:
            err = nextSeqProposed !== this.SEQUENCE_EMIG_DOC_LODGED_NAME 
              ? "Sequence to follow Emigration application Lodgement is"
              : "EMIGRATION Documents Lodgement"
            break;
        
        case this.SEQUENCE_EMIG_DOC_LODGED:
            err = nextSeqProposed === this.SEQUENCE_EMIG_CLEARED_NAME || nextSeqProposed === this.SEQUENCE_EMIG_DENIED_NAME
              ? ""
              : "Sequence after Emigration Document Lodgement is either Emigration Cleared Or Emigration Denied"
            
            break;
        case this.SEQUENCE_TRAVEL_BOOKED:
            err = nextSeqProposed === this.SEQUENCE_TRAVELED_NAME ? "" : "Sequence after Travel booked is TRAVELED";
            break;
        
        case this.SEQUENCE_EMIG_CLEARED:
              err = nextSeqProposed === this.SEQUENCE_TRAVELED_NAME ? "" : "Sequence after Travel booked is TRAVELED";
              break;
            default:
          break;
    }
    
    return err;
  }

  navigateByRoute(id: number, routeString: string, editable: boolean, obj: any[]) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            toedit: editable, 
            returnUrl: '/processing/list',
            depStages: obj
          } }
      );
  }

  getNextSeqForNextTransaction(seq: number) {
    var nextSeq = this.depStatuses.find(x => x.sequence == seq)?.nextSequence ?? 0;
    return nextSeq;    
  }

  getSequenceForNextTransaction(dep: IDeploymentPendingDto): number {

    var currentSeq = this.findMaxSeq(dep);
    var seqForNextTrasaction = this.depStatuses.find(x => x.sequence == currentSeq)?.nextSequence;
    return seqForNextTrasaction ?? 0;    
  }
  
  findMaxSeq(dep: IDeploymentPendingDto){
    //first row has the max seq, by design/.;
    var t= dep.deploySequence;
    
    return t;
  }

  getNextStageDateForNextTransaction(dep: IDeploymentPendingDto, nextSeq: number) {
    var lastDt: Date = new Date(dep.currentSeqDate);
    
    var days = this.depStatuses.find(x => x.sequence==  nextSeq)?.workingDaysReqdForNextStage;
    
    return new Date(lastDt.setDate(lastDt.getDate() + days!));

  }

  selectionChanged(item: any) { //item is IDeploymentPendingDto
    
    if(item === undefined) return;
        
    var checked=false;

    checked=item.checked!==true;

    //var found= this.deploys.find(x => x.depId === item.depId);
    var foundIndex = this.deploysSelected.findIndex(x => x.depId===item.depId);

    checked ? this.deploysSelected.push(item) : this.deploysSelected.splice(foundIndex,1);
    
  }

  getTravellingTicket(): IFlightdata | undefined {
      
    var dt = new Date();
    dt.setDate(dt.getDate()+4);
    var flight: IFlightdata = {etD_Boarding:dt, etA_Destination:dt, airlineName:"Air India", 
        airportOfBoarding: "Mumbai", airportOfDestination: "Jeddah", flightNo: "AI-823",
        etA_Via:null, etD_Via: null, flightNoVia:"", airportVia:"", airlineVia:"",
        etA_DestinationString:'', etD_BoardingString:'', etA_ViaString: '', etD_ViaString:''};
    
    const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          flightData: flight
        }
    }
 
    this.bsModalRef = this.bsModalService.show(ChooseFlightModalComponent, config);

    this.bsModalRef.content.candidateFlightEvent.subscribe({
      next: (response: IFlightdata|undefined) => {
        return response;
      },
      error: (err: any) => {
        console.log(err);
        return undefined;
      }
    })
    return undefined;
  }

  verifyTransactionData(): string {

    var str = "";

    if(this.sequenceSelected === '' ) return 'Please select the deployment transaction you want to apply';
    
    if(this.deploysSelected.length ===0) return 'Please select the items that you want to apply the transactions';
    
    //verify items selected carry same deployment sequence
    const distincts = [...new Set(this.deploysSelected.map(item => item.currentStatus))]; //.deploySequence))]; // [ 'A', 'B'];
    const distinctCurrentSeqs = [...new Set(distincts)]; //.deploySequence))]; // [ 'A', 'B'];
    console.log('distinctcurrentseq', distinctCurrentSeqs);
    if(distinctCurrentSeqs.length > 1) return "You can apply a common next sequence to items having same current sequence";
    
    const distinctCurrentECNRs = [...new Set(this.deploysSelected.map(item => item.currentStatus))];  // .deploySequence))]; // [ 'A', 'B'];
    
    if(distinctCurrentECNRs.length > 1) return "You may not select items having different values of ECNR - if an item has ECNR = true, then there are different yard stick available for next sequence applicability";
    
    //verify sequence selected is valid - there is a specific order of sequences allowed in table deploysselected;
    
    var proposedSeq=this.depStatuses.filter(x => x.sequence === +this.sequenceSelected)[0];
    
    var proposedSeqName = "";
    if(!proposedSeq) return "failed to get proposed seq ";
    proposedSeqName = proposedSeq.statusName;
    
    console.log('currentSeq:', distinctCurrentSeqs[0], 'proposed Seq:', proposedSeqName);

    var err = this.verifyNextSequence(distinctCurrentSeqs[0], proposedSeqName);
    if(err !== '') return err;

    var cities = [... new Set(this.deploysSelected.map(x => x.cityOfWorking))];

    if(this.sequenceSelected=== this.SEQUENCE_TRAVEL_BOOKED_NAME && cities.length > 1) return 'The deployment items selected by you have more than one destination airports to travel to.' 
      + ' Pl select the group with common destination airport, with common airport of Departure also.  Else, ' 
      + 'choose one candidate at a time';

    //TRANSACTION DATE FOLLOWS LAST TRANSACTION DATE?
    const lastDATE = Math.max(...this.deploysSelected.map(date => new Date(date.transactionDate).getTime()));
    //const lastTDt = new Date(Math.max(...this.deploysSelected.map(x => x.transactionDate.getTime())));
    err = lastDATE > new Date(this.transDate).getTime()
      ? "The selected Transaction date '" + this.transDate + "' is earlier than the last recorded transaction date " + 
        " of " + lastDATE
      : "";

    return err;

  }

  constructDepItemsToAdd(): IDepItemToAddDto[] {
    
    var id = this.depStatuses.find(x => x.statusName === this.sequenceSelected)?.id!;
    var depItemsToInsert: IDepItemToAddDto[]=[];
    console.log('id', this.sequenceSelected, id);

    this.deploysSelected.forEach(x => {
      var depitem: IDepItemToAddDto = {
        depId: x.depId, transactionDate : this.transDate,
        sequence: +this.sequenceSelected};

      depItemsToInsert.push(depitem);
    })

    return depItemsToInsert;
  }

  applyTransactions(): string {
  
    var errStr = this.verifyTransactionData();
    if(errStr !== '') {
      this.toastr.warning(errStr, 'Error in the proposed transactions');
      return errStr;
    }

    //all checks done, now call api.  the Api will further verify if nextSequence applied is valid.  it is not supposed to trust the client
    let depItemsToInsert: IDepItemToAddDto[]=this.constructDepItemsToAdd();
    
    if(depItemsToInsert===null || depItemsToInsert.length===0) return 'failed to construct Dep Items';

    if(this.sequenceSelected===this.SEQUENCE_TRAVEL_BOOKED_NAME) {    //ticket booking
        var flightItems: ICandiateFlightItem[]=[];
        
        this.deploysSelected.forEach(x => {
          var fltItem: ICandiateFlightItem={candidateName:x.candidateName, applicationNo: x.applicationNo,
              depId: x.depId, cvRefId: x.cvRefId, customerName: x.customerName, customerCity: x.cityOfWorking,
              id:0, depItemId:0, categoryName: x.categoryName};

          flightItems.push(fltItem);
          })
      
          //var tkt: IFlightdata|undefined;
          var dt = this.transDate;
          //dt.setDate(dt.getDate()+4);
          var flight: IFlightdata = {etD_Boarding:dt, etA_Destination:dt, airlineName:"Air India", 
              airportOfBoarding: "Mumbai", airportOfDestination: this.deploysSelected[0].cityOfWorking, 
              flightNo: "AI-823", etA_Via:null, etD_Via: null, flightNoVia:"", airportVia:"", airlineVia:"",
              etA_DestinationString:'', etD_BoardingString:'', etA_ViaString:'', etD_ViaString:''};

          const config = {
              class: 'modal-dialog-centered modal-lg',
              initialState: {
                flightData: flight
              }
          }
  
          this.bsModalRef = this.bsModalService.show(ChooseFlightModalComponent, config);
          const observableOuter =  this.bsModalRef.content.candidateFlightEvent;

        observableOuter.pipe(
          filter((tkt:IFlightdata) => tkt !==null),
          switchMap((tkt: IFlightdata) => {
            this.deploysSelected.forEach(x => {     //candidate flight is same, except candidate details differ with each object
              var candFlight: ICandidateFlightGrp = {
                  id: 0, dateOfFlight: tkt!.etD_Boarding, airlineName: tkt!.airlineName, flightNo: tkt!.flightNo, 
                  airportOfBoarding: tkt!.airportOfBoarding, 
                  airportOfDestination: tkt!.airportOfDestination, etA_Destination: tkt!.etA_Destination, 
                  fullPath: '', fileToUpload: null, orderNo: x.orderNo, candidateFlightItems: flightItems,
                  customerId: 0, 
                  etD_Boarding: tkt!.etD_Boarding, etD_BoardingString: tkt!.etD_BoardingString, 
                  etA_DestinationString: tkt!.etA_DestinationString, flightNoVia: tkt!.flightNoVia, 
                  airportVia: tkt!.airportVia, etA_Via: tkt!.etD_Via, 
                  etA_ViaString:tkt!.etA_ViaString, etD_ViaString:tkt.etD_ViaString};
              this.candFlight = candFlight;
          });

          var depitemsandcandcandidateFlt: IDepItemsAndCandFlightsToAddDto = {
            candFlightToAdd: this.candFlight!,
            depItemsToAdd: depItemsToInsert
          };

          return this.service.InsertDepItemsAndCandFlights(depitemsandcandcandidateFlt)
        })
        ).subscribe((response: IDeploymentPendingBriefDto[]) => {
            if(response.length > 1) this.UpdateDeploymentDOM(response);
        }) 
      
      } else {
        this.service.InsertDepItems(depItemsToInsert).subscribe({
          next: (itemsAdded:IDeploymentPendingBriefDto[]) => {
            if(itemsAdded === null || itemsAdded.length === 0) {      //returns DeploymentPendngDto[]
                this.toastr.warning('Failed to insert', 'Failed to insert any dep Transaction');
                this.sequenceSelected = '';
                depItemsToInsert=[];
    
                this.deploys.forEach(element => {
                  element.checked===false;
                }); 
    
                return "failed to insert dep transactions";
          
              } else {
                  this.UpdateDeploymentDOM(itemsAdded);
                  return "";
              }
            }, error: (err:any) => this.toastr.error(err.error.details, 'Error encountered')
          
          })
      }

    return "";
  }
  
  displayVisaTransaction(cvrefId: number) {
    this.navigateByRoute(cvrefId, '/visas/visaTransactionByCVRefId',false, []);
  }

  
  UpdateDeploymentDOM(briefDto: IDeploymentPendingBriefDto[]) {
    for(const x of briefDto) {
      var index = this.deploys.findIndex(y => y.depId == x.depId);
      if(index !==-1) {
        this.deploys[index].currentStatus = x.currentStatus;
        this.deploys[index].checked = false;
      }
    }

    this.deploysSelected=[];
    this.sequenceSelected='';

    this.toastr.success('Success', 'Inserted ' + briefDto.length + ' number of deployment transactions');
  }

  selected(value:any):void {
    this.sequenceSelected = value.currentStatus;
  }

  showTicketClicked(event: any) {   //value emitted is CVRefId
    
  }

  candidateFlightDelete(candidateFlightId: any) {
    var id: number = candidateFlightId;

    this.service.deleteCandidateFlight(id).subscribe((response: boolean) => {
      if(response) {
        this.toastr.success('Candidate Flight Detail deleted', 'Deleted successfully')
      } else {
        this.toastr.warning('Failed to delete the Candidate Flight', 'Failed to delete')
      }
    })
  }

  candidateFlightEdit(event: any) {   //ICandidateFlight
      var candFlight = event;
      if(candFlight==null) {
        this.toastr.warning('null object received from CAndidate Flight modal');
        return;
      }

      this.service.editCandidateFlight(candFlight).subscribe({
        next: response => {
          if(response =='') {
            this.toastr.success('The Candidate Flight was successfully updated', 'Success')
          } else {
            this.toastr.warning(response, 'failed to update the candidate flight object')
          }
        }
      })
  }  
  
  filterData() {
    var stStatus = this.statusNameAndSeq.filter(x => x.name === this.sequenceSelected).map(x => x.name)[0];
    this.deploys = this.deploys.filter(x => x.currentStatus === stStatus);
  }

  houseKeeping() {

    this.service.housekeeping().subscribe({
      next: (response: IReturnStringsDto) => {
        if(response.successString !== '') {
          this.toastr.success(response.successString, 'Success')
        }
      },
      error: (err: any) => this.toastr.error(err.error.details, 'Error')
    })
    
  }
}   

