import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersListingComponent } from './orders-listing/orders-listing.component';
import { CategoryListResolver } from '../_resolvers/admin/categoryListResolver';
import { CustomerIdAndNamesResolver } from '../_resolvers/admin/customerIdAndNamesResolver';
import { DLComponent } from './dl/dl.component';
import { OrderResolver } from '../_resolvers/orderResolver';
import { EmployeeIdsAndKnownAsResolver } from '../_resolvers/employeeIdsAndKnownAsResolver';
import { OrderForwardComponent } from './order-forward/order-forward.component';
import { OrderFwdToAgentResolver } from '../_resolvers/orders/orderFwdToAgentResolver';
import { OfficialIdAndCustomerNamesResolver } from '../_resolvers/admin/customers/officialIdAndCustomerNameResolver';
import { OrderAssessmentItemComponent } from './order-assessment-item/order-assessment-item.component';
import { OrderAssessmentItemResolver } from '../_resolvers/orders/orderAssessmentItemResolver';
import { OrderItemReviewComponent } from './order-item-review/order-item-review.component';
import { ContractReviewItemDtoResolver } from '../_resolvers/orders/contractReviewItemDtoResolver';
import { OrderAssessmentComponent } from './order-assessment/order-assessment.component';
import { OrderAssessmentResolver } from '../_resolvers/orders/orderAssessmentResolver';
import { RouterModule } from '@angular/router';
import { OrderMenuComponent } from './order-menu/order-menu.component';
import { DLForwardedComponent } from './DLForwarded/DLForwarded.component';
import { DLForwardedResolver } from '../_resolvers/orders/DLForwardedResolver';

const routes = [
  {path: '', component: OrderMenuComponent},
  
  {path: 'orders', component: OrdersListingComponent,
    resolve: {
      //orders: OrderBriefDtoResolver,
      professions: CategoryListResolver,
      customers: CustomerIdAndNamesResolver
    }
  },

  //{path: 'orderReview/:id', component: OrderFwdsComponent },

  {path: 'edit/:id', component: DLComponent,
    resolve: {
      order: OrderResolver,
      //associates: AgentsResolver,
      customers: CustomerIdAndNamesResolver,
      employees: EmployeeIdsAndKnownAsResolver,
      professions: CategoryListResolver,
      //contractReviewItemDto: ContractReviewItemDtoResolver
    }
  },

  {path: 'DLForwarded', component: DLForwardedComponent,
    resolve: {
      DLForwarded: DLForwardedResolver
    }},

  {path: 'ordersforwarded/:orderid', component: OrderForwardComponent,
    resolve: {
      orderfwd: OrderFwdToAgentResolver,
      customers: OfficialIdAndCustomerNamesResolver
    }
  },

  {path: 'orderassessmentitem/:id', component: OrderAssessmentItemComponent,
    resolve: {
      assessmentItem: OrderAssessmentItemResolver
    }
  },

  {path: 'orderitemreview/:id', component: OrderItemReviewComponent,
    resolve: {
      orderitemreview: ContractReviewItemDtoResolver
    }
  },

  {path: 'orderassessment/:id', component: OrderAssessmentComponent,
    resolve: {
      orderAssessment: OrderAssessmentResolver
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
export class OrderRoutingModule { }
