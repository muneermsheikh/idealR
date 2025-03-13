import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InterviewLineComponent } from './interview-line/interview-line.component';
import { InterviewListComponent } from './interview-list/interview-list.component';
import { SharedModule } from '../_modules/shared.module';
import { InterviewEditComponent } from './interview-edit/interview-edit.component';
import { InterviewRoutingModule } from './interview-routing.module';

import { EditComponent } from './edit/edit.component';
import { EditScheduleComponent } from './edit-schedule/edit-schedule.component';
import { InterviewAttendanceComponent } from './interview-attendance/interview-attendance.component';
import { AttendanceLineComponent } from './attendance-line/attendance-line.component';
import { MatStepperModule } from '@angular/material/stepper';
import { EditAttendanceModalComponent } from './edit-attendance-modal/edit-attendance-modal.component';
import { InterviewBriefPendingModalComponent } from './interview-brief-pending-modal/interview-brief-pending-modal.component';
import { SelectionHistoryComponent } from './selection-history/selection-history.component';
import { CallRecordModule } from '../callRecords/call-record.module';
import { MatIconModule } from '@angular/material/icon';


@NgModule({
  declarations: [
    InterviewLineComponent,
    InterviewListComponent,
    InterviewEditComponent,
    EditComponent,
    EditScheduleComponent,
    InterviewAttendanceComponent,
    AttendanceLineComponent,
    EditAttendanceModalComponent,
    InterviewBriefPendingModalComponent,
    SelectionHistoryComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    MatStepperModule,
    //BrowserAnimationsModule,
    InterviewRoutingModule,

    //material
    MatIconModule
  ]
})
export class InterviewModule { }
