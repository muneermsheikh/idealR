import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IMedicalObjective } from 'src/app/_models/admin/objectives/medicalObjective';
import { MedicalParams } from 'src/app/_models/admin/objectives/medicalParams';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { TaskService } from 'src/app/_services/admin/task.service';

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

  fromDate = new Date();
  uptoDate = new Date();

  bsValueDate = new Date();


  constructor(private service: TaskService, private toastr: ToastrService){}

  ngOnInit(): void {
    
    this.getMedObjectivesPaged();

  }

  getMedObjectivesPaged() {

    //this.mParams.fromDate = this.fromDate.getTime().toString();
    //this.mParams.uptoDate = this.uptoDate.getTime().toString();

    this.service.setMedParams(this.mParams);

    this.service.getPaginatedMedicalPerf(this.mParams.fromDate, this.mParams.uptoDate).subscribe({
      next: (response: any) => {
        if(response.result && response.pagination) {
          this.objs = response.result;
          this.pagination = response.pagination;
          this.totalCount = response.count;
        }
      },
      error: (err: any) => this.toastr.error(err.error.details, 'Error')
    });
  }

  displayReport() {
    if(isNaN(this.fromDate.getTime()) || isNaN(this.uptoDate.getTime())) {
      this.toastr.warning('invalid Date');
      return;
    }

    this.getMedObjectivesPaged();

  }   
  
  onPageChanged(event: any){
    const params = this.service.getParams();
    if (this.mParams.pageNumber !== event.page) {
      this.mParams.pageNumber = event.page;
      this.service.setMedParams(this.mParams);
      this.getMedObjectivesPaged();
    }
  }

}
