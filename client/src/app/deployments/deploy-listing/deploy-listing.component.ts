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
import { Subject, catchError, distinct, filter, from, of, switchMap, tap } from 'rxjs';
import { DeployAddModalComponent } from '../deploy-add-modal/deploy-add-modal.component';
import { IDepItem } from 'src/app/_models/process/depItem';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { IDepItemToAddDto } from 'src/app/_dtos/process/depItemToAddDto';
import { ChooseFlightModalComponent } from '../choose-flight-modal/choose-flight-modal.component';
import { IDepItemWithFlightDto } from 'src/app/_models/process/depItemsWithFlight';
import { ICandidateFlight } from 'src/app/_models/process/candidateFlight';
import { FlightDetailModalComponent } from '../flight-detail-modal/flight-detail-modal.component';
import { ICandidateFlightData } from 'src/app/_models/process/candidateFlightData';
import { IDeploymentStatus } from 'src/app/_models/masters/deployStatus';
import { DepAttachmentModalComponent } from '../dep-attachment-modal/dep-attachment-modal.component';

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

  deploys: IDeploymentPendingDto[]=[];
  deploysSelected: IDeploymentPendingDto[]=[]; 

  pagination: Pagination | undefined;

  routeId: string='';
  totalCount: number=0;
  
  depStatuses: IDeploymentStatus[]=[];
  statusNameAndSeq: IDeployStatusAndName[]=[];

  sequenceSelected: number=0;

  bsModalRef: BsModalRef | undefined;

  currentCVRefId: number=0;

  dParams = new deployParams();

  newSequence: Subject<number> = new Subject<number>();
  
  searchOptions = [
      {name:'Application', value:'applicationno'},
      {name:'Candidate Name', value:'candidateName'},  
      {name:'Order No', value:'orderNo'},  
      {name:'Category Name', value:'categoryName'},  
      {name:'Date Selected', value:'selectedon'},  
      {name:'Employer Name', value:'customerName'},
      {name:'Status', value:'status'}
  ]

  sortOptions = [
    {name:'By Application No Asc', value:'appno'},
    {name:'By Application No Desc', value:'apppnodesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Employer', value:'employer'},
    {name:'By Employer Desc', value:'employerdesc'}
  ]

 
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
    this.getDeployments();

    this.activatedRoute.data.subscribe({
      next: data => this.statusNameAndSeq = data['statusNameAndSeq']
    })
        
    this.getDeploymentStatuses();
  }

  getDeployStatusAndSeq() {
    this.service.getDepStatusAndNames().subscribe({
      next: (response) => this.statusNameAndSeq = response
    })
  }

  getDeploymentStatuses() {
    this.service.getDeployStatuses().subscribe({
      next: response => this.depStatuses = response,
      error: error => console.log(error)
    });
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
  
  deleteDeployment(event: any) {
    
    var id = event;

    const observableOuter = this.confirm.confirm('confirm Delete', 
      'Are you sure you want to delete this deployment?  All related transactions will also be deleted'
    );

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
      () => this.toastr.success('deployment deleted', 'success')
    )
  }
      
  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.getDeployments();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.dParams = new deployParams();
    this.service.setParams(this.dParams);
    this.getDeployments();
  }
  
 editDeploymentModal(dep: any, item: IDeploymentPendingDto){

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

      this.bsModalRef = this.bsModalService.show(DeployEditModalComponent, config);

      const observableOuter =  this.bsModalRef.content.updateDep;
      
      observableOuter.pipe(
        filter((response: IDep) => response !==null),
        switchMap((response: IDep) =>  {
          return this.service.updateDeployment(response)
        })
      ).subscribe((response: string) => {
  
        if(response==='') {
          this.toastr.success('Deployment updated', 'Success');
  
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

  
 editAttachmentModal(dep: any, item: IDeploymentPendingDto){

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

  verifyNextSequence(existingSeq: number, nextSeqProposed: number, ecnr: boolean): string {
    
    var thisStage = this.depStatuses.filter(x => x.sequence == existingSeq)[0];
    var nextStageProposed = this.depStatuses.filter(x => x.sequence == nextSeqProposed)[0];
    if(thisStage.nextSequence === nextStageProposed.sequence) return '';
    console.log('thisStage:', thisStage, 'nextStage:', nextStageProposed);
    if(ecnr && thisStage.sequence === 700 && nextSeqProposed === 1300) return '';
    
    return "Deployment Stage '" + thisStage.statusName + "' should follow with '" + 
      this.depStatuses.filter(x => x.sequence==thisStage.nextSequence).map(x => x.statusName) + "'";
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
    console.log('Next Sequence for next Transaction:', nextSeq);
    return nextSeq;    
  }

  getSequenceForNextTransaction(dep: IDeploymentPendingDto): number {

    var currentSeq = this.findMaxSeq(dep);

    var seqForNextTrasaction = this.depStatuses.find(x => x.sequence == currentSeq)?.nextSequence;
    console.log('next sequence:', seqForNextTrasaction);
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

  selectionChanged(item: any) {

    console.log('selection change:', item);
    
    if(item === undefined) {
        return;
    }
    
    var checked=false;

    checked=item.checked!==true;

    //var found= this.deploys.find(x => x.depId === item.depId);
    var foundIndex = this.deploysSelected.findIndex(x => x.depId===item.depId);

    checked ? this.deploysSelected.push(item) : this.deploysSelected.splice(foundIndex,1);
    
  }

  applyTransactions() {

    if(this.sequenceSelected === 0) {
      this.toastr.warning('Deployment Stage not selected', 'Please select the deployment transaction you want to apply');
      return;
    }
    if(this.deploysSelected.length ===0) {
      this.toastr.warning('Deployment items not selected', 'Please select the items that you want to apply the transactions');
      return;
    }
    //verify items selected carry same deployment sequence
    const distinctCurrentSeqs = [...new Set(this.deploysSelected.map(item => item.deploySequence))]; // [ 'A', 'B'];
    if(distinctCurrentSeqs.length > 1) {
      this.toastr.warning("You can apply a common next sequence to items having same current sequence", "error in selections");
      return of(null);
    }

    const distinctCurrentECNRs = [...new Set(this.deploysSelected.map(item => item.deploySequence))]; // [ 'A', 'B'];
    if(distinctCurrentECNRs.length > 1) {
        this.toastr.warning("You may not select items having different values of ECNR - if an item has ECNR = true, then there are different yard stick available for next sequence applicability", "Error");
        return of(null);
    }

    //verify sequence selected is valid - there is a specific order of sequences allowed in table deploysselected;
    var err = this.verifyNextSequence(distinctCurrentSeqs[0], this.sequenceSelected, this.deploysSelected[0].ecnr);
    if(err !== ''){
      this.toastr.warning(err, 'Invalid next transaction selected');
      return of(null);
    }

    //all checks done, now call api.  the Api will further verify if nextSequence applied is valid.  it is not supposed to trust the client
    let depItemsToInsert: IDepItemToAddDto[]=[];

    this.deploysSelected.forEach(x => {
        var depitem: IDepItemToAddDto = {
          id: 0, depId: x.depId, transactionDate : new Date(),
          sequence : this.sequenceSelected, nextSequence: 0,
          fullPath: ''};

        depItemsToInsert.push(depitem);
    })
    
    if(this.sequenceSelected === 1300) {    //tkt booked
        
      var cities = [... new Set(this.deploysSelected.map(x => x.cityOfWorking))];

      if(cities.length > 1) {
        this.toastr.warning('The deployment items selected by you have more than one destination airports to travel to.' + 
          ' Pl select the group with common destination airport, with common airport of Departure also.  Else, ' +
          'choose one candidate at a time','Incorrect selections for travel booking');
        
        return;
      }

        const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
            destinationAirport: cities[0],
          }
        }

        this.bsModalRef = this.bsModalService.show(ChooseFlightModalComponent, config);

        const observableOuter =  this.bsModalRef.content.updateFlight;    //returns a sngle object, not collection

        observableOuter.pipe(
          filter((response: ICandidateFlightData) => response !== undefined),
          switchMap((response: ICandidateFlightData) => {
            //console.log('response in listing:', response);
            if(response !==undefined) {
              var itemsWithflight: IDepItemWithFlightDto[]=[];
              var candidateFlights: ICandidateFlight[]=[];

              depItemsToInsert.forEach(x => {
                let depitem: IDepItem = {id: 0, depId: x.depId, sequence: x.sequence, 
                    nextSequence: x.nextSequence, transactionDate: x.transactionDate, 
                    nextSequenceDate: new Date(), fullPath: x.fullPath};
                                  
                let candFlightData: ICandidateFlight = {
                  id: 0, depId: x.depId, depItemId: 0, cvRefId: 0, applicationNo: 0, candidateName: '', customerName: '', customerCity: '', 
                  dateOfFlight: response.dateOfFlight, flightNo: response.flightNo, airportOfBoarding: response.airportOfBoarding,
                  airportOfDestination: response.airportOfDestination, eTD_Boarding: response.eTD_Boarding, 
                  eTA_Destination: response.eTA_Destination, airportVia: response.airportVia, flightNoVia: response.flightNoVia,
                  eTA_Via: response.eTA_Via, eTD_Via: response.eTD_Via
                }

                candidateFlights.push(candFlightData);
                
                let item: IDepItemWithFlightDto = {depItem: depitem, candidateFlight: candFlightData };
                itemsWithflight.push(item);
              })
  
              return this.service.insertDepItemsWithTravelBooked(itemsWithflight)
            } else {
              return of(undefined)
            }
            
          })
        ).subscribe((response: IDeploymentPendingDto[]) => {
            if(response !== null) {
              for(const x of response) {
                var index = this.deploys.findIndex(x => x.depId == x.depId);
                this.deploys[index] = x;  // Object.assign({}, x);
                this.newSequence.next(x.deploySequence);
            }
            this.toastr.success('Success', 'Inserted ' + response.length + ' number of deployment transactions');
            return;
            } else {
              return;
            }
        })
      //updateFlight: IFlightdata
    } else {
        this.service.InsertDepItems(depItemsToInsert).subscribe({
          next: (itemsAdded:IDeploymentPendingDto[]) => {
   
            if(itemsAdded === null || itemsAdded.length === 0) {      //returns DeploymentPendngDto[]
                this.toastr.warning('Failed to insert', 'Failed to insert any dep Transaction');
                this.sequenceSelected = -1;
                depItemsToInsert=[];
    
                this.deploys.forEach(element => {
                  element.checked===false;
                }); 
                
            } else {
                for(const x of itemsAdded) {
                    var index = this.deploys.findIndex(y => y.depId == x.depId);
                    if(index !==-1) {
                      this.deploys[index] = x;  // Object.assign({}, x);
                      this.newSequence.next(x.deploySequence);
                      x.checked=false;
                    }
                }
                this.toastr.success('Success', 'Inserted ' + itemsAdded + ' number of deployment transactions');
            }
          }, error: (err: any) => {
            this.toastr.error(err.error.details, 'Error encountered while adding deployment transaction')
          }
          
        })
    }
    
    return of(null);
  }

  selected(value:any):void {
    this.sequenceSelected = value;
  }

  public displayCandidateTicket(event: any) {   //value emitted is CVRefId

    var id=event;
    const observableOuter = this.service.getCandidateFlight(id);
    
    observableOuter.pipe(
      filter((response) => response !==undefined && response !==null),
      switchMap((response) => {
        const config = {
          class: 'modal-dialog-centered modal-md',
          initialState: {
            flight: response
          }
        }

        this.bsModalRef = this.bsModalService.show(FlightDetailModalComponent, config);
        const observableInner = this.bsModalRef.content.candidateFlightDelete;
        return observableInner
      })
    ).subscribe((response) => {
      console.log('inner response:', response );
    })
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
  
}   

