import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { MedicalParams } from 'src/app/_models/admin/objectives/medicalParams';
import { TaskService } from 'src/app/_services/admin/task.service';

@Component({
  selector: 'app-objective-report',
  templateUrl: './objective-report.component.html',
  styleUrls: ['./objective-report.component.css']
})
export class ObjectiveReportComponent {

  medParams = new MedicalParams();
  fromDate = new Date();
  uptoDate = new Date();

  bsValueDate = new Date();

  reportType = '';

  constructor(private toastr: ToastrService, private service: TaskService){}

  displayReport() {
    if(isNaN(new Date(this.medParams.fromDate).getTime()) || isNaN(new Date(this.uptoDate).getTime())) {
      this.toastr.warning('invalid Date');
      //return;
    }

    this.service.getMedicalObjectives(this.medParams.fromDate, this.medParams.uptoDate).subscribe({
      next: (response: any) => console.log(response)})
    
  }

}
