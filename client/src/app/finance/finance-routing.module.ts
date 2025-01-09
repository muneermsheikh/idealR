import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { VoucherListComponent } from './voucher-list/voucher-list.component';
import { COAListResolver } from '../_resolvers/finance/COAListResolver';
import { CoaListComponent } from './coa-list/coa-list.component';
import { StatementOfAccountResolver } from '../_resolvers/finance/statementOfAccountResolver';
import { SoaComponent } from './soa/soa.component';
import { ReceiptsPendingConfirmtionResolver } from '../_resolvers/receiptsPendingConfirmationResolver';
import { ConfirmFundReceiptComponent } from './confirm-fund-receipt/confirm-fund-receipt.component';
import { VoucherEditComponent } from './voucher-edit/voucher-edit.component';
import { FinanceVoucherResolver } from '../_resolvers/finance/financeVoucherResolver';

const routes: Routes = [
  
  {path: 'voucherlist', component: VoucherListComponent },

  {path: 'voucherEdit/:id', component: VoucherEditComponent,
      resolve: {
        voucher: FinanceVoucherResolver,
        coas: COAListResolver
      }
 },

  {path: 'coalist', component:CoaListComponent
      //, canActivate: [FinanceGuard]
      , data: {breadcrumb: {alias: 'Chart of Account'}}
  },
  
  {path: 'soa/:id/:fromDate/:uptoDate', 
    resolve: {soa: StatementOfAccountResolver},
    component: SoaComponent, data: {breakcrumb: {alias: 'statement of account'}}},
  
  {path: 'receiptspendingconfirmation', 
    resolve: {confirmationsPending: ReceiptsPendingConfirmtionResolver},
    component: ConfirmFundReceiptComponent}
    
  ]



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class FinanceRoutingModule { }
