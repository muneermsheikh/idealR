import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuComponent } from './menu/menu.component';
import { RouterModule } from '@angular/router';
import { UserManagementComponent } from '../admin/user-management/user-management.component';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerResolver } from '../_resolvers/admin/customerResolver';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';
import { CustRvwEditModalComponent } from './cust-rvw-edit-modal/cust-rvw-edit-modal.component';
import { CustomerReviewResolver } from '../_resolvers/customerReviewResolver';
import { FeedbackListComponent } from '../feedback/feedback-list/feedback-list.component';
import { CvsreferredComponent } from '../profiles/cvsreferred/cvsreferred.component';
import { ExcelConversionMenuComponent } from './excel-conversion-menu/excel-conversion-menu.component';


const routes = [
  {path: '', component: MenuComponent},
  
   {path: 'excelConversion', component: ExcelConversionMenuComponent},

  {path: 'excelConversionOfNaukri', component: ExcelConversionMenuComponent},
  
  {path: 'cvsreferred/:id', component: CvsreferredComponent},
  
  {path: 'customers', component: CustomerListComponent},

  {path: 'customerEdit/:id', component: CustomerEditComponent,
    resolve: {
      customer: CustomerResolver
    }
  },
    
  {path: 'reviewEdit/:id', component: CustRvwEditModalComponent,
    resolve: {
      review: CustomerReviewResolver
    }
  },

  {path: 'feedbacklist', component: FeedbackListComponent},
  
  {path: 'userroles', component: UserManagementComponent}
 
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
export class AdminRoutingModule { }
