import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { CvsMatchingProfAvailableDto } from 'src/app/_dtos/hr/cvsMatchingProfAvailableDto';
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
  lastTimeCalled: number= Date.now();
  userFiles: File[] = [];
  attachment: IntervwCandAttachment | undefined;
  selectedIndex: number=0;

  upload=false;
  itemCandidateSelected: IntervwItemCandidate|undefined;

  bsModalRef: BsModalRef | undefined;

  interviewResults=[{result: 'Not Interviewed'}, {result: 'Selected'}, 
    {result: 'Rejected-Exp irrelevant'}, {result: 'Rejected-Low Exp'}, {result: 'Rejected-Overage'}, 
      {result: 'Rejected-No communication'}, {result: 'Rejected-other reasons'}, {result: 'Shortlisted'}];

  constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute, 
      private service: InterviewService, private toastr: ToastrService, private bsModalService: BsModalService) {}
  
  bsValueDate = new Date();

  ngOnInit(): void {
     
      this.InitializeForm(this.interviewItem!);
  }

  form: FormGroup = new FormGroup({});

  InitializeForm(item: IIntervwItem) {

      this.form = this.fb.group({

          id: item.id,
          interviewId: item.intervwId,
          orderItemId: [item.orderItemId, Validators.required],
          professionId: [item.professionId,Validators.required],
          professionName: [item.professionName, Validators.required],
          interviewMode: [item.interviewMode, Validators.required],
          interviewerName: item.interviewerName,
          interviewVenue: [item.interviewVenue,Validators.required],

          interviewItemCandidates: this.fb.array(
          item.interviewItemCandidates.map(cand => (
            this.fb.group({
              id: cand.id,
              intervwItemId: [cand.intervwItemId ?? item.id, Validators.required],
              candidateId: [cand.candidateId,Validators.required],
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
        intervwCandAttachmentId: ''
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
      next: (response: any) => {
        if(response.errorMessage !== '') {
          this.toastr.error('failed to save the interview data', response.errorMessage)
        } else {
          this.toastr.success('interview saved', 'success')
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
        class: 'modal-dialog-centered modal-md',
        initialState: {
          displayText: remarks,
          title: 'edit Interviewer Remarks'
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
    if(fullpath === '' || fullpath === undefined) this.toastr.warning('no file exists for downloading', 'Bad Request');
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

    /*if(this.userFiles.length > 0) {
      var f = this.userFiles[0];
      this.attachment = {
        candidateId:cand.candidateId, id:cand.id, 
        url: '', fileName: f.name, attachmentSizeInBytes: f.size/1024, dateUploaded:new Date()};
      
        cand.IntervwCandAttachment=this.attachment;
    }*/

  }
  
  getMatchingCVs(item:IIntervwItem) {

    var profid=item.professionId;

    if(profid === 0) {
      this.toastr.warning('No Task object returned from Task line');
      return;
    }  
  
    const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          professionid: profid
        }
      }
  
      this.bsModalRef = this.bsModalService.show(CandidatesAvailableModalComponent, config);
  
      this.bsModalRef.content.emittedEvent.subscribe({
        next: (response: CvsMatchingProfAvailableDto[]) => {
          if(response !== null) {
              response.forEach(x => {
                
                this.interviewItemCandidates.push(
                this.fb.group({
                  id: 0,
                  intervwItemId: this.interviewItem?.id,
                  candidateId: [x.candidateId, Validators.required],
                  applicationNo: x.applicationNo,
                  candidateName: [x.fullName, Validators.required],
                  passportNo: '',
                  scheduledFrom: [new Date, Validators.required],
                  reportedAt: '',
                  interviewedAt: '',
                  interviewStatus: ['Not Interviewed', Validators.required],
                  interviewerRemarks: ''
                }))
              })
              this.toastr.success(response.length + ' candidates assigned.  Pl note the form needs to be saved for changes to take effect', 'Success');
          } else {
            this.toastr.warning('No candidates found - either registered or in prospective list - matching the profession ' + item.professionName, 'Not Found')
          }
        },
        error: (err: any) => this.toastr.error(err.error.details, 'error encountered')
      
      })
    }
  

}

