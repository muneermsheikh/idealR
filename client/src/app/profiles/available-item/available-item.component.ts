import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ICvsAvailableDto } from 'src/app/_dtos/admin/cvsAvailableDto';

@Component({
  selector: 'app-available-item',
  templateUrl: './available-item.component.html',
  styleUrls: ['./available-item.component.css']
})
export class AvailableItemComponent implements OnInit {

 
  @Input() cv: ICvsAvailableDto|undefined;
  @Input() isPrintPDF: boolean=false;
  @Output() msgEvent = new EventEmitter<number>();
  @Output() downloadEvent = new EventEmitter<ICvsAvailableDto>();
  @Output() cvAssessEvent = new EventEmitter<ICvsAvailableDto>();
  @Output() cvCheckedEvent = new EventEmitter<ICvsAvailableDto>();
 

  currentId=0;
  //header: ICvAssessmentHeader|undefined;
  //assessment: ICandidateAssessment|undefined;
  
  cvidForDocumentView: number=0;

  public isHidden: boolean = true;
  xPosTabMenu: number=0;
  yPosTabMenu: number=0;
  
  constructor() { }

  ngOnInit(): void {
    console.log(this.isPrintPDF)
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

  download(cv: ICvsAvailableDto) {
    this.downloadEvent.emit(cv);
  }

  //async 
  onClickLoadDocument(cvid: number) {
    // get a document from the Web API endpoint 'LoadDocument'
    this.msgEvent.emit(cvid);
  }

  displayAssessment() {
      this.cvAssessEvent.emit(this.cv);
  }

  setCurrentId(id: number) {
    this.currentId = id;
  }

  showhistory(cvid: number) {
    this.msgEvent.emit(cvid);
  }

  
  CVChecked(cv: ICvsAvailableDto) {
    this.cvCheckedEvent.emit(cv);
  }


}
