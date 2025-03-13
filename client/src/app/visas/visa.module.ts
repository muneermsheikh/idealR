import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VisalistComponent } from './visalist/visalist.component';
import { VisaEditComponent } from './visa-edit/visa-edit.component';
import { SharedModule } from '../_modules/shared.module';
import { NgxPrintModule } from 'ngx-print';
import { VisaRoutingModule } from './visa-routing.module';
import { MatIconModule } from '@angular/material/icon';
import { VisaLineComponent } from './visa-line/visa-line.component';
import { RegisterNewComponent } from './register-new/register-new.component';
import { VisaAssignComponent } from './visa-assign/visa-assign.component';
import { VisaAssignModalComponent } from './visa-assign-modal/visa-assign-modal.component';
import { VisaTransactionsComponent } from './visa-transactions/visa-transactions.component';
import { VisaTransactionLineComponent } from './visa-transaction-line/visa-transaction-line.component';



@NgModule({
  declarations: [
    VisalistComponent,
    VisaEditComponent,
    VisaLineComponent,
    RegisterNewComponent,
    VisaAssignComponent,
    VisaAssignModalComponent,
    VisaTransactionsComponent,
    VisaTransactionLineComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    NgxPrintModule,
    VisaRoutingModule,
    MatIconModule
  ]
})
export class VisaModule { }
