import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { map } from 'rxjs';
import { AccountService } from 'src/app/_services/account.service';

export const visaEditGuard: CanActivateFn = () => {
  
  const accountService = inject(AccountService);
  
  return accountService.currentUser$.pipe(
      map(user => {
        if(!user) return false;
        return user?.roles?.includes('visaEdit')
        })
      )      
};
