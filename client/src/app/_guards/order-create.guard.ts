import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountsService } from '../shared/services/accounts.service';

@Injectable({
  providedIn: 'root'
})
export class OrderCreateGuard implements CanActivate {

  constructor(private accountsService: AccountsService){}

  canActivate(): Observable<boolean | false> {
    
    return this.accountsService.currentUser$.pipe(
      map(user => {
        if (user?.roles?.includes('Admin') 
            || user?.roles?.includes('HRManager')
            || user?.roles?.includes('DocumentController-Admin')
            ) {
          return true;
        } else {
          return false;
        }
  }))
}

}
