import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserTasksComponent } from './user-tasks/user-tasks.component';
import { UserTaskRoutingModule } from './user-task-routing.module';
import { SharedModule } from '../_modules/shared.module';
import { TaskMenuComponent } from './task-menu/task-menu.component';
import { TaskLineComponent } from './task-line/task-line.component';
import { PendingTasksComponent } from './pending-tasks/pending-tasks.component';
import { EditModalComponent } from './edit-modal/edit-modal.component';
import { ObjectiveReportComponent } from './objective-report/objective-report.component';
import { MedObjectivesComponent } from './med-objectives/med-objectives.component';



@NgModule({
  declarations: [
    UserTasksComponent,
    TaskMenuComponent,
    TaskLineComponent,
    PendingTasksComponent,
    EditModalComponent,
    ObjectiveReportComponent,
    MedObjectivesComponent
  ],
  imports: [
    CommonModule,
    UserTaskRoutingModule,
    SharedModule
  ]
})
export class TaskModule { }
