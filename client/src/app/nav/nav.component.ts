import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{

  model: any = {};
  
  constructor(public accountService: AccountService, private router: Router,
      private toastrService: ToastrService
  ){}
  
  ngOnInit(): void {
    var usr = this.accountService.currentUser$.subscribe({
      next: response => console.log(response)
    })
  }


  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.toastrService.success('logged in successfully');
      }
    })

  }
  

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
