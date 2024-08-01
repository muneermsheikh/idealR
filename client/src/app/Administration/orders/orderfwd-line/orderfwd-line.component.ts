import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IOrderForwardCategory } from 'src/app/_models/orders/orderForwardCategory';
import { IOrderForwardToAgent } from 'src/app/_models/orders/orderForwardToAgent';

@Component({
  selector: 'app-orderfwd-line',
  templateUrl: './orderfwd-line.component.html',
  styleUrls: ['./orderfwd-line.component.css']
})
export class OrderfwdLineComponent {

  @Input() fwd: IOrderForwardCategory | undefined;
  
  @Output() editEvent = new EventEmitter<number>();
  @Output() displayCategoryEvent = new EventEmitter<number>();
  @Output() deleteEventMain = new EventEmitter<number>();
  @Output() deleteEventCat = new EventEmitter<number>();
  @Output() deleteEventOff = new EventEmitter<number>();

  hideCategory: boolean=false;

  editClicked() {
    this.editEvent.emit(this.fwd!.id);
  }

  displayFwdCategory() {
    this.displayCategoryEvent.emit(this.fwd!.id)
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
