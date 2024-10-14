import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IApplicationTask } from "../_models/admin/applicationTask";
import { TaskService } from "../_services/admin/task.service";
 
export const TaskWithOrderIdAndTaskTypeResolver: ResolveFn<IApplicationTask | null> = (
    route: ActivatedRouteSnapshot,
    ) => {
        var id = route.paramMap.get('orderid');
        var tasktypeid = route.paramMap.get('tasktypeid')!;
        if (id===null) return of(null);
          return inject(TaskService).getTaskByOrderIdAndTaskType(+id, tasktypeid);
    };