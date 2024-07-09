import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class ProcessGuard implements CanActivate {
  constructor(private accountService: AccountService) { }
 
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user?.roles?.includes('Document Controller-Processing')
          || user?.roles?.includes('Process Executive')
          || user?.roles?.includes('Process Supervisor')
        ) return true;
        else {
          //this.router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}});
          return false
        }
      })
    );
  }
  
}
