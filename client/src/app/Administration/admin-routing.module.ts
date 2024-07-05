import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuComponent } from './menu/menu.component';
import { CategoryListResolver } from '../_resolvers/admin/categoryListResolver';
import { AgentsResolver } from '../_resolvers/admin/agents.resolver';
import { RouterModule } from '@angular/router';
import { SelectionsComponent } from './selections/selections.component';
import { EmploymentsComponent } from './employments/employments.component';
import { CategoriesComponent } from './categories/categories.component';
import { IndustriesComponent } from './industries/industries.component';
import { QualificationsComponent } from './qualifications/qualifications.component';
import { UserManagementComponent } from '../admin/user-management/user-management.component';
import { OrdersListingComponent } from './orders/orders-listing/orders-listing.component';
import { OrderEditComponent } from './orders/order-edit/order-edit.component';
import { OrderResolver } from '../_resolvers/orderResolver';
import { EmployeeIdsAndKnownAsResolver } from '../_resolvers/employeeIdsAndKnownAsResolver';
import { CustomerIdAndNamesResolver } from '../_resolvers/admin/customerIdAndNamesResolver';
import { OrderAssessmentComponent } from './orders/order-assessment/order-assessment.component';
import { OrderAssessmentResolver } from '../_resolvers/orders/orderAssessmentResolver';
import { OrderAssessmentItemResolver } from '../_resolvers/orders/orderAssessmentItemResolver';
import { OrderAssessmentItemComponent } from './orders/order-assessment-item/order-assessment-item.component';
import { ContractReviewItemDtoResolver } from '../_resolvers/orders/contractReviewItemDtoResolver';
import { OrderFwdToAgentResolver } from '../_resolvers/orders/orderFwdToAgentResolver';
import { OrderFwdsComponent } from './orders/order-fwds/order-fwds.component';
import { OrderForwardComponent } from './orders/order-forward/order-forward.component';
import { OfficialIdAndCustomerNamesResolver } from '../_resolvers/admin/customers/officialIdAndCustomerNameResolver';
import { SelectionPendingComponent } from './selections/selection-pending/selection-pending.component';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerResolver } from '../_resolvers/admin/customerResolver';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';
import { FeedbackComponent } from '../feedback/feedback/feedback.component';
import { CustRvwEditModalComponent } from './cust-rvw-edit-modal/cust-rvw-edit-modal.component';
import { CustomerReviewResolver } from '../_resolvers/customerReviewResolver';
import { OrderItemReviewComponent } from './orders/order-item-review/order-item-review.component';


const routes = [
  {path: '', component: MenuComponent},
  
  {path: 'orders', component: OrdersListingComponent,
    resolve: {
      //orders: OrderBriefDtoResolver,
      professions: CategoryListResolver,
      customers: CustomerIdAndNamesResolver
    }
  },
  
  {path: 'orders/edit/:id', component: OrderEditComponent,
    resolve: {
      order: OrderResolver,
      associates: AgentsResolver,
      customers: CustomerIdAndNamesResolver,
      employees: EmployeeIdsAndKnownAsResolver,
      professions: CategoryListResolver,
      contractReviewItemDto: ContractReviewItemDtoResolver
    }
  },

  {path: 'ordersforwarded', component: OrderFwdsComponent },

  {path: 'orderfwd/:orderid', component: OrderForwardComponent,
    resolve: {
      orderfwd: OrderFwdToAgentResolver,
      customers: OfficialIdAndCustomerNamesResolver
    }
  },

  {path: 'orderassessmentitem/:id', component: OrderAssessmentItemComponent,
    resolve: {
      assessmentItem: OrderAssessmentItemResolver,
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
  },
  
  {path: 'selections', component: SelectionsComponent},

  {path: 'pendingSelections', component: SelectionPendingComponent},

  {path: 'employment', component: EmploymentsComponent},

  {path: 'categories', component: CategoriesComponent},

  {path: 'industries', component: IndustriesComponent},

  {path: 'qualifications', component: QualificationsComponent },

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

  {path: 'feedback/:id', component: FeedbackComponent,
    resolve: {
      customer: CustomerResolver
    }
  },
  
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
