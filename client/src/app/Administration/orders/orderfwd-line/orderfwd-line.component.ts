import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IOrderForwardToAgentDto } from 'src/app/_dtos/orders/orderForwardToAgentDto';
import { IOrderForwardToAgent } from 'src/app/_models/orders/orderForwardToAgent';

@Component({
  selector: 'app-orderfwd-line',
  templateUrl: './orderfwd-line.component.html',
  styleUrls: ['./orderfwd-line.component.css']
})
export class OrderfwdLineComponent {

  @Input() fwd: IOrderForwardToAgentDto | undefined;
  
  @Output() editEvent = new EventEmitter<number>();
  @Output() deleteEventMain = new EventEmitter<number>();
  @Output() deleteEventCat = new EventEmitter<number>();
  @Output() deleteEventOff = new EventEmitter<number>();

  editClicked() {
    this.editEvent.emit(this.fwd!.id);
  }

  deleteOrder(id: number) {
    this.deleteEventMain.emit(id);
  }

  deleteCat(id: number) {
    this.deleteEventCat.emit(id);
  }

  deleteOff(id: number) {
    this.deleteEventOff.emit(id);
  }
}
