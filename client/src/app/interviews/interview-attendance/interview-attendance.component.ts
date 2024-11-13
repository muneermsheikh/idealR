import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { AppId } from 'src/app/_dtos/admin/appId';
import { IInterviewAttendanceDto } from 'src/app/_dtos/admin/interviewAttendanceDto';
import { ISelectionStatusDto } from 'src/app/_dtos/admin/selectionStatusDto';
import { IInterviewAttendanceToUpdateDto, InterviewAttendanceToUpdateDto } from 'src/app/_dtos/hr/interviewAttendanceToUpdateDto';
import { IntervwCandAttachment } from 'src/app/_models/hr/intervwCandAttachment';
import { Pagination } from 'src/app/_models/pagination';
import { attendanceParams } from 'src/app/_models/params/Admin/attendanceParams';
import { AttendanceService } from 'src/app/_services/admin/attendance.service';
import { CvrefService } from 'src/app/_services/hr/cvref.service';
import { EditAttendanceModalComponent } from '../edit-attendance-modal/edit-attendance-modal.component';
import { IIntervwItemCandidate } from 'src/app/_models/hr/intervwItemCandidate';
import { filter } from 'rxjs';

@Component({
  selector: 'app-interview-attendance',
  templateUrl: './interview-attendance.component.html',
  styleUrls: ['./interview-attendance.component.css']
})
export class InterviewAttendanceComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  orderId = 0;
  attendances: IInterviewAttendanceDto[]=[];
  att: IInterviewAttendanceDto|undefined;   //to upload with attachment
  attendancesSelected: IInterviewAttendanceDto[]=[]; 
  pagination: Pagination | undefined;
  totalCount = 0;
  selectionStatuses: ISelectionStatusDto[]=[];
  
  lastTimeCalled: number= Date.now();
  userFiles: File[] = [];
  attachment: IntervwCandAttachment | undefined;
  selectedIndex: number=0;

  upload=false;

  attendancesToUpdate: IInterviewAttendanceToUpdateDto[]=[];
  aParams = new attendanceParams();
  bsModalRef: BsModalRef|undefined;

  constructor(private service: AttendanceService, private toastr: ToastrService, 
    private activatedRoute: ActivatedRoute, private cvrefservice: CvrefService,
    private bsModalService: BsModalService)
  {
    this.orderId = this.activatedRoute.snapshot.params['orderid'];
    if(this.orderId !== 0) this.aParams.orderId=this.orderId;
    this.selectionStatuses = this.cvrefservice.selectionStatuses;
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
    console.log('attendancesToUpdate', this.attendancesToUpdate, 'att', this.att);
     this.service.updateAttendances(this.attendancesToUpdate).subscribe({
      next: (response: IInterviewAttendanceToUpdateDto[]) => {
        if(response !== null) {
          response.forEach(x => {
            var index = this.attendances.findIndex(m => m.interviewItemCandidateId == x.interviewCandidateId);
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
      }, error: (err: any) => this.toastr.error(err.error?.details, 'Error encountered')
     })
  }

  attendanceEdit(att: IInterviewAttendanceDto) {
    var attUpdate =  new InterviewAttendanceToUpdateDto();

    attUpdate.interviewCandidateId=att.interviewItemCandidateId;
    attUpdate.interviewRemarks=att.interviewerRemarks;
    attUpdate.interviewStatus = att.interviewStatus;
    attUpdate.interviewedAt = att.interviewedAt!;
    console.log('attUpdate.interviewedAt', attUpdate.interviewedAt, 'att', att.interviewedAt);
    attUpdate.reportedAt = att.reportedAt!;

    this.attendancesToUpdate.push(attUpdate);
    console.log('attendanceToUpdate', this.attendancesToUpdate);
  }

  attendanceDelete(id: number) {
    this.service.deleteAttendance(id).subscribe({
      next: (response) => {if(response) {
          var index = this.attendances.findIndex(x => x.interviewItemCandidateId === id);
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
    if(this.searchTerm) this.searchTerm.nativeElement!.value = '';
    const params = this.service.getAParams();
    this.service.setAParams(params);
    this.getAttendancePaged();
  }

  statusChanged(att: IInterviewAttendanceDto) {
    console.log('statusChanged att:', att);

    if(att) {
      att.interviewedAt = new Date();
      //console.log(new Date(this.att.reportedAt!).getFullYear());
      if(new Date(att.reportedAt!).getFullYear() < 2000) att.reportedAt = new Date();

      var attUpdate =  new InterviewAttendanceToUpdateDto();

      var index = this.attendancesToUpdate.findIndex(x => x.interviewCandidateId ==  att.interviewItemCandidateId);
      if(index !== -1) {
        this.attendancesToUpdate[index].interviewStatus = att.interviewStatus;
        this.attendancesToUpdate[index].interviewedAt = att.interviewedAt!;
        this.attendancesToUpdate[index].reportedAt = att.reportedAt!;
        this.attendancesToUpdate[index].interviewRemarks = att.interviewerRemarks
        this.attendancesToUpdate[index].interviewStatus = att.interviewStatus;
      } else {
        var attUpdate = new InterviewAttendanceToUpdateDto();
        attUpdate.interviewCandidateId = att.interviewItemCandidateId;
        attUpdate.interviewStatus = att.interviewStatus;
        attUpdate.interviewedAt = att.interviewedAt!;
        attUpdate.reportedAt = att.reportedAt!;
        attUpdate.interviewRemarks = att.interviewerRemarks
        this.attendancesToUpdate.push(attUpdate);
      }
      console.log('atttoUpdate', this.attendancesToUpdate, 'att', att.interviewedAt);
    }
  }

  reported(id: number) {  //interviewItemCandidateId

    var index=this.attendances.findIndex(x => x.interviewItemCandidateId === id);
    if(index !== -1) this.attendances[index].reportedAt = new Date();
  }

  
  selectionChanged(item: IInterviewAttendanceDto) {
    
    var checked=false;

    checked=item.checked!==true;

    var foundIndex = this.attendancesSelected.findIndex(x => x.interviewItemCandidateId===item.interviewItemCandidateId);

    checked ? this.attendancesSelected.push(item) : this.attendancesSelected.splice(foundIndex,1);
  }

  inviteCandidatesForInterview() {
    if(this.attendancesSelected.length === 0) return;

    var ids = this.attendancesSelected.map(x => x.interviewItemCandidateId);
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

  uploadNote(att: IInterviewAttendanceDto) {

    var itemCandidate: IIntervwItemCandidate = {
        id: att.interviewItemCandidateId,  interviewItemId: 0,
        candidateId: att.interviewItemCandidateId, personId: att.personId,
        prospectiveCandidateId: 0, applicationNo: att.applicationNo,
        candidateName: att.candidateName, passportNo: '',
        scheduledFrom: att.scheduledFrom, 
        reportedAt: att.reportedAt, 
        interviewedAt: att.interviewedAt,
        interviewerRemarks: att.interviewerRemarks,
        interviewStatus: att.interviewStatus,
        attachmentFileNameWithPath: att.attachmentFileNameWithPath
    }
    new Date(Date.UTC(2024, 10, 2, 12, 30, 0));
    const config = {
        class: 'modal-dialog-centered modal-md',
        initialState: {
          itemCandidate: itemCandidate
        }
      }
  
      this.bsModalRef = this.bsModalService.show(EditAttendanceModalComponent, config);
  
      this.bsModalRef.content.updateAttendanceEvent.subscribe({
        next: (response: IIntervwItemCandidate) => {
          console.log('response in attendance', response);
          if(response === null) {
            this.toastr.info('Failed to update the candidate assignment, Or aborted by user', 'Failed to update the assignment')
          } else {
            this.toastr.success('Candidate Assignment updated',  'Success')            
          }
        }, error: (err: any) => this.toastr.error(err.error?.details, 'Error encountered')
      })
  } 

  savewWithattachments = () => {
    if(this.att===undefined || this.userFiles.length === 0) this.toastr.warning('the  object to upload is not defined');
    var microsecondsDiff: number= 28000;
    var nowDate: number =Date.now();
        
    if(nowDate < this.lastTimeCalled+ microsecondsDiff) return;
    
    this.lastTimeCalled=Date.now();
    let formData = new FormData();
    //const formValue = this.form.value;
    //console.log('formValue:', formValue);
    if(this.userFiles.length > 0) {
      this.userFiles.forEach(f => {
        formData.append('file', f, f.name);        
      })
    }

    formData.append('data', JSON.stringify(this.attendancesToUpdate));

    console.log('att in savewithattachments after stringify:', formData);

    this.service.UploadInterviewerNote(formData).subscribe({
      next: (response: string) => {
        if(response !== '') {
          this.toastr.error(response, 'failed to save the interview data')
        } else {
          this.toastr.success('interview saved', 'success')
        }
      }, error: (err: any) => this.toastr.error(err.error?.details, 'Error encountered')
    })
    this.userFiles=[];
    this.att=undefined;
  }

  
  downloadInterviewerNote(event: any) {

    const fullpath = event.attachmentFileNameWithPath;
    if(fullpath === '' || fullpath === undefined || fullpath===null) {
       this.toastr.warning('no file exists for downloading', 'Bad Request');
       return;
    }
    this.service.downloadInterviewerNote(fullpath).subscribe({
      next: (blob: Blob) => {
        const a = document.createElement('a');
        const objectUrl = URL.createObjectURL(blob);
        a.href = objectUrl;
        var i=fullpath.lastIndexOf('\\');
        if(i !== -1) {
          a.download = fullpath.substring(i+1);
        } else {
          a.download = 'filename.ext'
        }
        
        a.click();
        URL.revokeObjectURL(objectUrl);
      }
      , error: (err: any) => this.toastr.error(err.error.details, 'Error encountered while downloading the file ')
    })
  }

  uploadattachment(index: number) {
    //this.itemCandidateSelected = this.interviewItemCandidates.at(index)!.value;
    //this.selectedIndex=index;
    this.upload = !this.upload;
  }

  
  onFileInputChange(event: Event, att: IInterviewAttendanceDto) {
    this.selectedIndex = att.interviewItemCandidateId;
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];
    if(f.size > 0) {
      this.userFiles.push(f);
      this.att!.attachmentFileNameWithPath = f.name;      
      var index = this.attendancesSelected.findIndex(x => x.interviewItemCandidateId === att.interviewItemCandidateId);
      if(index !== -1) {
        this.attendancesSelected[index].attachmentFileNameWithPath = f.name;
      }
    }
    
  }

  FilesUploaded(event: File[]) {
    this.userFiles=event;
  }
}
