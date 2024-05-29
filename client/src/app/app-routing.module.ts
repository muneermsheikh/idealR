import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MessagesComponent } from './messages/messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberLikedListComponent } from './members/member-liked-list/member-liked-list.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { CategoryListResolver } from './_resolvers/admin/categoryListResolver';
import { AgentsResolver } from './_resolvers/admin/agents.resolver';
import { ProfileListComponent } from './profiles/profile-list/profile-list.component';
import { QualificationListResolver } from './_resolvers/qualificationListResolver';
import { CandidateResolver } from './_resolvers/admin/candidateResolver';
import { CandidateEditComponent } from './profiles/candidate-edit/candidate-edit.component';
import { CvAssessComponent } from './hr/cv-assess/cv-assess.component';
import { OpenOrderItemsResolver } from './_resolvers/openOrderItemsResolver';
import { CandidateAssessedResolver } from './_resolvers/hr/candidate-assessed.resolver';


const routes: Routes = [
  {path: '', component: HomeComponent},
  {
    path:'',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard,],
    children: [
      //{path: 'candidates', loadChildren:() => import('./candidates/candidate.module').then(mod => mod.CandidateModule), data: {breadcrumb: 'Candidates'}},
       {path: 'candidates', component: ProfileListComponent,
          resolve: {
            professions: CategoryListResolver,
            agents: AgentsResolver,
          },
      },
      {path: 'candidates/register/edit/:id', component:CandidateEditComponent, 
      resolve: {
        categories: CategoryListResolver,
        qualifications: QualificationListResolver,
        agents: AgentsResolver,
        candidateBriefDto: CandidateResolver
      },
      data: {breadcrumb: 'Edit Candidate'}},

      {path: 'hr/cvassess/:id', component: CvAssessComponent, 
      resolve: {
        openOrderItemsBrief: OpenOrderItemsResolver,
        assessmentsDto: CandidateAssessedResolver,
        //candidate: CandidateResolver
      },
      data: {breadcrumb: 'Edit Candidate'}},
      
      /* {path: 'members', component: MemberListComponent, canActivate: [authGuard]},
      {path: 'members/:username', component: MemberDetailsComponent,
          resolve: {member: memberDetailedResolver}},
      {path: 'member/edit', component: MemberEditComponent, canDeactivate: [preventUnsavedMemberEditGuard]},
      */
      {path: 'messages', component:MessagesComponent},
      {path: 'memberlikes', component: MemberLikedListComponent},
      {path: 'admin', component: AdminPanelComponent
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
