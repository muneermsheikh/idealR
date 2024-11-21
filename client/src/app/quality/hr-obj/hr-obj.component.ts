import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { IHrObjective } from 'src/app/_models/admin/objectives/hrObjective';
import { MedicalParams } from 'src/app/_models/admin/objectives/medicalParams';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { getWorkingDays } from 'src/app/_services/paginationHelper';
import { QualityService } from 'src/app/_services/quality.service';
import { convertDateToDateOnly } from 'src/app/finance/coa-list/coa-list.component';
import { DateInputRangeModalComponent } from 'src/app/modals/date-input-range-modal/date-input-range-modal.component';

@Component({
  selector: 'app-hr-obj',
  templateUrl: './hr-obj.component.html',
  styleUrls: ['./hr-obj.component.css']
})
export class HrObjComponent implements OnInit {

  objs: IHrObjective[]=[];
  pagination: Pagination|undefined;
  user?: User;
  mParams = new MedicalParams();
  totalCount=0;
  workingDays=0;

  fromDate = new Date();
  uptoDate = new Date();

  bsValueDate = new Date();
  dateRangeFrom: Date=new Date();
  dateRangeUpto: Date = new Date();

  bsModalRef : BsModalRef | undefined;

  subroute: string='';   //pending tasks or report


  constructor(private service: QualityService, private toastr: ToastrService, 
    private activatedRoute: ActivatedRoute, private modalService: BsModalService){
      this.subroute = this.activatedRoute.snapshot.params['subroute'];
    }

  ngOnInit(): void {
    
    if(this.subroute === 'pending') {
      this.getHRTasksPending();
    } else {
      this.getHRObjectivesPaged();
    }
  }

  getHRTasksPending() {

      this.service.getPendingHRTasks(this.mParams).subscribe({
        next: (response: any) => {
            this.objs = response.result;
            this.totalCount = response.count;
            this.pagination = response.pagination;
        }, 
        error: (err: any) => this.toastr.error(err.error?.details, 'Error encountered')
      })
  }

  getHRObjectivesPaged() {
     
      const config = {
        class:'modal-dialog-centered modal-md',
        initialState: {
          title: 'get Date Range Input for Statement of Account'
        }
      };
        
      this.bsModalRef = this.modalService.show(DateInputRangeModalComponent, config);
      const observableOuter = this.bsModalRef.content.returnDateRangeEvent;

      observableOuter.pipe(
        filter((dateRange: any) => dateRange !==null),
        switchMap((dateRange: any) => {
            var dt1 = convertDateToDateOnly(dateRange.fromDate);
            var dt2 = convertDateToDateOnly(dateRange.uptoDate);
            this.mParams.fromDate = dt1;
            this.mParams.uptoDate = dt2;
            return this.service.getPaginatedHRPerf(this.mParams);
        })
    ).subscribe((response: any) => {
      console.log('HR objectives paged response:', response);
      if(response !== null) {
        this.toastr.success('HR Objectives retrieved', 'Objectives Retrieved');
        this.objs = response.result;
        this.totalCount = response.count;
        this.pagination = response.pagination;
      } else {
        this.toastr.error('Error in retrieving Medical Objectives data', 'failed to retrieve')
      }
      
    });
  }

  onPageChanged(event: any){
    const params = this.service.getMedParams();
    if (this.mParams.pageNumber !== event.page) {
      this.mParams.pageNumber = event.page;
      this.service.setMedParams(this.mParams);
      this.getHRObjectivesPaged();
    }
  }

  WorkingDays(date1: Date, date2: Date): number {

    return getWorkingDays(date1, date2);
  }

}
