import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../_modules/shared.module';
import { MenuComponent } from './menu/menu.component';
import { AdminRoutingModule } from './admin-routing.module';
import { ReviewStatusNamePipe } from './review-status-name.pipe';
import { ChooseAgentsModalComponent } from './choose-agents-modal/choose-agents-modal.component';
import { CustomerLineComponent } from './customer-line/customer-line.component';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerEditModalComponent } from './customer-edit-modal/customer-edit-modal.component';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';
import { CustomerReviewModalComponent } from './customer-review-modal/customer-review-modal.component';
//import { AngularEditorModule } from '@kolkov/angular-editor';
import { ExcelConversionMenuComponent } from './excel-conversion-menu/excel-conversion-menu.component';
import { CustRvwEditModalComponent } from './cust-rvw-edit-modal/cust-rvw-edit-modal.component';
import { MatIconModule } from '@angular/material/icon';
import { NgxPrintModule } from 'ngx-print';


@NgModule({
  declarations: [
    MenuComponent,
    ReviewStatusNamePipe,
    ChooseAgentsModalComponent,
    CustomerLineComponent,
    CustomerListComponent,
    CustomerEditModalComponent,
    CustomerEditComponent,
    CustomerReviewModalComponent,
    CustRvwEditModalComponent,
    ExcelConversionMenuComponent
    
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    SharedModule,
    MatIconModule,
    NgxPrintModule
  ],
  schemas: [NO_ERRORS_SCHEMA]
})
export class AdministrationModule { }
