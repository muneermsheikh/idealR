import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { UsersWithRolesResolver } from './_resolvers/usersWithRolesResolver';
import { HrGuard } from './_guards/hr.guard';
import { RolesResolver } from './_resolvers/identity/roles.resolver';
import { TrialExpiredComponent } from './errors/trial-expired/trial-expired.component';


const routes: Routes = [
  {path: '', component: HomeComponent},
  {
    path:'',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      {path: 'candidates', canActivate:[HrGuard],
        loadChildren:() => import('./profiles/profile.module').then(mod => mod.ProfileModule)},
        
      {path: 'prospectives', canActivate:[HrGuard], loadChildren: () =>  import ('./prospectives/prospective.module').then(mod => mod.ProspectiveModule)},

      {path: 'feedback', loadChildren:() => import('./feedback/feedback.module').then(mod => mod.FeedbackModule)},
      {path: 'tasks', loadChildren:() => import('./tasks/task.module').then(mod => mod.TaskModule)},
      
      {path: 'quality', loadChildren: () => import('./quality/quality.module').then(mod => mod.QualityModule)},
      
      /* {path: 'hr/cvassess/:id', component: CvAssessComponent, canActivate: [HrGuard],
      resolve: {
        openOrderItemsBrief: OpenOrderItemsResolver,
        assessmentsDto: CandidateAssessedResolver,
      },
      data: {breadcrumb: 'Edit Candidate'}}, */
      
      {path: 'administration', loadChildren:() => import('./Administration/administration.module')
        .then(mod => mod.AdministrationModule), 
      },
      {path: 'orders', loadChildren: () => import('./orders/order.module').then(mod => mod.OrderModule)},
      {path: 'selections', loadChildren: () => import('./selections/selection.module').then(mod => mod.SelectionModule)},
      {path: 'masters', loadChildren:() => import('./masters/master.module').then(mod => mod.MasterModule)},
      {path: 'tasks', loadChildren: () => import('./tasks/task.module').then(mod => mod.TaskModule)},
      
      {path: 'finance', loadChildren:() => import('./finance/finance.module').then(mod => mod.FinanceModule) },
      {path: 'members', loadChildren:() => import('./members/member.module').then(mod => mod.MemberModule)}, 
      
      {path: 'callRecords', loadChildren: () => import('./callRecords/call-record.module').then(mod => mod.CallRecordModule)},

      {path: 'messages', loadChildren: () => import('./messages/message.module').then(mod => mod.MessageModule)},

      {path: 'deployment', //canActivate: [ProcessGuard],
        loadChildren:() => import('./deployments/deployment.module').then(mod=>mod.DeploymentModule)},
     
      {path: 'interviews', //canActivate: [ProcessGuard],
        loadChildren:() => import('./interviews/interview.module').then(mod=>mod.InterviewModule)},
      
      {path: 'visas', loadChildren: () => import('./visas/visa.module').then(mod => mod.VisaModule)},

      {path: 'userroles', component: UserManagementComponent,
        resolve: {
          userswithroles: UsersWithRolesResolver,
          roles: RolesResolver
        }
        //, canActivate: [adminGuard]
      },
      
    ]
  },
  {path: 'errors', component: TestErrorComponent},
  {path: 'not-found', component: NotFoundComponent},
  {path: 'server-error', component: ServerErrorComponent},
  {path: 'activation-error', component: TrialExpiredComponent},
  {path: '**', component: NotFoundComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
