import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { ICvsAvailableDto } from 'src/app/_dtos/admin/cvsAvailableDto';
import { IOrderItemBriefDto } from 'src/app/_dtos/admin/orderItemBriefDto';
import { ICustomerNameAndCity } from 'src/app/_models/admin/customernameandcity';
import { IProfession } from 'src/app/_models/masters/profession';
import { Pagination } from 'src/app/_models/pagination';
import { OpenOrderItemsParams } from 'src/app/_models/params/Admin/openOrderItemsParams';
import { candidateParams } from 'src/app/_models/params/hr/candidateParams';
import { User } from 'src/app/_models/user';
import { OrderService } from 'src/app/_services/admin/order.service';
import { CandidateService } from 'src/app/_services/candidate.service';
import { CandidateAssessmentService } from 'src/app/_services/hr/candidate-assessment.service';
import { CvrefService } from 'src/app/_services/hr/cvref.service';
import { UploadDownloadService } from 'src/app/_services/upload-download.service';
import { CvAssessModalComponent } from 'src/app/hr/cv-assess-modal/cv-assess-modal.component';
import { IdsModalComponent } from 'src/app/modals/ids-modal/ids-modal.component';

@Component({
  selector: 'app-cvs-available-to-ref',
  templateUrl: './cvs-available-to-ref.component.html',
  styleUrls: ['./cvs-available-to-ref.component.css']
})
export class CvsAvailableToRefComponent implements OnInit {

  
  @ViewChild('search', {static: false}) searchTerm?: ElementRef;
  
  professions: IProfession[]=[];
  agents: ICustomerNameAndCity[]=[];
  
  pagination: Pagination | undefined;
  bsModalRef: BsModalRef | undefined;

  cvs: ICvsAvailableDto[]=[];
  selectedCVs: ICvsAvailableDto[]=[];

  user: User | undefined;
  returnUrl = '';

  cvParams: candidateParams = new candidateParams();

  totalCount: number=0;
  
  constructor(private router: Router, private activatedRoute: ActivatedRoute, 
      private service: CandidateService, 
      private candAssessService: CandidateAssessmentService,
      private referService: CvrefService,
      private toastr: ToastrService, 
      private modalService: BsModalService, 
      private orderService: OrderService,
      private downloadService: UploadDownloadService) {
        this.cvParams.typeOfCandidate="available";

        let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
              if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

              if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
          }
      }

  ngOnInit(): void {
    
    this.loadCandidates(false);
    
    this.activatedRoute.data.subscribe(data => {
        this.professions = data['professions'],
        this.agents = data['agents']
      }
    )

    this.selectedCVs=[];

  }

  loadCandidates(fromCache: boolean=true) {
    if(this.cvParams) {

      this.service.setCVParams(this.cvParams);
      this.service.getAvailableCandidatesPaged(this.cvParams, fromCache).subscribe({
        next: response => {
          if(response.result && response.pagination) {
            this.cvs = response.result;
            this.pagination = response.pagination;
            console.log('cvs', this.cvs);
          }
        }
      })

      
    }

  }

  getCVs() {
   
    if(this.cvParams)   {
      this.service.setCVParams(this.cvParams);

        this.service.getAvailableCandidatesPaged(this.cvParams, true).subscribe({
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

  
  downloadFileEvent(candidateid: any) {
    return this.downloadService.downloadFile(candidateid).subscribe(() => {
      this.toastr.success('document downloaded');
    }, (error: any) => {
      this.toastr.error('failed to download document', error);
    })
  }

  displayAssessmentModal(cvbrief: any)
  {
        var orderitemid = cvbrief.orderItemId;
        var candidateid = cvbrief.candidateId;

        this.candAssessService.getCandidateAssessmentDto(candidateid, orderitemid)
          .subscribe(response => {
            if(response === null) {
              this.toastr.warning('failed to retrieve the assessment object');
            } else {
                const config = {
                  class: 'modal-dialog-centered modal-lg',
                  initialState: {
                    assess: response
                  }
                }
                this.bsModalRef = this.modalService.show(CvAssessModalComponent, config);
                this.bsModalRef.content.candAssessEvent.subscribe(() => {
                  console.log('succeeded');
                })
            }
          })
    
  }

  displayAssessmentModalBySwitchMap(cvbrief: any) {

    var orderitemid = cvbrief.orderItemId;
    var candidateid = cvbrief.candidateId;

    const observableOuter = this.candAssessService.getCandidateAssessmentDto(candidateid, orderitemid);
    
    observableOuter.pipe(
      filter((response) => response !==undefined),
      switchMap((response) => {
        const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
            assess: response,
            username: this.user?.userName
          }
        }
        
        this.bsModalRef = this.modalService.show(CvAssessModalComponent, config);
        const observableInner = this.bsModalRef.content.candAssessEvent;
        return observableInner
      })
    ).subscribe((response) => {
      console.log('inner response:', response );
    })

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
      var cv = this.selectedCVs.find(x => x.candidateId==cvbrief.id);
      var index = this.selectedCVs.findIndex(x => x.candidateId===cvbrief.id);

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

  referCVs() {

    if(this.selectedCVs.length === 0) {
      this.toastr.warning('No CVs selected to forward to client', 'No selections made');
      return;
    }

    //cvref needs cvAssessmentIds[]
    var ids = this.selectedCVs.map(x => x.candAssessmentId);

    this.referService.referCVs(ids).subscribe({
      next: (response: any) => {
        console.log('returned from api, cv ref:', response);

        if(response === '') {
          this.toastr.success('selected CVs referred, and CV Referral message available in Messages section for edits', 'success');
          this.cvs.forEach(x => {
            if(x.checked) x.checked=false;
          })
          this.selectedCVs=[];
        } else {
          this.toastr.warning('failed to refer the CVs', 'failed to refer CVs');
        }
      },
      error: (err: any) => this.toastr.error(err, 'Error in referring the CVs;')
    })
    
  }

  close() {
    this.router.navigateByUrl(this.returnUrl);
  }
}
