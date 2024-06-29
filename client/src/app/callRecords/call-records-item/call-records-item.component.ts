import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IUserHistoryBriefDto } from 'src/app/_dtos/admin/useHistoryBriefDto';

@Component({
  selector: 'app-call-records-item',
  templateUrl: './call-records-item.component.html',
  styleUrls: ['./call-records-item.component.css']
})
export class CallRecordsItemComponent {
  @Output() editEvent = new EventEmitter<IUserHistoryBriefDto>();
  @Output() deleteEvent = new EventEmitter<number>();
  
  @Input() hist?: IUserHistoryBriefDto;
  
  constructor() { }

  ngOnInit(): void {
  }

  editClicked() {
    console.log('edit clicked');
    this.editEvent.emit(this.hist);
  }

  deleteClicked() {
    this.deleteEvent.emit(this.hist?.id);
  }

  callByPhone() {

  }

  sendEmail() {
    
  }

}
