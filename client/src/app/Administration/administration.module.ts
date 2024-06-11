import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CvrefComponent } from './cvref/cvref.component';
import { SharedModule } from '../_modules/shared.module';
import { MenuComponent } from './menu/menu.component';
import { OrderIndexComponent } from './orders/order-index/order-index.component';
import { AdminRoutingModule } from './admin-routing.module';
import { SelectionsComponent } from './selections/selections.component';
import { EmploymentsComponent } from './employments/employments.component';
import { CategoriesComponent } from './categories/categories.component';
import { IndustriesComponent } from './industries/industries.component';
import { QualificationsComponent } from './qualifications/qualifications.component';
import { CustomersComponent } from './customers/customers.component';
import { OrdersListingComponent } from './orders/orders-listing/orders-listing.component';
import { OrderItemComponent } from './orders/order-item/order-item.component';
import { ReviewStatusNamePipe } from './review-status-name.pipe';
import { OrderEditComponent } from './orders/order-edit/order-edit.component';
import { ChooseAgentsModalComponent } from './choose-agents-modal/choose-agents-modal.component';
import { JdModalComponent } from './orders/jd-modal/jd-modal.component';
import { RemunerationModalComponent } from './orders/remuneration-modal/remuneration-modal.component';
import { OrderAssessmentComponent } from './orders/order-assessment/order-assessment.component';
import { OrderAssessmentItemComponent } from './orders/order-assessment-item/order-assessment-item.component';
import { ProspectiveListComponent } from './Prospectives/prospective-list/prospective-list.component';
import { OrderItemReviewModalComponent } from './orders/order-item-review-modal/order-item-review-modal.component';
import { OrderForwardComponent } from './orders/order-forward/order-forward.component';
import { OrderfwdLineComponent } from './orders/orderfwd-line/orderfwd-line.component';
import { OrderFwdsComponent } from './orders/order-fwds/order-fwds.component';
import { SelectionPendingComponent } from './selections/selection-pending/selection-pending.component';
import { SelectionLineComponent } from './selections/selection-line/selection-line.component';
import { SelectionPendingLineComponent } from './selections/selection-pending-line/selection-pending-line.component';
import { EmploymentModalComponent } from './selections/employment-modal/employment-modal.component';
import { SelectionModalComponent } from './selections/selection-modal/selection-modal.component';


@NgModule({
  declarations: [
    CvrefComponent,
    MenuComponent,
    OrderIndexComponent,
    SelectionsComponent,
    EmploymentsComponent,
    CategoriesComponent,
    IndustriesComponent,
    QualificationsComponent,
    CustomersComponent,
    OrdersListingComponent,
    OrderItemComponent,
    ReviewStatusNamePipe,
    OrderEditComponent,
    ChooseAgentsModalComponent,
    JdModalComponent,
    RemunerationModalComponent,
    //OrderAssessmentItemQComponent,
    OrderAssessmentComponent,
    OrderAssessmentItemComponent,
    ProspectiveListComponent,
    OrderItemReviewModalComponent,
    OrderForwardComponent,
    OrderfwdLineComponent,
    OrderFwdsComponent,
    SelectionPendingComponent,
    SelectionLineComponent,
    SelectionPendingLineComponent,
    EmploymentModalComponent,
    SelectionModalComponent,
       
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    SharedModule
  ]
})
export class AdministrationModule { }
