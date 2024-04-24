import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = (route, state) => {

  const accountService = inject(AccountService);
  const toastrService = inject(ToastrService);

  return accountService.currentUser$.pipe(
    map(user => {
      if(!user) return false;
      console.log('adminguard user roles', user.roles, user.roles.includes('admin'));

      if(user.roles.length===0) {
        toastrService.info('no roles defined for the user');
        return false;
      } else {
        console.log('adminguard user.roles:', user.roles);
      }

      if(user.roles.includes('Admin')) {
        console.log('admin role present');
        toastrService.success('roles authorized');
        return true;
      } else {
        toastrService.warning('user does not have admin credentials');
        return false;
      }
    })
  )

};
