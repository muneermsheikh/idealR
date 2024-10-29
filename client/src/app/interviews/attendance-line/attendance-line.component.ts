import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IInterviewAttendanceDto } from 'src/app/_dtos/admin/interviewAttendanceDto';
import { ISelectionStatusDto } from 'src/app/_dtos/admin/selectionStatusDto';
import { IIntervwAttendance } from 'src/app/_models/hr/intervwAttendance';
import { AttendanceService } from 'src/app/_services/admin/attendance.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CvrefService } from 'src/app/_services/hr/cvref.service';

@Component({
  selector: 'app-attendance-line',
  templateUrl: './attendance-line.component.html',
  styleUrls: ['./attendance-line.component.css']
})
export class AttendanceLineComponent implements OnInit {

  @Input() att: IInterviewAttendanceDto | undefined;
  @Output() reportedEvent = new EventEmitter<number>();
  @Output() editEvent = new EventEmitter<IInterviewAttendanceDto>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() statusChangedEvent = new EventEmitter<IInterviewAttendanceDto>();
  @Output() selectedEvent = new EventEmitter<IInterviewAttendanceDto>();

  selectionStatuses: ISelectionStatusDto[]=[];
  candidateInterviewStatuses: IIntervwAttendance[]=[];
  lastSelectionStatus='';
  bsValueDate = new Date();

  showStepper:boolean=false;

  constructor(private cvrefservice: CvrefService, private confirm: ConfirmService, private attendanceService: AttendanceService){
    this.selectionStatuses = this.cvrefservice.selectionStatuses;
  }
  
  ngOnInit(): void {
   this.lastSelectionStatus = this.att?.interviewStatus!;
  }

  reportedClicked() {
    this.reportedEvent.emit(this.att?.id);
  }

  editClicked() {
    this.editEvent.emit(this.att);  //updates attendancesToUPdate array in parent component.  It is used by Update event in parent component
  }

  deleteClicked() {
    this.deleteEvent.emit(this.att?.id);
  }

  getCandidateInterviewStatuses() {
    this.attendanceService.getAttendanceStatus(this.att!.id).subscribe({
      next: (response: IIntervwAttendance[]) => {
        this.candidateInterviewStatuses = response;
        this.showStepper = !this.showStepper;
      }
    })
 
  }

  statusChangeClicked() {
    this.statusChangedEvent.emit(this.att);
  }

  selectedClicked() {
    this.selectedEvent.emit(this.att)
}

}