import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class FinanceGuard implements CanActivate {
  
  constructor(private accountsService: AccountService) {}

  canActivate(): Observable<boolean|false> {
    return this.accountsService.currentUser$.pipe(
      map(user => {
        if(user?.roles?.includes('Finance')) {
          return true;
        } else {
          return false;
        }
      })
    )

  }
}
