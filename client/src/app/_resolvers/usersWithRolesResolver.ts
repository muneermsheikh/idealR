import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { AdminService } from "../_services/admin/admin.service";
import { IUserWithRolesDto } from "../_dtos/admin/usersWithRolesDto";

 export const UsersWithRolesResolver: ResolveFn<IUserWithRolesDto[] | undefined | null> = (
  ) => {
    var users = inject(AdminService).getUsersWithRoles();
    if(users !== null) return users;

    return of(undefined);
  };