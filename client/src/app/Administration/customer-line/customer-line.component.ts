import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ICustomerBriefDto } from 'src/app/_dtos/admin/customerBriefDto';
import { CustomersService } from 'src/app/_services/admin/customers.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-customer-line',
  templateUrl: './customer-line.component.html',
  styleUrls: ['./customer-line.component.css']
})
export class CustomerLineComponent {

  @Input() dto: ICustomerBriefDto | undefined;
  @Input() isPrintPDF: boolean = false;

  @Output() editEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() selectedEvent = new EventEmitter<number>();
  @Output() EvaluationEvent = new EventEmitter<number>();
  @Output() feedbackEvent = new EventEmitter<number>();
  @Output() displayHistoryEvent = new EventEmitter<number>();

  constructor(private service: CustomersService, private toastr: ToastrService,
      private confirm: ConfirmService){}

  editClicked() {
      this.editEvent.emit(this.dto!.id);
  }

  deleteClicked() {
    this.deleteEvent.emit(this.dto!.id);
  }

  selectedClicked() {
    this.selectedEvent.emit(this.dto!.id);
  }

  displayEvaluationClicked() {

    this.EvaluationEvent.emit(this.dto!.id)   
  }
  

  feedbackFormClicked() {
    this.feedbackEvent.emit(this.dto!.id);
  }

  displayHistory() {
    this.displayHistoryEvent.emit(this.dto!.id);
  }
  
}
