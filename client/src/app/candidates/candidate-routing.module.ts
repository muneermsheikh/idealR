import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes } from '@angular/router';
import { CandidateListComponent } from './candidate-list/candidate-list.component';
import { CandidateEditComponent } from './candidate-edit/candidate-edit.component';
import { CategoryListResolver } from '../_resolvers/admin/categoryListResolver';
import { QualificationsResolver } from '../_resolvers/qualificationsResolver';
import { CandidateResolver } from '../_resolvers/admin/candidateResolver';
import { CandidateHistoryComponent } from './candidate-history/candidate-history.component';


const routes: Routes = [
  {path: 'candidatelist', component: CandidateListComponent, data: {breadcrumb: 'Candidate List'}},
  {path: 'register/edit/:id', component:CandidateEditComponent, 
      resolve: {
        categories: CategoryListResolver,
        qualifications: QualificationsResolver,
        //agents: AgentsResolver,
        candidate: CandidateResolver
      },
      data: {breadcrumb: 'Edit Candidate'}},
  {path: 'candidatehistory', component: CandidateHistoryComponent, data: {breadcrumb: 'Register'}},
  {path: '**', redirectTo: 'not-found', pathMatch: 'full'}
  
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class CandidateRoutingModule { }
