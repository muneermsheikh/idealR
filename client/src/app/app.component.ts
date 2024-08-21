import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

import { BreakpointObserver } from '@angular/cdk/layout';   //side navi
import { ViewChild} from '@angular/core';   //sidenav
import { MatSidenav } from '@angular/material/sidenav';

import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

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

  exportExcelCustomers() {

  }

  exportExcelEmployees() {

  }



}
