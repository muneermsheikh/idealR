import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../_modules/shared.module';
import { MenuComponent } from './menu/menu.component';
import { AdminRoutingModule } from './admin-routing.module';
import { SelectionsComponent } from './selections/selections.component';
import { OrdersListingComponent } from './orders/orders-listing/orders-listing.component';
import { OrderItemComponent } from './orders/order-item/order-item.component';
import { ReviewStatusNamePipe } from './review-status-name.pipe';
import { ChooseAgentsModalComponent } from './choose-agents-modal/choose-agents-modal.component';
import { JdModalComponent } from './orders/jd-modal/jd-modal.component';
import { RemunerationModalComponent } from './orders/remuneration-modal/remuneration-modal.component';
import { OrderAssessmentComponent } from './orders/order-assessment/order-assessment.component';
import { OrderAssessmentItemComponent } from './orders/order-assessment-item/order-assessment-item.component';
import { ProspectiveListComponent } from './Prospectives/prospective-list/prospective-list.component';
import { OrderItemReviewComponent } from './orders/order-item-review/order-item-review.component';
import { OrderForwardComponent } from './orders/order-forward/order-forward.component';
import { OrderfwdLineComponent } from './orders/orderfwd-line/orderfwd-line.component';
import { OrderFwdsComponent } from './orders/order-fwds/order-fwds.component';
import { SelectionPendingComponent } from './selections/selection-pending/selection-pending.component';
import { SelectionLineComponent } from './selections/selection-line/selection-line.component';
import { SelectionPendingLineComponent } from './selections/selection-pending-line/selection-pending-line.component';
import { EmploymentModalComponent } from './selections/employment-modal/employment-modal.component';
import { SelectionModalComponent } from './selections/selection-modal/selection-modal.component';
import { ProspectiveLineComponent } from './Prospectives/prospective-line/prospective-line.component';
import { CustomerLineComponent } from './customer-line/customer-line.component';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerEditModalComponent } from './customer-edit-modal/customer-edit-modal.component';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';
import { CustomerReviewComponent } from './customer-review/customer-review.component';
import { CustomerReviewModalComponent } from './customer-review-modal/customer-review-modal.component';
import { CustRvwEditModalComponent } from './cust-rvw-edit-modal/cust-rvw-edit-modal.component';
import { ContractRvwItemComponent } from './orders/CR/contract-rvw-item/contract-rvw-item.component';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { MessageComponent } from './message/message/messages.component';
import { DLComponent } from './dl/dl.component';
import { ExcelConversionMenuComponent } from './excel-conversion-menu/excel-conversion-menu.component';
import { OrderAssessmentItemModalComponent } from './orders/order-assessment-item-modal/order-assessment-item-modal.component';


@NgModule({
  declarations: [
    MenuComponent,
    SelectionsComponent,
    OrdersListingComponent,
    OrderItemComponent,
    ReviewStatusNamePipe,
    ChooseAgentsModalComponent,
    JdModalComponent,
    RemunerationModalComponent,
    OrderAssessmentComponent,
    OrderAssessmentItemComponent,
    ProspectiveListComponent,
    OrderItemReviewComponent,
    OrderForwardComponent,
    OrderfwdLineComponent,
    OrderFwdsComponent,
    SelectionPendingComponent,
    SelectionLineComponent,
    SelectionPendingLineComponent,
    EmploymentModalComponent,
    SelectionModalComponent,
    ProspectiveLineComponent,
    CustomerLineComponent,
    CustomerListComponent,
    CustomerEditModalComponent,
    CustomerEditComponent,
    CustomerReviewComponent,
    CustomerReviewModalComponent,
    CustRvwEditModalComponent,
    ContractRvwItemComponent,
    MessageComponent,
    DLComponent,
    ExcelConversionMenuComponent,
    OrderAssessmentItemModalComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    AngularEditorModule,
    SharedModule
  ],
  schemas: [NO_ERRORS_SCHEMA]
})
export class AdministrationModule { }
