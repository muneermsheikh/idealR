import { formatDate } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IInterviewItemWithErrDto } from 'src/app/_dtos/admin/interviewItemWithErrDto';
import { CvsMatchingProfAvailableDto } from 'src/app/_dtos/hr/cvsMatchingProfAvailableDto';
import { defaultInterviewAddress } from 'src/app/_dtos/hr/defaultInterviewAddress';
import { IntervwCandAttachment } from 'src/app/_models/hr/intervwCandAttachment';
import { IIntervwItem } from 'src/app/_models/hr/intervwItem';
import { IntervwItemCandidate } from 'src/app/_models/hr/intervwItemCandidate';
import { InterviewService } from 'src/app/_services/hr/interview.service';
import { CandidatesAvailableModalComponent } from 'src/app/modals/candidates-available-modal/candidates-available-modal.component';
import { DisplayTextModalComponent } from 'src/app/modals/display-text-modal/display-text-modal.component';

@Component({
  selector: 'app-edit-schedule',
  templateUrl: './edit-schedule.component.html',
  styleUrls: ['./edit-schedule.component.css']
})
export class EditScheduleComponent implements OnInit{

  @Input() interviewItem: IIntervwItem | undefined;
  @Input() interviewDateFrom: Date | undefined;
  @Input() interviewDateUpto: Date | undefined;

  personType = '';
  personId = '';
  lastTimeCalled: number= Date.now();
  userFiles: File[] = [];
  attachment: IntervwCandAttachment | undefined;
  selectedIndex: number=0;
  

  upload=false;
  itemCandidateSelected: IntervwItemCandidate|undefined;

  bsModalRef: BsModalRef | undefined;
  displayHistory: boolean = false;

  interviewResults=[{result: 'Not Interviewed'}, {result: 'Selected'}, 
    {result: 'Rejected-Exp irrelevant'}, {result: 'Rejected-Low Exp'}, {result: 'Rejected-Overage'}, 
      {result: 'Rejected-No communication'}, {result: 'Rejected-other reasons'}, {result: 'Shortlisted'}];

  constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute, 
      private router: Router,
      private service: InterviewService, private toastr: ToastrService, private bsModalService: BsModalService) {}
  
  bsValueDate = new Date();

  ngOnInit(): void {
    if(this.interviewDateFrom) {
      var dt = new Date(this.interviewDateFrom);
      this.interviewDateFrom = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate(),10,30);
    }
    this.InitializeForm(this.interviewItem!);
  }

  form: FormGroup = new FormGroup({});

  InitializeForm(item: IIntervwItem) {

      this.form = this.fb.group({

          id: item.id,
          intervwId: item.intervwId,
          orderItemId: [item.orderItemId, Validators.required],
          professionId: [item.professionId,Validators.required],
          professionName: [item.professionName, Validators.required],
          interviewMode: [item.interviewMode, Validators.required],
          interviewerName: item.interviewerName,
          interviewVenue: [item.interviewVenue,Validators.required],
          venueAddress: [item.venueAddress],
          venueAddress2: [item.venueAddress2],
          venueCityAndPIN: [item.venueCityAndPIN],
          siteRepName: [item.siteRepName],
          sitePhoneNo: [item.sitePhoneNo],

          interviewItemCandidates: this.fb.array(
          item.interviewItemCandidates.map(cand => (
            this.fb.group({
              id: cand.id,
              
              intervwItemId: [cand.interviewItemId ?? item.id, Validators.required],
              candidateId: [cand.candidateId,Validators.required],
              personId: [cand.personId],
              applicationNo: cand.applicationNo,
              candidateName: [cand.candidateName, Validators.required],
              passportNo: cand.passportNo,
              scheduledFrom: [cand.scheduledFrom, Validators.required],
              reportedAt: cand.reportedAt,
              interviewedAt: cand.interviewedAt,
              interviewStatus: [cand.interviewStatus,Validators.required],
              interviewerRemarks: cand.interviewerRemarks,
              attachmentFileNameWithPath: cand.attachmentFileNameWithPath 
            })
          ))
        )
      })
  }

  findCandidateId(index: number) {
    return this.interviewItemCandidates.at(index).get('candidateId')?.value;
  }

  findIntervwItemCandidateId(index: number) {
    return  this.interviewItemCandidates.at(index).get('id')?.value ?? 0;
  }


  findCandidateStatus(index: number): string {
    return this.interviewItemCandidates.at(index).get('interviewStatus')?.value ?? '';
  }

  get interviewItemCandidates(): FormArray {
    return this.form.get('interviewItemCandidates') as FormArray
  }

  newInterviewItemCandidate(): FormGroup{
    return this.fb.group({
        id: 0,
        intervwItemId: this.interviewItem?.id,
        candidateId: [0, Validators.required],
        applicationNo: 0,
        candidateName: ['', Validators.required],
        passportNo: '',
        scheduledFrom: ['', Validators.required],
        reportedAt: '',
        interviewedAt: '',
        interviewStatus: ['', Validators.required],
        interviewerRemarks: '',
        intervwCandAttachmentId: '',
        attachmentFileNameWithPath: ''
    })
  }

  addInterviewItemCandidate(index: number) {
    this.interviewItemCandidates.push(this.newInterviewItemCandidate())
  }

  removeInterviewItem(itemIndex: number) {
    this.interviewItemCandidates.removeAt(itemIndex);
    this.interviewItemCandidates.markAsDirty;
  }

  formChanged() {}

  onFileInputChange(event: Event) {
    const itemCandidate = this.interviewItemCandidates.at(this.selectedIndex)?.value;
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];

    if(f.size > 0) {
      this.userFiles.push(f);
      itemCandidate.attachmentFileNameWithPath = f.name;      
      this.interviewItemCandidates.at(this.selectedIndex)?.get('attachmentFileNameWithPath')?.setValue(f.name);
      this.form.markAsDirty();
    }
    
  }

  savewithattachments = () => {
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

    formData.append('data', JSON.stringify(this.form.value));

    this.service.editOrInsertInterviewItemWithFile(formData).subscribe({
      next: (response: IInterviewItemWithErrDto) => {
        console.log(response);
        if(response.error !== '') {
          this.toastr.error(response.error, 'failed to save the interview Item data')
        } else {
          this.toastr.success('interview Item saved', 'success');
          this.form.markAsPristine();
        }
      }, error: (err: any) => this.toastr.error(err.error.details, 'Error encountered')
    })
    this.userFiles=[];
    
  }

  update() {

    var fmValue = this.form.value;

    if(fmValue.id !== 0) {    //record exists - edit it
      this.service.updateInterviewItem(fmValue).subscribe({
        next: (response: IIntervwItem) => {
          if(response === null) {
            this.toastr.warning('failed to update the interview item', 'failure')
          } else {
            this.toastr.success('Interview category updated', 'success')
          }
        },
        error: (err: any) => this.toastr.error(err.error.details, "Error encountered")
      })
    } else {      //record does not exist, insert
      this.service.insertInterviewItem(fmValue).subscribe({
        next: (response: IIntervwItem) => {
          if(response===null) {
            this.toastr.warning('failed to insert the interview item', 'failure')
          } else {
            this.toastr.success('The interview category updated', 'success')
          }
        }, error: (err:any) => this.toastr.error(err.error.details, 'Error in inserting the interview category')
      })
    }
    
  }

  displayInterviewerRemarks(index: number) {

      var remarks = this.interviewItemCandidates.at(index).get('interviewerRemarks')!.value;
      const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          displayText: remarks,
          title: this.interviewItemCandidates.at(index).get('candidateName')!.value +  ' - edit Interviewer Remarks'
        }
      }
  
      this.bsModalRef = this.bsModalService.show(DisplayTextModalComponent, config);
  
      this.bsModalRef.content.textChangedEvent.subscribe({
        next: (response: string) => {
          if(response !== '') {
            this.interviewItemCandidates.at(index).get('interviewerRemarks')?.setValue(response)
          }
        }
      })
        
  }

  downloadattachment(index: number) {
    const fullpath = this.interviewItemCandidates.at(index).get('attachmentFileNameWithPath')?.value;
    if(fullpath === '' || fullpath === undefined || fullpath===null) {
       this.toastr.warning('no file exists for downloading', 'Bad Request');
       return;
    }
    this.service.downloadInterviewerCommentFile(fullpath).subscribe({
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
    this.itemCandidateSelected = this.interviewItemCandidates.at(index)!.value;
    this.selectedIndex=index;
    this.upload = !this.upload;
  }
  
  registerReportedAt(index: number) {
    this.interviewItemCandidates.at(index).get('reportedAt')?.setValue(formatDate(new Date(), 'yyyy-MM-ddThh:mm', 'en'));
    this.form.markAsDirty();
  }

  setInterviewedAt(index: number) {
    this.interviewItemCandidates.at(index).get('interviewedAt')?.setValue(formatDate(new Date(), 'yyyy-MM-ddThh:mm', 'en'));
    this.form.markAsDirty();
  }

  getMatchingCVs(item:IIntervwItem) {

    var profid=item.professionId;

    if(profid === 0) {
      this.toastr.warning('No Task object returned from Task line');
      return;
    }  

    if(this.interviewDateFrom === undefined || new Date(this.interviewDateFrom).getFullYear() < 2000) {
        this.toastr.warning('Interview Begin and End Dates not defined', 'Interview dates not defined');
        return;
    }

    var venue = this.form.get('interviewVenue')?.value;
    var interviewer = this.form.get('interviewerName')?.value;
    if(venue === '' || venue === 'To Be Annoounced'|| interviewer === '' || interviewer === 'To Be Announced') {
      this.toastr.warning('Interview Venue and Interviewer Name not provided');
      return;
    }
    
    var interviewBeginDateTime = this.interviewItemCandidates.length === 0 ? this.interviewDateFrom 
      : this.interviewItemCandidates.at(this.interviewItemCandidates.length - 1).get('scheduledFrom')?.value;

    //interviewBeginDateTime=addHours(interviewBeginDateTime, 10);
    //**todo** instead of last index, find max value of the scheduledFrom column
    var interviewDuration=30; //minutes

    const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          professionid: profid
        }
      }
  
      this.bsModalRef = this.bsModalService.show(CandidatesAvailableModalComponent, config);

      this.bsModalRef.content.emittedEvent.subscribe({      //calls interview.service.getMatchingCandidates based on profession selected
        next: (response: CvsMatchingProfAvailableDto[]) => {
          if(response !== null) {
              response.forEach(x => {
                
                this.interviewItemCandidates.push(
                this.fb.group({
                  id: 0,
                  intervwItemId: this.interviewItem?.id ?? 0,
                  candidateId: [x.candidateId ?? 0, Validators.required],
                  applicationNo: x.applicationNo,
                  candidateName: [x.fullName, Validators.required],
                  passportNo: '',
                  scheduledFrom: [interviewBeginDateTime, Validators.required],
                  reportedAt: null,
                  interviewedAt: null,
                  interviewStatus: ['Not Interviewed', Validators.required],
                  interviewerRemarks: '',
                  prospectiveCandidateId: [x.prospectiveCandidateId]
                }))
                interviewBeginDateTime = addMinutes(interviewBeginDateTime, interviewDuration);
              })
              this.toastr.success(response.length + ' candidates assigned.  Pl note the form needs to be saved for changes to take effect', 'Success');
          } else {
            this.toastr.warning('No candidates found - either registered or in prospective list - matching the profession ' + item.professionName, 'Not Found')
          }
        },
        error: (err: any) => this.toastr.error(err.error.details, 'error encountered')
      
      })
    }
  
    copyDefaultVenueAddress() {
      var address = new defaultInterviewAddress();
      this.form.get('interviewVenue')?.setValue(address.venue);
      this.form.get('venueAddress')?.setValue(address.address);
      this.form.get('venueAddress2')?.setValue(address.address2);
      this.form.get('venueCityAndPIN')?.setValue(address.city);
      this.form.get('siteRepName')?.setValue(address.siteRepName);
      this.form.get('sitePhoneNo')?.setValue(address.sitePhoneNo);
    }

    hideSelectionProgress() {
      //this.displayHistory = false;
    }

    getCandidateId(index: number): string {
      return this.interviewItemCandidates.at(index).get('candidateId')?.value.toString();
    }

    callStatusChanged(index: number, status: string) {
      this.interviewItemCandidates.at(index).setValue(status);
    }

}
function addMinutes(date: Date, minutes: number): Date {
  let result = new Date(date);
  result.setMinutes(result.getMinutes() + minutes);
  return result;
}

function addHours(date: Date, hours: number): Date {
  let result = new Date(date);
  result.setHours(result.getHours() + hours);
  return result;
}




