import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ISelectionStatusDto } from 'src/app/_dtos/admin/selectionStatusDto';
import { IIntervwItemCandidate } from 'src/app/_models/hr/intervwItemCandidate';
import { AttendanceService } from 'src/app/_services/admin/attendance.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CvrefService } from 'src/app/_services/hr/cvref.service';

@Component({
  selector: 'app-edit-attendance-modal',
  templateUrl: './edit-attendance-modal.component.html',
  styleUrls: ['./edit-attendance-modal.component.css']
})
export class EditAttendanceModalComponent implements OnInit {

  @Output() updateAttendanceEvent = new EventEmitter<IIntervwItemCandidate|null>();
  @Output() deleteAttendanceData = new EventEmitter<IIntervwItemCandidate>();
  @Output() deleteInterviwItemCandidate = new EventEmitter<number>();

  itemCandidate: IIntervwItemCandidate | undefined;
  userFiles: File[]=[];
  upload = false;

  bsValueDate = new Date();
  lastTimeCalled: number= Date.now();

  selectionStatuses: ISelectionStatusDto[]=[];
  
  constructor(public bsModalRef: BsModalRef, private confirm: ConfirmService,
    private toastr: ToastrService, private service: AttendanceService,
    private cvrefservice: CvrefService) {
      this.selectionStatuses = this.cvrefservice.selectionStatuses;
    }
  
  ngOnInit(): void {
    console.log(this.itemCandidate);
  }

    updateAttendance() {
      
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
  
      formData.append('data', JSON.stringify(this.itemCandidate));
  
      this.service.UploadInterviewerNote(formData).subscribe({
        next: (response: string) => {
          console.log('response', response);
          if(response !== '') {
            this.toastr.error(response, 'failed to save the interview data');
            this.updateAttendanceEvent.emit(null);
          } else {
            this.toastr.success('interview saved', 'success')
            this.updateAttendanceEvent.emit(this.itemCandidate);
          }
        }, error: (err: any) => {
          this.toastr.error(err.error?.details, 'Error encountered');
          this.updateAttendanceEvent.emit(null);
        }
      })
      this.userFiles=[];
      this.bsModalRef.hide();
    }

    eraseAttendanceData() {

        this.confirm.confirm('Confirm Erase attendance', 
          'This will set to null all dates and interview Status.  It will take effect only after this form is saved.  Please Press OK to proceed, Cancel to Abort')
          .subscribe({
          next: (confirmed) => {
            if(confirmed) {
              this.itemCandidate?.reportedAt!=null;
              this.itemCandidate?.scheduledFrom!=null;
              this.itemCandidate?.interviewedAt!=null;
              this.itemCandidate?.interviewStatus!='';
              this.toastr.info('candidate detail schedules and status erased.  Changes will be lost if not saved');
            } else {
              this.toastr.info('Aborted', 'Aborted by user')
            }
          }
        })

    }

    deleteItemCandidate() {
      
    }

    toggleUpload() {
      this.upload = !this.upload;
    }
    
    onFileInputChange(event: Event) {
      const target = event.target as HTMLInputElement;
      const files = target.files as FileList;
      const f = files[0];
      if(f.size > 0) {
        this.userFiles.push(f);
        this.itemCandidate!.attachmentFileNameWithPath = f.name;      
      }
      
    }

    statusChanged() {
      if(this.itemCandidate) {
          this.itemCandidate.interviewedAt = new Date();
         if(new Date( this.itemCandidate.reportedAt!).getFullYear() < 2000) this.itemCandidate.reportedAt = new Date();
            this.itemCandidate.interviewedAt = this.itemCandidate.interviewedAt;
            this.itemCandidate.reportedAt = this.itemCandidate.reportedAt!;
        }
        if(new Date(this.itemCandidate!.scheduledFrom).getFullYear() < 2000) {
          this.itemCandidate!.scheduledFrom = this.itemCandidate!.interviewedAt!;
        }    
    }
  
}
