import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { IApplicationTaskInBrief } from "src/app/shared/models/admin/applicationTaskInBrief";
import { UserTaskService } from "src/app/shared/services/admin/user-task.service";

 export const LoggedInUserTaskResolver: ResolveFn<IApplicationTaskInBrief[]|null> = (
  ) => {
    return inject(UserTaskService).getPendingTasksOfLoggedInUser()
  };