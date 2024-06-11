import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICandidateBriefDto } from 'src/app/_dtos/admin/candidateBriefDto';
import { IOrderItemBriefDto } from 'src/app/_dtos/admin/orderItemBriefDto';
import { ICustomerNameAndCity } from 'src/app/_models/admin/customernameandcity';
import { IProfession } from 'src/app/_models/masters/profession';
import { Pagination } from 'src/app/_models/pagination';
import { OpenOrderItemsParams } from 'src/app/_models/params/Admin/openOrderItemsParams';
import { candidateParams } from 'src/app/_models/params/hr/candidateParams';
import { User } from 'src/app/_models/user';
import { OrderService } from 'src/app/_services/admin/order.service';
import { CandidateService } from 'src/app/_services/candidate.service';
import { UploadDownloadService } from 'src/app/_services/upload-download.service';
import { IdsModalComponent } from 'src/app/modals/ids-modal/ids-modal.component';

@Component({
  selector: 'app-profile-list',
  templateUrl: './profile-list.component.html',
  styleUrls: ['./profile-list.component.css']
})
export class ProfileListComponent implements OnInit{

  @ViewChild('search', {static: false}) searchTerm?: ElementRef;
  
  pagination: Pagination | undefined;

  cvs: ICandidateBriefDto[]=[];
  selectedCVs: ICandidateBriefDto[]=[];

  cvParams: candidateParams = new candidateParams();

  totalCount: number=0;
  //candidateCities: ICandidateCity[]=[];
  professions: IProfession[]=[];
  existingQBankCategories: IProfession[]=[];
  agents: ICustomerNameAndCity[]=[];
  bsModalRef: BsModalRef | undefined;

  idFromChild: number=0;

//ngSelect
  selectedProfIds: number[]=[];
  events: Event[] = [];

  documentLoading=false;

  sortOptions = [
    {name:'By Application No Asc', value:'appno'},
    {name:'By Application No Desc', value:'apppnodesc'},
    {name:'By City Asc', value:'city'},
    {name:'By City Desc', value:'citydesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Agent', value:'agent'},
    {name:'By Agent Desc', value:'agentdesc'}
  ]

  constructor(
      private service: CandidateService, 
      //private accountService: AccountService,
      private router: Router,
      private activatedRoute: ActivatedRoute, 
      private modalService: BsModalService,
      private orderService: OrderService, 
      private downloadservice: UploadDownloadService,
      private toastr: ToastrService) { 

        //this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);

        this.cvParams = new candidateParams();
        this.service.setCVParams(this.cvParams);
      }

  ngOnInit(): void {

    this.loadCandidates(false);
    
    this.activatedRoute.data.subscribe(data => {
        //this.cvs = data.candidateBriefs,
        //this.totalCount = data.candidateBriefs.count,
        this.professions = data['professions'],
        this.agents = data['agents']
      }
    )

    //this.getCities();
    //this.getProfessions();
    //this.getAgents();

  }

  loadCandidates(fromCache: boolean=true) {
    if(this.cvParams) {

      this.service.setCVParams(this.cvParams);

      this.service.getCandidatesPaged(this.cvParams, fromCache).subscribe({
        next: response => {
          if(response.result && response.pagination) {
            this.cvs = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }

  }

  getCVs() {
   
    if(this.cvParams)   {
      this.service.setCVParams(this.cvParams);

        this.service.getCandidatesPaged(this.cvParams, true).subscribe({
          next: response => {
            if(response.result && response.pagination) {
              this.cvs = response.result;
              this.totalCount = response.count;
            }
          }
        })
    }
  }

  onSearch() {
    const params = this.service.getCVParams() ?? new candidateParams();
    
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setCVParams(params);
    this.getCVs();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.cvParams = new candidateParams();
    this.service.setCVParams(this.cvParams);
    this.getCVs();
  }
  
   onPageChanged(event: any){

      if(!this.cvParams) return;

      if(this.cvParams.pageNumber !== event.page) {
        this.cvParams.pageNumber = event.page;
        this.service.setCVParams(this.cvParams);
        this.getCVs();
      }
    }
  

  //Having selected candidates, refer them to internal reviews or directly to client
  openChecklistModal(user: User) {
    const title = 'Choose Order Item to refer selected CVs to';
    var returnvalue:any;
    var ids: number[]=[];
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        title,
        orderItems: this.getOpenOrderItemsArray(),
        ids
      }
    }
    this.bsModalRef = this.modalService.show(IdsModalComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe((values: number[]) => {
      ids = values;
      if (ids.length) {
        //this.service.submitCVsForReview().subscribe(() => {
      //          user.roles = [...rolesToUpdate.roles]
        //}
        //)
      }
    })
  }
  
  onAgentSelected(agentId: any) {
    const prms = this.service.getCVParams() ?? new candidateParams();
    prms.agentId = agentId;
    prms.pageNumber=1;
    this.service.setCVParams(prms);
    this.getCVs();



  }

  private getOpenOrderItemsArray(): IOrderItemBriefDto[] {
    const roles: any[] = [];
    let aitems: IOrderItemBriefDto[]=[];
    let aitem: IOrderItemBriefDto;
    let openItemsParams=new OpenOrderItemsParams();
    this.orderService.getOrderItemsBriefDto(openItemsParams).subscribe((response: IOrderItemBriefDto[]) => {
      aitems = response;
      if(aitems.length===0) return;
      return aitems;
    }, (error: any) => {
      console.log('failed to retrieve roles array', error);
    })
    return aitems;
  }

  //output value from child
 /* showDocumentViewerEvent(id: number){
    this.idFromChild = id;
    return this.service.viewDocument(id).subscribe(result => {
      this.loadInitialDocument(result);
    }, error => {
      this.toastr.error(error);
    })
    
  }
  */

  downloadFileEvent(candidateid: any) {
    return this.downloadservice.downloadFile(candidateid).subscribe(() => {
      this.toastr.success('document downloaded');
    }, (error: any) => {
      this.toastr.error('failed to download document', error);
    })
  }

  cvAssessEvent(cvbrief: any)
  {
    this.navigateByUrl('/hr/cvassess/' + cvbrief.id, cvbrief, false);
  }

  navigateByUrl(route: string, cvObject: any|undefined, toedit: boolean) {
    this.router.navigate(
      [route],
      { state: 
        { 
          cvbrief: cvObject, 
          //openorderitems,
          //assessmentsDto,
          userobject: this.service.user,
          toedit: toedit, 
          returnUrl: '/candidates' 
        } }
    )
  }

  cvCheckedEvent(cvbrief: any){   // ICandidateBriefDto) {
      var cv = this.selectedCVs.find(x => x.id==cvbrief.id);
      var index = this.selectedCVs.findIndex(x => x.id===cvbrief.id);

      if(cv !==undefined) {
        if(!cv.checked) {
          this.selectedCVs.splice(index,1);
        } else {
          this.selectedCVs.push(cvbrief);  
        }
      } else {
        this.selectedCVs.push(cvbrief);
      }

  }

  cvEditClicked(id: any) {
    
    this.navigateByUrl('/candidates/register/edit/' + id, undefined, true);
  }

  cvAssessClicked() {
    this.navigateByUrl('/hr/assessments', this.selectedCVs, false);
  }

}
