import { Component, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileListComponent } from './profile-list/profile-list.component';
import { CategoryListResolver } from '../_resolvers/admin/categoryListResolver';
import { AgentsResolver } from '../_resolvers/admin/agents.resolver';
import { CandidateEditComponent } from './candidate-edit/candidate-edit.component';
import { QualificationListResolver } from '../_resolvers/qualificationListResolver';
import { CandidateResolver } from '../_resolvers/admin/candidateResolver';
import { CvsAvailableToRefComponent } from './cvs-available-to-ref/cvs-available-to-ref.component';
import { ProfileMenuComponent } from './profile-menu/profile-menu.component';
import { RouterModule } from '@angular/router';
import { CvAssessModalComponent } from '../hr/cv-assess-modal/cv-assess-modal.component';
import { CandidateAssessmentDtoResolver } from '../_resolvers/hr/candidate-assessmentDtoResolver';
import { ProspectiveListComponent } from '../Administration/Prospectives/prospective-list/prospective-list.component';
import { FileuploadComponent } from './fileupload/fileupload.component';

const routes = [
    {path: '', component: ProfileMenuComponent},

    {path: 'listing', component: ProfileListComponent,
        resolve: {
          professions: CategoryListResolver,
          agents: AgentsResolver,
        },
    },
    {path: 'register/edit/:id', component:CandidateEditComponent, 
    resolve: {
      categories: CategoryListResolver,
      qualifications: QualificationListResolver,
      agents: AgentsResolver,
      candidate: CandidateResolver
    },
    data: {breadcrumb: 'Edit Candidate'}},

    {path: 'availableToRef', component: CvsAvailableToRefComponent,
        resolve: {
          professions: CategoryListResolver,
          agents: AgentsResolver
        }
    },

    {path: 'candidateAssessmentDto/:id', component: CvAssessModalComponent,
        resolve: {
          candidateAssessmentDto: CandidateAssessmentDtoResolver
        }
    },
    {path: 'prospective', component: ProspectiveListComponent},
    {path: 'fileupload', component: FileuploadComponent}

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
export class ProfileRoutingModule { }
