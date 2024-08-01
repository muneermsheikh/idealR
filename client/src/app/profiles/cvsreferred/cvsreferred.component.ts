import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, Navigation, Route, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { ISelPendingDto } from 'src/app/_dtos/admin/selPendingDto';
import { Pagination } from 'src/app/_models/pagination';
import { CVRefParams } from 'src/app/_models/params/Admin/cvRefParams';
import { User } from 'src/app/_models/user';
import { CandidateAssessmentService } from 'src/app/_services/hr/candidate-assessment.service';
import { CvrefService } from 'src/app/_services/hr/cvref.service';
import { getParamsNamesOfCVRefParams } from 'src/app/_services/paginationHelper';
import { UploadDownloadService } from 'src/app/_services/upload-download.service';
import { CvAssessModalComponent } from 'src/app/hr/cv-assess-modal/cv-assess-modal.component';

@Component({
  selector: 'app-cvsreferred',
  templateUrl: './cvsreferred.component.html',
  styleUrls: ['./cvsreferred.component.css']
})
export class CvsreferredComponent implements OnInit{

  @ViewChild('search', {static: false}) searchTerm?: ElementRef;
  
  pagination: Pagination | undefined;
  bsModalRef: BsModalRef | undefined;

  user: User | undefined;
  returnUrl = '';

  cvParams: CVRefParams = new CVRefParams();

  cvs: ISelPendingDto[]=[];
  selectedCVs: ISelPendingDto[]=[];

  totalCount: number=0;

  paramNames='';
  id=0;
  
  constructor(private router: Router,
      private service: CvrefService, 
      private toastr: ToastrService, 
      private modalService: BsModalService, 
      private candAssessService: CandidateAssessmentService,
      private downloadService: UploadDownloadService,
      private route: ActivatedRoute) {

        var routeid = this.route.snapshot.paramMap.get('id') ?? '0';
        this.id=+routeid;

        let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
              if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

              if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
          }
      }

  ngOnInit(): void {
    if(this.id!==0) this.cvParams.orderId=this.id;
    if(this.id! !== 0) this.cvParams.cVRefStatus='';
    this.loadCVsReferred();
    if(this.cvs.length > 0) this.paramNames=getParamsNamesOfCVRefParams(this.cvParams);
  }

  loadCVsReferred() {

      this.service.setParams(this.cvParams);
      this.service.referredCVsPaginated(true).subscribe({
        next: response => {
          if(response.result && response.pagination) {
            this.cvs = response.result;
            this.pagination = response.pagination;
          }
        }
      })
  

  }
 
  
  onSearch() {
    const params = this.service.getParams() ?? new CVRefParams();
    
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.loadCVsReferred();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.cvParams = new CVRefParams();
    this.service.setParams(this.cvParams);
    this.loadCVsReferred();
  }
  
   onPageChanged(event: any){

      if(!this.cvParams) return;

      if(this.cvParams.pageNumber !== event.page) {
        this.cvParams.pageNumber = event.page;
        this.service.setParams(this.cvParams);
        this.loadCVsReferred();
      }
    }
  
  
  downloadFileEvent(candidateid: any) {
    return this.downloadService.downloadFile(candidateid).subscribe(() => {
      this.toastr.success('document downloaded');
    }, (error: any) => {
      this.toastr.error('failed to download document', error);
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

  close() {
    this.router.navigateByUrl(this.returnUrl || '/candidates');
  }
}
