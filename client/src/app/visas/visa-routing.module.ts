import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VisalistComponent } from './visalist/visalist.component';
import { visaListPagedResolver } from '../_resolvers/admin/visa-list-paged.resolver';
import { RouterModule } from '@angular/router';
import { RegisterNewComponent } from './register-new/register-new.component';
import { CustomersBriefResolver } from '../_resolvers/admin/customersBriefResolver';
import { VisaTransactionsComponent } from './visa-transactions/visa-transactions.component';
import { visaTransactionsPagedResolver } from '../_resolvers/admin/visa-transactions-paged.resolver';
import { visaTransactionByCVRefIdResolver } from '../_resolvers/admin/visa-transaction-by-CVRefId.resolver';


const routes = [
  {path: '', component: VisalistComponent,
    resolve: {visas: visaListPagedResolver}
  },
  
  {path: 'visaEdit/:visaid', component: RegisterNewComponent
     , resolve: {
      customers: CustomersBriefResolver
      //, visa: visaResolver,
      
    }
  },

  {path: 'visaTransactions', component: VisaTransactionsComponent
    , resolve: {
      visas: visaTransactionsPagedResolver
    }
  },

  {path: 'visaTransactions/:visaId', component: VisaTransactionsComponent
    , resolve: {
      visas: visaTransactionsPagedResolver
    }
  },

  {path: 'visaTransactionByCVRefId/:cvRefId', component: VisaTransactionsComponent
    , resolve: {
      visas: visaTransactionByCVRefIdResolver
    }
  }
  
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
export class VisaRoutingModule { }
