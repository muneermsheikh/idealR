import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ICallRecordDto } from 'src/app/_dtos/admin/callRecordDto';
import { CallRecordsService } from 'src/app/_services/call-records.service';

@Component({
  selector: 'app-selection-history',
  templateUrl: './selection-history.component.html',
  styleUrls: ['./selection-history.component.css']
})
export class SelectionHistoryComponent implements OnInit{

  callRecordDtos: ICallRecordDto[]=[];

  @Input() personType: string =  '';
  @Input() personId: string = '';
  @Output() closeEvent = new EventEmitter<boolean>();

  constructor(private service: CallRecordsService){}

  ngOnInit(): void {
    
    this.service.getCallRecordsOfACandidate(this.personType, this.personId).subscribe({
      next: (response: ICallRecordDto[]) => this.callRecordDtos = response
    })
  }

  close() {
    this.closeEvent.emit(true);
  }

}
