import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberLikedListComponent } from './members/member-liked-list/member-liked-list.component';
import { CvAssessComponent } from './hr/cv-assess/cv-assess.component';
import { OpenOrderItemsResolver } from './_resolvers/openOrderItemsResolver';
import { CandidateAssessedResolver } from './_resolvers/hr/candidate-assessed.resolver';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { UsersWithRolesResolver } from './_resolvers/usersWithRolesResolver';
import { HrGuard } from './_guards/hr.guard';
import { ProcessGuard } from './_guards/process.guard';


const routes: Routes = [
  {path: '', component: HomeComponent},
  {
    path:'',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      
      {path: 'candidates', canActivate:[HrGuard],
        loadChildren:() => import('./profiles/profile.module').then(mod => mod.ProfileModule)},

      {path: 'feedback', loadChildren:() => import('./feedback/feedback.module').then(mod => mod.FeedbackModule)},
      
      {path: 'hr/cvassess/:id', component: CvAssessComponent, canActivate: [HrGuard],
      resolve: {
        openOrderItemsBrief: OpenOrderItemsResolver,
        assessmentsDto: CandidateAssessedResolver,
      },
      data: {breadcrumb: 'Edit Candidate'}},
      
      {path: 'administration', loadChildren:() => import('./Administration/administration.module')
        .then(mod => mod.AdministrationModule), 
      },

      {path: 'callRecords', loadChildren: () => import('./callRecords/call-record.module').then(mod => mod.CallRecordModule)},

      {path: 'deployment', canActivate: [ProcessGuard],
        loadChildren:() => import('./deployments/deployment.module').then(mod=>mod.DeploymentModule)},
     
      {path: 'userroles', component: UserManagementComponent,
        resolve: {
          userswithroles: UsersWithRolesResolver
        }
        //, canActivate: [adminGuard]
      }
    ]
  },
  {path: 'errors', component: TestErrorComponent},
  {path: 'not-found', component: NotFoundComponent},
  {path: 'server-error', component: ServerErrorComponent},
  {path: '**', component: NotFoundComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
