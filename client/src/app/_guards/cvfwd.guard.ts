import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';
import { map } from 'rxjs';

export const cvfwdGuard: CanActivateFn = (route, state) => {
  
  const accountService = inject(AccountService);
  
  return accountService.currentUser$.pipe(
    map(user => {
      if(!user) return false;
      if(user.roles.length === 0) return false;

      return user.roles.includes('Document Controller-Admin');
    })
  )
};
