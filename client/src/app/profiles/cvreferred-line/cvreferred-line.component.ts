import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ICVRefDto } from 'src/app/_dtos/admin/cvRefDto';
import { ISelPendingDto } from 'src/app/_dtos/admin/selPendingDto';

@Component({
  selector: 'app-cvreferred-line',
  templateUrl: './cvreferred-line.component.html',
  styleUrls: ['./cvreferred-line.component.css']
})
export class CvreferredLineComponent implements OnInit {
 
  @Input() cv: ISelPendingDto|undefined;
  @Input() isPrintPdf: boolean = false;

  @Output() msgEvent = new EventEmitter<number>();
  @Output() downloadEvent = new EventEmitter<number>();
  @Output() cvAssessEvent = new EventEmitter<ISelPendingDto>();
  @Output() cvCheckedEvent = new EventEmitter<ISelPendingDto>();
  @Output() deleteEvent = new EventEmitter<number>();

  currentId=0;
  
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

  deleteClicked(id: number) {   //cvref.id
    this.deleteEvent.emit(id)
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

  CVChecked() {
    this.cvCheckedEvent.emit(this.cv);
  }

}
