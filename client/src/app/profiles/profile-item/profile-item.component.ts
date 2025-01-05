import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrComponentlessModule, ToastrService } from 'ngx-toastr';
import { ICandidateBriefDto } from 'src/app/_dtos/admin/candidateBriefDto';
import { ICandidateAssessment } from 'src/app/_models/hr/candidateAssessment';
import { ICvAssessmentHeader } from 'src/app/_models/hr/cvAssessmentHeader';
import { InterviewBriefPendingModalComponent } from 'src/app/interviews/interview-brief-pending-modal/interview-brief-pending-modal.component';

@Component({
  selector: 'app-profile-item',
  templateUrl: './profile-item.component.html',
  styleUrls: ['./profile-item.component.css']
})
export class ProfileItemComponent implements OnInit {

  @Input() cv: ICandidateBriefDto|undefined;
  @Output() msgEvent = new EventEmitter<number>();
  @Output() downloadEvent = new EventEmitter<number>();
  @Output() cvAssessEvent = new EventEmitter<ICandidateBriefDto>();
  @Output() cvCheckedEvent = new EventEmitter<ICandidateBriefDto>();
  @Output() cvEditEvent = new EventEmitter<number>();
  @Output() cvDeleteEvent = new EventEmitter<number>();


  currentId=0;
  header: ICvAssessmentHeader|undefined;
  assessment: ICandidateAssessment|undefined;
  
  cvidForDocumentView: number=0;

  public isHidden: boolean = true;
  xPosTabMenu: number=0;
  yPosTabMenu: number=0;
  
  bsModalRef: BsModalRef|undefined;

  constructor(private modalService: BsModalService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }
  //right click
  rightClick(event: any) {
    event.stopPropagation();
    this.xPosTabMenu = event.clientX;
    this.yPosTabMenu = event.clientY;
    this.isHidden = false;
    return false;
  }

  closeRightClickMenu() {
    this.isHidden = true;
  }

  download(id: number) {
    this.downloadEvent.emit(id);
  }

  //async 
  onClickLoadDocument(cvid: number) {
    // get a document from the Web API endpoint 'LoadDocument'
    this.msgEvent.emit(cvid);
  }

  setCurrentId(id: number) {
    this.currentId = id;
  }

  showhistory(cvid: number) {
    this.msgEvent.emit(cvid);
  }

  cvAssessClicked(t: ICandidateBriefDto)
  {
    this.cvAssessEvent.emit(t);
  }

  CVChecked(cv: ICandidateBriefDto) {
    this.cvCheckedEvent.emit(cv);
  }

  editCV(id: number) {
      this.cvEditEvent.emit(id);
  }

  deleteCV(id: number) {
    this.cvDeleteEvent.emit(id);
  }

  assignToInterview(cvid: number) {
   
      const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          candidateId: this.cv?.id,
          candidateName: this.cv?.fullName,
          applicationNo: this.cv?.applicationNo,
          interviewDate: new Date(),
          categoryMatching: this.cv?.userProfessions.flat()
        }
      }
      
      console.log('config', this.cv?.userProfessions, this.cv?.userProfessions.flat());

      this.bsModalRef = this.modalService.show(InterviewBriefPendingModalComponent, config);
      this.bsModalRef.content.EmitEvent.subscribe({
        next: (response: boolean) => {
          if(response) this.toastr.success('selected candidate added to the interview item  selected', 'Success')
        }
      })

  }


}
