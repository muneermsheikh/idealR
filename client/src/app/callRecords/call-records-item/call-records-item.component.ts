import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IUserHistoryBriefDto } from 'src/app/_dtos/admin/useHistoryBriefDto';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { CallRecordsService } from 'src/app/_services/call-records.service';

@Component({
  selector: 'app-call-records-item',
  templateUrl: './call-records-item.component.html',
  styleUrls: ['./call-records-item.component.css']
})
export class CallRecordsItemComponent {
  @Output() editEvent = new EventEmitter<ICallRecord>();
  @Output() deleteEvent = new EventEmitter<number>();
  
  @Input() hist?: IUserHistoryBriefDto;
  
  constructor(private service: CallRecordsService) {
    
   }

  ngOnInit(): void {
  }

  editClicked() {
    this.service.getCallRecordWithItems(this.hist!.id, this.hist!.personType, 
        this.hist!.personId, this.hist!.categoryRef).subscribe({
          next: response => this.editEvent.emit(response)
        })
  }

  deleteClicked() {
    this.deleteEvent.emit(this.hist?.id);
  }

  callByPhone() {

  }

  sendEmail() {
    
  }

}
