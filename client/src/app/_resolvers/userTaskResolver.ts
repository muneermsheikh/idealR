import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IApplicationTaskInBrief } from "../shared/models/admin/applicationTaskInBrief";
import { TaskService } from "../shared/services/task.service";
 
export const QBankByCategoryIdResolver: ResolveFn<IApplicationTaskInBrief[]> = (
  ) => {
        return inject(TaskService).getTasks(false);
  };