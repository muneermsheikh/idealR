import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserTasksComponent } from './user-tasks/user-tasks.component';
import { RouterModule } from '@angular/router';
import { LoggedInUserTaskResolver } from '../_resolvers/admin/loggedInUserTaskResolver';

const routes = [
  {path: '', component: UserTasksComponent, resolve: {tasks: LoggedInUserTaskResolver}},
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
