import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { IApplicationTask } from "../_models/admin/applicationTask";
import { TaskService } from "../_services/admin/task.service";

export const TaskWithItemsFromIdResolver: ResolveFn<IApplicationTask | undefined> = (
    route: ActivatedRouteSnapshot,
  ) => {
        var id = route.paramMap.get('id');    
        return inject(TaskService).getTaskWithItems(+id!);
  };