import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IProspectiveBriefDto } from 'src/app/_dtos/hr/prospectiveBriefDto';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { IProspectiveCandidate } from 'src/app/_models/hr/prospectiveCandidate';
import { CallRecordsService } from 'src/app/_services/call-records.service';

@Component({
  selector: 'app-prospective-line',
  templateUrl: './prospective-line.component.html',
  styleUrls: ['./prospective-line.component.css']
})
export class ProspectiveLineComponent implements OnInit{

  @Input() prospective: IProspectiveBriefDto | undefined;
  
  @Output() editEvent = new EventEmitter<ICallRecord>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() selectedEvent = new EventEmitter<IProspectiveBriefDto>();
  @Output() convertToCandidateEvent = new EventEmitter<number>();

  StatusSelected="";

  constructor(private callRecService: CallRecordsService){}

  ngOnInit(): void {
    
  }
  
  selected(value:any):void {
    this.StatusSelected = value;
  }

  editEventClicked() {

    if(this.prospective) {
        this.callRecService.getCallRecordWithItems(0, this.prospective.personType, this.prospective.personId, this.prospective.categoryRef)
          .subscribe({
            next: (response: ICallRecord) => {
              if(response) {
                this.editEvent.emit(response)
              }
            }
          })
    }
  }

  selectedClicked() {
    this.selectedEvent.emit(this.prospective);
  }
  deleteEventClicked() {
    
    this.deleteEvent.emit(this.prospective?.id);
  }

  convertToCandidateClicked() {
    this.convertToCandidateEvent.emit(this.prospective?.id);
  }
}
