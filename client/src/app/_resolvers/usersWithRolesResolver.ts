import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { IUser } from "../shared/models/admin/user";
import { AdminService } from "../shared/services/admin/admin.service";
import { of } from "rxjs";

 export const UsersWithRolesResolver: ResolveFn<IUser[] | undefined | null> = (
  ) => {
    var users = inject(AdminService).getUsersWithRoles();
    if(users !== null) return users;

    return of(undefined);
  };