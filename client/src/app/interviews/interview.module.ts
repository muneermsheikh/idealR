import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InterviewLineComponent } from './interview-line/interview-line.component';
import { InterviewListComponent } from './interview-list/interview-list.component';
import { SharedModule } from '../_modules/shared.module';
import { InterviewAssignmentComponent } from './interview-assignment/interview-assignment.component';
import { InterviewEditComponent } from './interview-edit/interview-edit.component';
import { InterviewRoutingModule } from './interview-routing.module';

import { EditComponent } from './edit/edit.component';
import { EditScheduleComponent } from './edit-schedule/edit-schedule.component';



@NgModule({
  declarations: [
    InterviewLineComponent,
    InterviewListComponent,
    InterviewAssignmentComponent,
    InterviewEditComponent,
    EditComponent,
    EditScheduleComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    InterviewRoutingModule
  ]
})
export class InterviewModule { }
