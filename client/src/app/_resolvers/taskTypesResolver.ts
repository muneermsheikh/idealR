import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { ITaskType } from "../shared/models/admin/taskType";
import { TaskService } from "../shared/services/task.service";

 
export const TaskTypeResolver: ResolveFn<ITaskType[] > = (
    ) => {
        return inject(TaskService).getTaskTypes();
    };