import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserTasksComponent } from './user-tasks/user-tasks.component';
import { UserTaskRoutingModule } from './user-task-routing.module';
import { SharedModule } from '../_modules/shared.module';
import { TaskMenuComponent } from './task-menu/task-menu.component';
import { TaskLineComponent } from './task-line/task-line.component';
import { PendingTasksComponent } from './pending-tasks/pending-tasks.component';
import { EditModalComponent } from './edit-modal/edit-modal.component';


@NgModule({
  declarations: [
    UserTasksComponent,
    TaskMenuComponent,
    TaskLineComponent,
    PendingTasksComponent,
    EditModalComponent,
  ],
  imports: [
    CommonModule,
    UserTaskRoutingModule,
    SharedModule
  ]
})
export class TaskModule { }
