import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IFeedbackDto } from 'src/app/_dtos/hr/feedbackDto';
import { IFeedback } from 'src/app/_models/hr/feedback';
import { FeedbackService } from 'src/app/_services/feedback.service';

@Component({
  selector: 'app-feedback-line',
  templateUrl: './feedback-line.component.html',
  styleUrls: ['./feedback-line.component.css']
})
export class FeedbackLineComponent {

  @Input() fdbk: IFeedbackDto|undefined;
  @Output() editEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() emailEvent = new EventEmitter<number>();

  constructor(private service: FeedbackService){};

  editClicked(fdbkId: number) { 
    this.service.getFeedbackWithItems(fdbkId).subscribe({
      next: response => {
          this.editEvent.emit(fdbkId);
      }
    })
  }

  deleteClicked(fdbkId: number) {
    this.deleteEvent.emit(fdbkId);
  }

  generateEmail(fdbkId: number) {
    this.emailEvent.emit(fdbkId)
  }

}
