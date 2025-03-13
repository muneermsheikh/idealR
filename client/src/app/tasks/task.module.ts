import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserTasksComponent } from './user-tasks/user-tasks.component';
import { UserTaskRoutingModule } from './user-task-routing.module';
import { SharedModule } from '../_modules/shared.module';
import { TaskMenuComponent } from './task-menu/task-menu.component';
import { TaskLineComponent } from './task-line/task-line.component';
import { EditModalComponent } from './edit-modal/edit-modal.component';
import { MatIconModule } from '@angular/material/icon';


@NgModule({
  declarations: [
    UserTasksComponent,
    TaskMenuComponent,
    TaskLineComponent,
    EditModalComponent,
  ],
  imports: [
    CommonModule,
    UserTaskRoutingModule,
    SharedModule,
    MatIconModule
  ]
})
export class TaskModule { }
