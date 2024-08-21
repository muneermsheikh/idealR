import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IInterview } from 'src/app/_models/hr/interview';
import { IInterviewBrief } from 'src/app/_models/hr/interviewBrief';
import { IIntervw } from 'src/app/_models/hr/intervw';
import { InterviewService } from 'src/app/_services/hr/interview.service';

@Component({
  selector: 'app-interview-line',
  templateUrl: './interview-line.component.html',
  styleUrls: ['./interview-line.component.css']
})

export class InterviewLineComponent {

  @Input() interview: IInterviewBrief | undefined;
  @Output() deleteEvent = new EventEmitter<number>();

  @Output() editEvent = new EventEmitter<number>();
  
  constructor(private service: InterviewService, private toastr: ToastrService){}


  editClicked() {
    this.editEvent.emit(this.interview?.orderNo)
  }

  deleteClicked() {
      this.deleteEvent.emit(this.interview!.id);
  }

  

}
