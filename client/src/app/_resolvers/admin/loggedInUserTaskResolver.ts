import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IApplicationTaskInBrief } from "src/app/_models/admin/applicationTaskInBrief";
import { UserTaskService } from "src/app/_services/admin/task.service";

 export const LoggedInUserTaskResolver: ResolveFn<IApplicationTaskInBrief[]|null> = (
  ) => {
    return inject(UserTaskService).getPendingTasksOfLoggedInUser()
  };