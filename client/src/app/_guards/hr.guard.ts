import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountsService } from '../shared/services/accounts.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class HrGuard implements CanActivate {
  constructor(private accountService: AccountsService, private toastr: ToastrService, private router: Router) { }
  
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(auth => {
        if (auth) return true;
        else {
          this.router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}});
          return false
        }
      })
    );
  }
}
