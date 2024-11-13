import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IInterviewAttendanceDto } from 'src/app/_dtos/admin/interviewAttendanceDto';
import { ISelectionStatusDto } from 'src/app/_dtos/admin/selectionStatusDto';
import { IIntervwAttendance } from 'src/app/_models/hr/intervwAttendance';
import { IIntervwItemCandidate } from 'src/app/_models/hr/intervwItemCandidate';
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
  @Output() downloadEvent = new EventEmitter<string>(); //filenamdwithPath to download
  @Output() uploadEvent = new EventEmitter<IInterviewAttendanceDto>();
  @Output() FilesUploadedEvent = new EventEmitter<File[]>();

  selectionStatuses: ISelectionStatusDto[]=[];
  candidateInterviewStatuses: IIntervwAttendance[]=[];
  lastSelectionStatus='';
  bsValueDate = new Date();
  
  upload=false;   //flag to show fileupload button
  userFiles: File[] = [];
  showStepper:boolean=false;

  constructor(private cvrefservice: CvrefService, private confirm: ConfirmService, private attendanceService: AttendanceService){
    this.selectionStatuses = this.cvrefservice.selectionStatuses;
  }
  
  ngOnInit(): void {
   this.lastSelectionStatus = this.att?.interviewStatus!;
  }

  reportedClicked() {
    this.reportedEvent.emit(this.att?.interviewItemCandidateId);
    this.editEvent.emit(this.att);
  }

  editClicked() {
    this.editEvent.emit(this.att);  //updates attendancesToUPdate array in parent component.  It is used by Update event in parent component
  }

  deleteClicked() {
    this.deleteEvent.emit(this.att?.interviewItemCandidateId);
  }

  getCandidateInterviewStatuses() {
    this.attendanceService.getAttendanceStatus(this.att!.interviewItemCandidateId).subscribe({
      next: (response: IIntervwAttendance[]) => {
        this.candidateInterviewStatuses = response;
        this.showStepper = !this.showStepper;
      }
    })
 
  }

  statusChangeClicked() {
    this.statusChangedEvent.emit(this.att);
    if(this.att) {
      this.att.interviewedAt = new Date();
      //console.log(new Date(this.att.reportedAt!).getFullYear());
      if(new Date(this.att.reportedAt!).getFullYear() < 2000) this.att.reportedAt = new Date();
    }
    
  }

  selectedClicked() {
    this.selectedEvent.emit(this.att)
  }

  downloadattachment() {
    this.downloadEvent.emit(this.att?.attachmentFileNameWithPath);
  }

  uploadattachment() {
    if(this.userFiles.length === 0) return;

    this.uploadEvent.emit(this.att);
    this.FilesUploadedEvent.emit(this.userFiles);
  }
  
  onFileInputChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];
    if(f.size > 0) {
      this.userFiles.push(f);
      this.att!.attachmentFileNameWithPath = f.name;      
    }
    
  }
}