import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FeedbackComponent } from './feedback/feedback.component';
import { FeedbackListComponent } from './feedback-list/feedback-list.component';
import { SharedModule } from '../_modules/shared.module';
import { FeedbackRoutingModule } from './feedback-routing.module';
import { FeedbackMenuComponent } from './feedback-menu/feedback-menu.component';
import { FeedbackLineComponent } from './feedback-line/feedback-line.component';
import { NgxPrintModule } from 'ngx-print';
import { MatIconModule } from '@angular/material/icon';



@NgModule({
  declarations: [
    FeedbackComponent,
    FeedbackListComponent,
    FeedbackMenuComponent,
    FeedbackLineComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    FeedbackRoutingModule,
    NgxPrintModule,
    MatIconModule
  ]
})
export class FeedbackModule { }
