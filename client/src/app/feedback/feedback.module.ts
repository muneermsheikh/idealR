import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FeedbackComponent } from './feedback/feedback.component';
import { FeedbackListComponent } from './feedback-list/feedback-list.component';
import { SharedModule } from '../_modules/shared.module';
import { FeedbackRoutingModule } from './feedback-routing.module';
import { FeedbackMenuComponent } from './feedback-menu/feedback-menu.component';
import { FeedbackStddQsComponent } from './feedback-stdd-qs/feedback-stdd-qs.component';
import { FeedbackLineComponent } from './feedback-line/feedback-line.component';



@NgModule({
  declarations: [
    FeedbackComponent,
    FeedbackListComponent,
    FeedbackMenuComponent,
    FeedbackStddQsComponent,
    FeedbackLineComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    FeedbackRoutingModule
  ]
})
export class FeedbackModule { }
