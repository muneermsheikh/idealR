import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrComponentlessModule, ToastrService } from 'ngx-toastr';
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
  
  constructor(private service: CallRecordsService, private toastr: ToastrService) {}

  ngOnInit(): void {
  }

  editClicked() {
    this.service.getCallRecordWithItems(this.hist!.personType, this.hist!.personId).subscribe({
          next: response => {
            this.editEvent.emit(response)
          }, error: err => this.toastr.warning(err.error.text, 'Failed to retrieve the call record')
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
