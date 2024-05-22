import { Injectable, inject } from '@angular/core';

import {  
  ResolveFn
} from '@angular/router';
import { AdminService } from 'src/app/_services/admin/admin.service';


export const RolesResolver: ResolveFn<string[]|null> = (
  ) => {
    return inject(AdminService).getIdentityRoles();
  };