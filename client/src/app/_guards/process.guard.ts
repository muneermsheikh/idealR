import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class ProcessGuard implements CanActivate {
  constructor(private accountService: AccountService, private toastr: ToastrService) { }
 
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        const roles = user!.roles;
        console.log('admin included:', roles.includes('Admin'), roles);

        if (roles.includes('Document Controller-Processing')
          || roles.includes('Process Executive')
          || roles.includes('Process Supervisor')
          || roles.includes('Admin')
        ) return true;
        else {
          //this.router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}});
          this.toastr.warning('You do not have authorization to acess this resource');
          return false
        }
     })
    );
  }
  
}
