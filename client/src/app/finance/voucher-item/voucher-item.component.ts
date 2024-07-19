import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IVoucherDto } from 'src/app/_dtos/finance/voucherDto';

@Component({
  selector: 'app-voucher-item',
  templateUrl: './voucher-item.component.html',
  styleUrls: ['./voucher-item.component.css']
})
export class VoucherItemComponent {

  @Input() t?: IVoucherDto;
  
  @Output() editEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();

  constructor() { }
  
  editTransaction(id: number) {
    this.editEvent.emit(id);
  }
  
  deleteTransaction(id: number) {
    this.deleteEvent.emit(id);
  }

}
