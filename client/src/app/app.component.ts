import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { user } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client';

  constructor(private accountService: AccountService){}

  ngOnInit(): void {
    this.setCurrentUser();
  }

 
  setCurrentUser() {
    const userstring = localStorage.getItem('user');
    if (!userstring) return;
    const user: user = JSON.parse(userstring);
    this.accountService.setCurrentUser(user);
  }
}
