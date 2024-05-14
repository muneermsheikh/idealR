import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderListComponent } from './order-list/order-list.component';
import { OrderLineComponent } from './order-line/order-line.component';
import { OrderEditComponent } from './order-edit/order-edit.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../_modules/shared.module';
import { OrderRoutingModule } from './order-routing.module';
import { ReviewItemStatusNamePipe } from './review-item-status-name.pipe';
import { ReviewStatusNamePipe } from './review-status-name.pipe';
import { ContractReviewItemModalComponent } from './contract-review-item-modal/contract-review-item-modal.component';
import { JdModalComponent } from './jd-modal/jd-modal.component';
import { OrderForwardComponent } from './order-forward/order-forward.component';
import { OrderItemIdsModalComponent } from './order-item-ids-modal/order-item-ids-modal.component';
import { RemunerationModalComponent } from './remuneration-modal/remuneration-modal.component';



@NgModule({
  declarations: [
    OrderListComponent,
    OrderLineComponent,
    OrderEditComponent,
    ReviewItemStatusNamePipe,
    ReviewStatusNamePipe,
    ContractReviewItemModalComponent,
    JdModalComponent,
    OrderForwardComponent,
    OrderItemIdsModalComponent,
    RemunerationModalComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,
    OrderRoutingModule
  ]
})
export class OrdersModule { }
