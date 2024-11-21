import { Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

import { BreakpointObserver } from '@angular/cdk/layout';   //side navi
import { ViewChild} from '@angular/core';   //sidenav
import { MatSidenav } from '@angular/material/sidenav';

import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Member } from './_models/member';
import { MemberService } from './_services/member.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
  , encapsulation: ViewEncapsulation.None
})

export class AppComponent implements OnInit {
  title = 'client';
  model: any = {};
  user?:User;
  userIsAdmin=false;
  
  constructor(public accountService: AccountService, private toastr: ToastrService,
    private router: Router
  ){}

  ngOnInit(): void {
    this.setCurrentUser();
  }

 
  setCurrentUser() {
    const userstring = localStorage.getItem('user');
    if (!userstring) return;
    this.user = JSON.parse(userstring);
    this.accountService.setCurrentUser(this.user!);
    this.userIsAdmin = this.user?.roles?.includes('Admin')!;
  }

  
  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.toastr.success('logged in successfully');
        this.model = {};
      }
    })

  }

  
  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  exportExcelProspectives() {
    this.router.navigateByUrl('/administration/excelConversion')
  }

  exportNaukriProspectives() {
    this.router.navigateByUrl('/administration/excelConversionOfNaukri')
  }

  exportExcelCustomers() {

  }

  exportExcelEmployees() {

  }

  editLoggedinMember() {
     var username = this.user?.userName;
     if(username===null || username === undefined) return;
    
     var member = inject(MemberService).getMember(username).subscribe({
        next: (response: Member) => {
          this.navigateByRoute(username!, '/members/edit', response);
        }
     })

  }

    
  navigateByRoute(id: string, routeString: string, object: Member) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            member: object,
            returnUrl: '/' 
          } }
      );
  }

}
