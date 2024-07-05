import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FeedbackListComponent } from './feedback-list/feedback-list.component';
import { FeedbackComponent } from './feedback/feedback.component';
import { FeedbackMenuComponent } from './feedback-menu/feedback-menu.component';
import { FeedbackStddQsComponent } from './feedback-stdd-qs/feedback-stdd-qs.component';
import { FeedbackInputResolver } from '../_resolvers/admin/feedbackInputResolver';

const routes = [
  {path: '', component: FeedbackMenuComponent},
  {path: 'list', component: FeedbackListComponent},
  {path: 'edit/:id', component: FeedbackComponent,
      resolve: {
        fdbackInput: FeedbackInputResolver
      }
  },
  {path: 'stddQs', component: FeedbackStddQsComponent}
];


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ], exports: [
    RouterModule
  ]
})
export class FeedbackRoutingModule { }
