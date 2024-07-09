import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { CanActivate } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class OrderCreateGuard implements CanActivate {

  constructor(private accountsService: AccountService){}

  canActivate(): Observable<boolean | false> {
    
    return this.accountsService.currentUser$.pipe(
      map(user => {
        if (user?.roles?.includes('OrderCreate') 
            //|| user?.roles?.includes('HRManager')
            //|| user?.roles?.includes('Document Controller-Admin')
            ) {
          return true;
        } else {
          return false;
        }
  }))
}

}
