import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-finance-menu',
  templateUrl: './finance-menu.component.html',
  styleUrls: ['./finance-menu.component.css']
})
export class FinanceMenuComponent {

  user?: User;

  constructor(private toastr:ToastrService, private accountService: AccountService, private router: Router) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user;
      }
    })
  }

  
  close() {
    
  }
  
  
  navigateByRoute(id: number, routeString: string) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            returnUrl: '/administration/customers' 
          } }
      );
  }

}
