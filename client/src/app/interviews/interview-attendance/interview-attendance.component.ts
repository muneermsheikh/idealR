import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AppId } from 'src/app/_dtos/admin/appId';
import { IInterviewAttendanceDto } from 'src/app/_dtos/admin/interviewAttendanceDto';
import { IInterviewAttendanceToUpdateDto, InterviewAttendanceToUpdateDto } from 'src/app/_dtos/hr/interviewAttendanceToUpdateDto';
import { Pagination } from 'src/app/_models/pagination';
import { attendanceParams } from 'src/app/_models/params/Admin/attendanceParams';
import { AttendanceService } from 'src/app/_services/admin/attendance.service';

@Component({
  selector: 'app-interview-attendance',
  templateUrl: './interview-attendance.component.html',
  styleUrls: ['./interview-attendance.component.css']
})
export class InterviewAttendanceComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  orderId = 0;
  attendances: IInterviewAttendanceDto[]=[];
  attendancesSelected: IInterviewAttendanceDto[]=[]; 
  pagination: Pagination | undefined;
  totalCount = 0;

  attendancesToUpdate: IInterviewAttendanceToUpdateDto[]=[];
  aParams = new attendanceParams();

  constructor(private service: AttendanceService, private toastr: ToastrService, private activatedRoute: ActivatedRoute){
    this.orderId = this.activatedRoute.snapshot.params['orderid'];
    if(this.orderId !== 0) this.aParams.orderId=this.orderId;
  }

  ngOnInit(): void {

    if(this.aParams.orderId !== 0) this.getAttendancePaged()    
  }

  getAttendancePaged() {

    this.service.setAParams(this.aParams);
    this.service.getAttendancePaged().subscribe({
      next: response => {
        this.attendances = response.result;
        this.pagination = response.pagination;
        this.totalCount = response.totalCount;

        console.log('attendances:', this.attendances);
      }
    })
   
  }

  updateAttendances() {
     this.service.updateAttendances(this.attendancesToUpdate).subscribe({
      next: (response: IInterviewAttendanceToUpdateDto[]) => {
        if(response !== null) {
          response.forEach(x => {
            var index = this.attendances.findIndex(m => m.id == x.interviewCandidateId);
            if(index !== -1) {
              this.attendances[index].interviewedAt=x.interviewedAt;
              this.attendances[index].reportedAt = x.reportedAt;
              this.attendances[index].interviewStatus = x.interviewStatus;
              this.attendances[index].interviewerRemarks = x.interviewRemarks;
            }
          })
          this.toastr.success('Interview Attendance updated', 'Success');
          this.attendancesToUpdate=[];
        } else {
          this.toastr.warning('Failed to update the attendance data', 'Failure')
        }
      }, error: (err: any) => this.toastr.error(err.error.details, 'Error encountered')
     })
  }

  attendanceEdit(att: IInterviewAttendanceDto) {
    var attUpdate =  new InterviewAttendanceToUpdateDto();

    attUpdate.interviewCandidateId=att.id;
    attUpdate.interviewRemarks=att.interviewerRemarks;
    attUpdate.interviewStatus = att.interviewStatus;
    attUpdate.interviewedAt = att.interviewedAt!;
    attUpdate.reportedAt = att.reportedAt!;

    this.attendancesToUpdate.push(attUpdate);
  }

  attendanceDelete(id: number) {
    this.service.deleteAttendance(id).subscribe({
      next: (response) => {if(response) {
          var index = this.attendances.findIndex(x => x.id === id);
          if(index !== -1) this.attendances.splice(index,1);
          this.toastr.success('Deleted', 'Attendance Deleted');
        } else {
          this.toastr.warning('Failed to delete', 'Failed to delete the candidate attendance')
        }},
        error: (err: any) => this.toastr.error(err.error.details, 'Error encountered in deleting the attendance record')
    })
  }

  onPageChanged(event: any){
    const params = this.service.getAParams() ?? new attendanceParams();
    
    if (params.pageNumber !== event.page) {
      params.pageNumber = event.page;
      this.service.setAParams(params);
      this.getAttendancePaged();
    }
  }

  onSearch() {
    const params = this.service.getAParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setAParams(params);
    this.getAttendancePaged();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    const params = this.service.getAParams();
    this.service.setAParams(params);
    this.getAttendancePaged();
  }

  statusChanged(att: IInterviewAttendanceDto) {

    this.attendanceEdit(att);
  }

  reported(id: number) {  //interviewItemCandidateId

    var index=this.attendances.findIndex(x => x.id === id);
    if(index !== -1) this.attendances[index].reportedAt = new Date();
  }

  
  selectionChanged(item: IInterviewAttendanceDto) {
    
    var checked=false;

    checked=item.checked!==true;

    var foundIndex = this.attendancesSelected.findIndex(x => x.id===item.id);

    checked ? this.attendancesSelected.push(item) : this.attendancesSelected.splice(foundIndex,1);
  }

  inviteCandidatesForInterview() {
    if(this.attendancesSelected.length === 0) return;

    var ids = this.attendancesSelected.map(x => x.id);
    console.log('ids', ids);
    
      this.service.composeInvitationMsgs(ids).subscribe({
        next: (response: AppId[]) => {
          if(response.length > 0) {
            this.toastr.success('Messages composed and available to view/edit/send from the Messages folder', 'Success')
            response.forEach(x => {
              var index = this.attendances.findIndex(x => x.applicationNo==x.applicationNo);
              if(index !== -1) this.attendances[index].applicationNo=x.applicationNo;
            })
          } else {
            this.toastr.warning('Warning', 'Failed to compose invitation messages')
          }
        }, error: (err: any) => this.toastr.error(err.error.details, 'Error encountered')
      })
  }

}
