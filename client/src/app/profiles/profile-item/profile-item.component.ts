import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ICandidateBriefDto } from 'src/app/_dtos/admin/candidateBriefDto';
import { ICandidateAssessment } from 'src/app/_models/hr/candidateAssessment';
import { ICvAssessmentHeader } from 'src/app/_models/hr/cvAssessmentHeader';

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


  currentId=0;
  header: ICvAssessmentHeader|undefined;
  assessment: ICandidateAssessment|undefined;
  
  cvidForDocumentView: number=0;

  public isHidden: boolean = true;
  xPosTabMenu: number=0;
  yPosTabMenu: number=0;
  
  constructor() { }

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

  async onClickLoadDocument(cvid: number) {
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
      console.log('profile item edit cv, id: ', id);
  }
}
