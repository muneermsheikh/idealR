import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FeedbackListComponent } from './feedback-list/feedback-list.component';
import { FeedbackComponent } from './feedback/feedback.component';
import { FeedbackMenuComponent } from './feedback-menu/feedback-menu.component';
import { FeedbackInputResolver } from '../_resolvers/admin/feedbackInputResolver';
import { FeedbackHistoryResolver } from '../_resolvers/admin/feedbackHistoryResolver';


const routes = [
  {path: '', component: FeedbackMenuComponent},
  {path: 'list', component: FeedbackListComponent},
  {path: 'edit/:id/:customerId', component: FeedbackComponent,
      resolve: {
        feedback: FeedbackInputResolver,
        history: FeedbackHistoryResolver
      }
  }
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
