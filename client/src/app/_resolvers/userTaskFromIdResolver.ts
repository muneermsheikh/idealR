import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { IApplicationTask } from "../shared/models/admin/applicationTask";
import { TaskService } from "../shared/services/task.service";

export const UserTaskFromIdResolver: ResolveFn<IApplicationTask | undefined> = (
    route: ActivatedRouteSnapshot,
  ) => {
        var id = route.paramMap.get('id');    
        return inject(TaskService).getTask(+id!);
  };