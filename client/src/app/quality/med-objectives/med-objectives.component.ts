import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { IMedicalObjective } from 'src/app/_models/admin/objectives/medicalObjective';
import { MedicalParams } from 'src/app/_models/admin/objectives/medicalParams';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { getWorkingDays } from 'src/app/_services/paginationHelper';
import { QualityService } from 'src/app/_services/quality.service';
import { convertDateToDateOnly } from 'src/app/finance/coa-list/coa-list.component';
import { DateInputRangeModalComponent } from 'src/app/modals/date-input-range-modal/date-input-range-modal.component';

@Component({
  selector: 'app-med-objectives',
  templateUrl: './med-objectives.component.html',
  styleUrls: ['./med-objectives.component.css']
})
export class MedObjectivesComponent implements OnInit {

  objs: IMedicalObjective[]=[];
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

  constructor(private service: QualityService, private toastr: ToastrService,
    private modalService: BsModalService){}

  ngOnInit(): void {
    
    this.getMedicalObjectivesPaged();

  }

  getMedicalObjectivesPaged() {
     
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
            return this.service.getPaginatedMedExecPerf(this.mParams);
        })
    ).subscribe((response: any) => {
      console.log('med objectives getmedicalobjectives paged response:', response);
      if(response !== null) {
        this.toastr.success('Medical Objectives retrieved', 'Objectives Retrieved');
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
      this.getMedicalObjectivesPaged();
    }
  }

  WorkingDays(date1: Date, date2: Date): number {

    return getWorkingDays(date1, date2);
  }

}
