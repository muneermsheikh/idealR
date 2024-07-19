import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IStatementofAccountItemDto } from 'src/app/_dtos/finance/statementOfAccountDto';

@Component({
  selector: 'app-soa-line',
  templateUrl: './soa-line.component.html',
  styleUrls: ['./soa-line.component.css']
})
export class SoaLineComponent {

  @Input() soaItem: IStatementofAccountItemDto | undefined;
  @Output() displayVoucherEvent = new EventEmitter<number>();
  
  displayVoucher(){
    this.displayVoucherEvent.emit(this.soaItem!.voucherNo);
  }
  
}
