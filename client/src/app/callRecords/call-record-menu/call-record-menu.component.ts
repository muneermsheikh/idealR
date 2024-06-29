import { Component } from '@angular/core';
import { take } from 'rxjs';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-call-record-menu',
  templateUrl: './call-record-menu.component.html',
  styleUrls: ['./call-record-menu.component.css']
})
export class CallRecordMenuComponent {

  user?: User;
  hasAdminRole: boolean=false;

  constructor(private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
    this.hasAdminRole = this.user !== undefined ? this.user.roles.includes('Admin') : false;
  }
}
