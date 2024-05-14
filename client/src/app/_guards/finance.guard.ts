import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Observable, map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class FinanceGuard implements CanActivate {
  
  constructor(private accountsService: AccountService, private toastr: ToastrService) {}

  canActivate(): Observable<boolean|false> {
    return this.accountsService.currentUser$.pipe(
      map(user => {
        if(user?.roles?.includes('Finance')) {
          return true;
        } else {
          this.toastr.error('Unauthorized');  
          return false;
        }
      })
    )

  }
}
