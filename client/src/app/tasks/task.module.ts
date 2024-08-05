import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserTasksComponent } from './user-tasks/user-tasks.component';
import { UserTaskRoutingModule } from './user-task-routing.module';
import { SharedModule } from '../_modules/shared.module';
import { TaskMenuComponent } from './task-menu/task-menu.component';
import { TaskEditModalComponent } from './task-edit-modal/task-edit-modal.component';
import { TaskLineComponent } from './task-line/task-line.component';



@NgModule({
  declarations: [
    UserTasksComponent,
    TaskMenuComponent,
    TaskEditModalComponent,
    TaskLineComponent
  ],
  imports: [
    CommonModule,
    UserTaskRoutingModule,
    SharedModule
  ]
})
export class TaskModule { }
