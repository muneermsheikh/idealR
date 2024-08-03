import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserTasksComponent } from './user-tasks/user-tasks.component';
import { UserTaskRoutingModule } from './user-task-routing.module';
import { SharedModule } from '../_modules/shared.module';



@NgModule({
  declarations: [
    UserTasksComponent
  ],
  imports: [
    CommonModule,
    UserTaskRoutingModule,
    SharedModule
  ]
})
export class TaskModule { }
