import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IProspectiveBriefDto } from 'src/app/_dtos/hr/prospectiveBriefDto';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { CallRecordsService } from 'src/app/_services/call-records.service';

@Component({
  selector: 'app-prospective-line',
  templateUrl: './prospective-line.component.html',
  styleUrls: ['./prospective-line.component.css']
})
export class ProspectiveLineComponent implements OnInit{

  @Input() prospective: IProspectiveBriefDto | undefined;
  @Input() isReport: boolean =false;
  
  @Output() editEvent = new EventEmitter<ICallRecord | null>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() selectedEvent = new EventEmitter<IProspectiveBriefDto>();
  @Output() convertToCandidateEvent = new EventEmitter<number>();
  @Output() ComposeMessageEvent = new EventEmitter<boolean>();
  //@Output() modeOfAdviseEvent = new EventEmitter<string>();


      StatusSelected="";

      constructor(private callRecService: CallRecordsService, private toastr: ToastrService){}

      ngOnInit(): void {
        
      }
      
      selected(value:any):void {
        this.StatusSelected = value;
      }

      editEventClicked() {

        if(this.prospective) {
            this.callRecService.getCallRecordWithItems("Prospective", this.prospective.personId)
              .subscribe({
                next: (response: ICallRecord) => {
                  console.log('prospective line:', response);
                  if(response) {
                    console.log('editebentclicked', response);
                    this.editEvent.emit(response)
                  }
                }, error: err => this.editEvent.emit(null)
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

      callStatusChanged(status: string) {
        this.prospective!.status = status;
      }
      
     
  }
