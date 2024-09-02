import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IEmployeeBriefDto } from 'src/app/_dtos/admin/employeeBriefDto';
import { IEmployee } from 'src/app/_models/admin/employee';
import { EmployeeService } from 'src/app/_services/admin/employee.service';

@Component({
  selector: 'app-employee-line',
  templateUrl: './employee-line.component.html',
  styleUrls: ['./employee-line.component.css']
})
export class EmployeeLineComponent {

  @Input() emp: IEmployeeBriefDto | undefined;
  @Output() deleteEmployeeEvent = new EventEmitter<number>();
  @Output() editEmployeeEvent =new EventEmitter<number>();
  @Output() uploadClickedEvent = new EventEmitter<number>();

  constructor(private service: EmployeeService, private toastr: ToastrService){}

  editClicked() {
    this.editEmployeeEvent.emit(this.emp!.id)
  }

  deleteClicked() {
    this.deleteEmployeeEvent.emit(this.emp!.id);
  }

  uploadClicked() {
      this.uploadClickedEvent.emit(this.emp!.id);
  }
  

}
