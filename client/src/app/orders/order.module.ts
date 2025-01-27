import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../_modules/shared.module';
import { ContractRvwItemComponent } from './contract-rvw-item/contract-rvw-item.component';
import { DLComponent } from './dl/dl.component';
import { JdModalComponent } from './jd-modal/jd-modal.component';
import { OrderAssessmentComponent } from './order-assessment/order-assessment.component';
import { OrderAssessmentItemComponent } from './order-assessment-item/order-assessment-item.component';
import { OrderAssessmentItemModalComponent } from './order-assessment-item-modal/order-assessment-item-modal.component';
import { OrderForwardComponent } from './order-forward/order-forward.component';
import { DLForwardedComponent } from './DLForwarded/DLForwarded.component';
import { OrderItemComponent } from './order-item/order-item.component';
import { OrderItemReviewComponent } from './order-item-review/order-item-review.component';
import { DLForwardedLineComponent } from './DLForwarded-line/DLForwarded-line.component';
import { OrdersListingComponent } from './orders-listing/orders-listing.component';
import { RemunerationModalComponent } from './remuneration-modal/remuneration-modal.component';
import { OrderRoutingModule } from './order-routing.module';
import { MatIconModule } from '@angular/material/icon';
import { OrderAssessmentItemHeaderComponent } from './order-assessment-item-header/order-assessment-item-header.component';
import { NgxPrintModule } from 'ngx-print';


@NgModule({
  declarations: [
    ContractRvwItemComponent,
    DLComponent,
    JdModalComponent,
    OrderAssessmentComponent,
    OrderAssessmentItemComponent,
    OrderAssessmentItemModalComponent,
    OrderForwardComponent,
    DLForwardedComponent,
    OrderItemComponent,
    OrderItemReviewComponent,
    DLForwardedLineComponent,
    OrdersListingComponent,
    RemunerationModalComponent,
    OrderAssessmentItemHeaderComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    NgxPrintModule,
    OrderRoutingModule,
    SharedModule,
    MatIconModule
  ]
})
export class OrderModule { }
