import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit{

  users: any;
  
  registerMode = false;

  constructor(public accountService: AccountService, private router: Router){}

  ngOnInit(): void {
    //this.router.navigateByUrl('/tasks/loggedInUserTasks')
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(event: any) {
    this.registerMode = event;
  }
}
