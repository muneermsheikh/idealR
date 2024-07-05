import { Component } from '@angular/core';
import { Navigation, Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-feedback-menu',
  templateUrl: './feedback-menu.component.html',
  styleUrls: ['./feedback-menu.component.css']
})
export class FeedbackMenuComponent {

    user?: User;
    returnUrl='';


    constructor(private router: Router, private accountService: AccountService) {
          this.accountService.currentUser$.pipe(take(1)).subscribe({
            next: user => {
              if (user)
                this.user = user;
            }
          })
          
          let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
          }
    }

    ListClicked() {
      this.navigateByUrl('/feedback/list');
    }

    StddQsClicked() {
      
    }

    GoHomeClicked() {

    }

    
    navigateByUrl(route: string) {
      this.router.navigate(
        [route],
        { state: 
          { 
            user: this.user,
            returnUrl: '/feedback' 
          } }
      )
    }
    
  }


