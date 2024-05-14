import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountsService } from '../shared/services/accounts.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class OrdersCreateGuard implements CanActivate {
  
  constructor(private accountService: AccountsService, private toaster:ToastrService) { }
  
  canActivate(): Observable<boolean|false> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user?.roles?.includes('OrderCreate') ) {
          return true;
        } else{
          this.toaster.warning('The logged in user does not have authority to use this function', 
          'Not authorized', {closeButton: true, timeOut: 0});
          return false;
        }
      })
    )
  }
  
}
