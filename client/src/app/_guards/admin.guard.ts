import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { map } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {

  const accountService = inject(AccountService);

  return accountService.currentUser$.pipe(
    map(user => {
      if(!user) return false;
      //console.log('adminguard user roles', user.roles, user.roles.includes('admin'));

      if(user?.roles?.length===0) return false;

      return user?.roles?.includes('Admin');
      
    })
  )

};
