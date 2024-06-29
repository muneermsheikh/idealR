import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes } from '@angular/router';
import { CandidateListComponent } from './candidate-list/candidate-list.component';
import { CategoryListResolver } from '../_resolvers/admin/categoryListResolver';
import { CandidateResolver } from '../_resolvers/admin/candidateResolver';
import { CandidateHistoryComponent } from './candidate-history/candidate-history.component';
import { QualificationListResolver } from '../_resolvers/qualificationListResolver';
import { AgentsResolver } from '../_resolvers/admin/agents.resolver';
import { CandidateEditComponent } from '../profiles/candidate-edit/candidate-edit.component';
import { CvsAvailableToRefComponent } from '../profiles/cvs-available-to-ref/cvs-available-to-ref.component';
import { CandidateBriefResolver } from '../_resolvers/hr/candidateBriefResolver';
import { ProspectiveListComponent } from '../Administration/Prospectives/prospective-list/prospective-list.component';


const routes: Routes = [
  {path: '', component: CandidateListComponent, data: {breadcrumb: 'Candidate List'}},
  {path: 'register/edit/:id', component:CandidateEditComponent, 
      resolve: {
        categories: CategoryListResolver,
        qualifications: QualificationListResolver,
        agents: AgentsResolver,
        candidate: CandidateResolver
      },
      data: {breadcrumb: 'Edit Candidate'}},
  {path: 'candidatehistory', component: CandidateHistoryComponent, data: {breadcrumb: 'Register'}},
  {path: '**', redirectTo: 'not-found', pathMatch: 'full'},

  {path: 'cvsAvailableToRef', component: CvsAvailableToRefComponent,
    resolve: {
      cvs: CandidateBriefResolver
    }
  }
  
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class CandidateRoutingModule { }
