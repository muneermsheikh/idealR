import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoaItemComponent } from './coa-item/coa-item.component';
import { CoaListComponent } from './coa-list/coa-list.component';
import { CoaEditModalComponent } from './coa-edit-modal/coa-edit-modal.component';
import { ConfirmFundReceiptComponent } from './confirm-fund-receipt/confirm-fund-receipt.component';
import { SoaLineComponent } from './soa-line/soa-line.component';
import { SoaComponent } from './soa/soa.component';
import { VoucherItemComponent } from './voucher-item/voucher-item.component';
import { VoucherListComponent } from './voucher-list/voucher-list.component';
import { FinanceRoutingModule } from './finance-routing.module';
import { SharedModule } from '../_modules/shared.module';
import { AddPaymentModalComponent } from './add-payment-modal/add-payment-modal.component';
import { VoucherEditComponent } from './voucher-edit/voucher-edit.component';



@NgModule({
  declarations: [
    CoaItemComponent,
    CoaListComponent,
    CoaEditModalComponent,
    ConfirmFundReceiptComponent,
    SoaLineComponent,
    SoaComponent,
    VoucherItemComponent,
    VoucherListComponent,
    AddPaymentModalComponent,
    VoucherEditComponent,
  ],
  imports: [
    CommonModule, 
    FinanceRoutingModule,
    SharedModule
  ],
  schemas: [NO_ERRORS_SCHEMA]
})
export class FinanceModule { }
