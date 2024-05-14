import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';


const routes = [
  {path: '', component: OrderIndexComponent,  data: {breadcrumb: 'Admin Division'}},

  {path: 'edit/:id', component: OrderEditComponent,
    resolve: 
      {
        order: OrderResolver,
        agents: AgentsResolver,
        professions: CategoryListResolver,
        associates: CustomerOfficialsResolver,
        employees: EmployeeIdsAndKnownAsResolver,
        customers: CustomerNameCityResolver,
      },
      
      data: {breadcrumb: {alias: 'OrderEdit'}}
  },

  {path: 'add', component:OrderEditComponent , 
    canActivate: [OrdersCreateGuard],
  resolve: {
      professions: CategoryListResolver,
      employees: EmployeeIdsAndKnownAsResolver,
      customers: CustomerNameCityResolver,
    },
    data: {breadcrumb: {alias: 'OrderAdd'}}},

  {path: 'view/:id', component: OrderEditComponent , data: {breadcrumb: {alias: 'OrderView'}}},

  {path: 'test', component: DlEditComponent , data: {breadcrumb: {alias: 'OrderView'}}},

  {path: 'itemassess/:id', component: AssessQComponent,
    canActivate: [HrGuard],
    resolve: {
      itembrief: OrderItemBriefResolver,
      assessment: AssessmentQsResolver
  }},

    {path: 'forwards/:orderid', component: OrderForwardComponent,
    data: {breadcrumb: {alias: 'DL Forwards'}},
    resolve: {
      dlforwarddata: DLForwardsOfAnOrderIdResolver }
    }
]


@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class OrderRoutingModule { }
