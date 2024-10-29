import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserTasksComponent } from './user-tasks/user-tasks.component';
import { RouterModule } from '@angular/router';
import { TaskMenuComponent } from './task-menu/task-menu.component';
import { PendingTasksComponent } from './pending-tasks/pending-tasks.component';
import { ObjectiveReportComponent } from './objective-report/objective-report.component';

const routes = [
  {path: '', component: TaskMenuComponent},
  {path: 'loggedInUserTasks',  component: UserTasksComponent},
  {path: 'tasksforadmin', component: PendingTasksComponent},
  {path: 'objectives', component: ObjectiveReportComponent},
  {path: 'medObjectives', component: ObjectiveReportComponent}
  
]


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class UserTaskRoutingModule { }
