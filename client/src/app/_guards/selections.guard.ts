import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class SelectionsGuard implements CanActivate {
  
  constructor(private accountsService: AccountService) {}

  canActivate(): Observable<boolean|false> {
    return this.accountsService.currentUser$.pipe(
      map(user => {
        if(
          user?.roles?.includes('Selection')
          || user?.roles?.includes("Admin")
          || user?.roles?.includes("HR Manager")
          || user?.roles?.includes("HR Supervisor")
        ) {
          return true;
        } else {
          return false;
        }
      })
    )

  }
  
}
