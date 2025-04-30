import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserTasksComponent } from './user-tasks/user-tasks.component';
import { UserTaskRoutingModule } from './user-task-routing.module';
import { SharedModule } from '../_modules/shared.module';
import { TaskMenuComponent } from './task-menu/task-menu.component';
import { TaskLineComponent } from './task-line/task-line.component';

import { MatIconModule } from '@angular/material/icon';
import { TaskEditModalComponent } from './task-edit-modal/task-edit-modal.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    UserTasksComponent,
    TaskMenuComponent,
    TaskLineComponent,
    TaskEditModalComponent
  ],
  imports: [
    CommonModule,
    UserTaskRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    MatIconModule
  ]
})
export class TaskModule { }
