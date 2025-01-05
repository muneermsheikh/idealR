import { NgModule } from '@angular/core';
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
import { CvsreferredComponent } from './cvsreferred/cvsreferred.component';
import { CandidatesAvailableToReferPagedResolver } from '../_resolvers/hr/candidatesAvailableToReferPagedResolver';
import { CvReferredPagedResolver } from '../_resolvers/admin/cvReferredPagedResolver';
import { CvAssessComponent } from './cv-assess/cv-assess.component';
import { HrGuard } from '../_guards/hr.guard';
import { OpenOrderItemsResolver } from '../_resolvers/openOrderItemsResolver';
import { CandidateAssessedResolver } from '../_resolvers/hr/candidate-assessed.resolver';
import { CandidateListingPagedResolver } from '../_resolvers/hr/candidateListingPagedResolver';

const routes = [
    {path: '', component: ProfileMenuComponent},

    {path: 'listing', component: ProfileListComponent,
        resolve: {
          professions: CategoryListResolver,
          agents: AgentsResolver,
          candidateBriefs: CandidateListingPagedResolver
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
          cvs: CandidatesAvailableToReferPagedResolver,
          professions: CategoryListResolver,
          agents: AgentsResolver
        }
    },

    {path: 'cvsreferred', component: CvsreferredComponent,
      resolve: {
        cvrefPaged: CvReferredPagedResolver
      }
    },
    {path: 'cvassess/:id', component: CvAssessComponent, canActivate: [HrGuard],
      resolve: {
        openOrderItemsBrief: OpenOrderItemsResolver,
        assessmentsDto: CandidateAssessedResolver,
      },
      data: {breadcrumb: 'CV Assessment'}},
      

    /*{path: 'candidateAssessmentDto/:id', component: CvAssessModalComponent,
        resolve: {
          candidateAssessmentDto: CandidateAssessmentDtoResolver
        }
    },
    {path: 'fileupload', component: FileuploadComponent}
    */
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
