import { Inject, inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { take } from "rxjs";
import { IApplicationTaskInBrief } from "src/app/_models/admin/applicationTaskInBrief";
import { TaskParams } from "src/app/_models/params/Admin/taskParams";
import { User } from "src/app/_models/user";
import { AccountService } from "src/app/_services/account.service";
import { TaskService } from "src/app/_services/admin/task.service";


 export const LoggedInUserTaskResolver: ResolveFn<IApplicationTaskInBrief[]|null> = (
  ) => {
    const service= Inject(TaskService);
    var user!: User;

    inject(AccountService).currentUser$.pipe(take(1)).subscribe({next: user => user = user});

    var params = new TaskParams();
    params.assignedToUsername=user.userName;
    params.assignedByUsername=user.userName;
    service.setOParams(params);

    return service.getPendingTasksOfLoggedInUser()
  };
