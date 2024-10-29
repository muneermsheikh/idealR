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
import { InterviewAttendanceComponent } from './interview-attendance/interview-attendance.component';
import { AttendanceLineComponent } from './attendance-line/attendance-line.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatStepperModule } from '@angular/material/stepper';


@NgModule({
  declarations: [
    InterviewLineComponent,
    InterviewListComponent,
    InterviewAssignmentComponent,
    InterviewEditComponent,
    EditComponent,
    EditScheduleComponent,
    InterviewAttendanceComponent,
    AttendanceLineComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    MatStepperModule,
    //BrowserAnimationsModule,
    InterviewRoutingModule
  ]
})
export class InterviewModule { }
