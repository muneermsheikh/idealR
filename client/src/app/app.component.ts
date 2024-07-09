import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

import { BreakpointObserver } from '@angular/cdk/layout';   //side navi
import { ViewChild} from '@angular/core';   //sidenav
import { MatSidenav } from '@angular/material/sidenav';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client';
  uploadExcel: boolean=false;
  
  constructor(private accountService: AccountService, private toastr: ToastrService){}

  ngOnInit(): void {
    this.setCurrentUser();
  }

 
  setCurrentUser() {
    const userstring = localStorage.getItem('user');
    if (!userstring) return;
    const user: User = JSON.parse(userstring);
    this.accountService.setCurrentUser(user);
  }

  exportExcelProspectives() {

  }

  exportExcelCustomers() {

  }

  exportExcelEmployees() {

  }

  
  importCustomerExcel() {
    this.uploadExcel = !this.uploadExcel;
  }


  onFileInputChange(event: Event) {
      var formData = new FormData();
      const target = event.target as HTMLInputElement;
      const files = target.files as FileList;
      const f = files[0];

      if(f.size > 0) 
      {
          formData.append('file', f, f.name);
          
          this.accountService.copyProspectiveXLSFileToDB(formData).subscribe({
            next: (response: string) => {
              if(response === '') {
                this.toastr.success(response + ' file(s) copied to database', 'success')
                } else {
                  this.toastr.warning(response, 'Failed to copy the excel data to database')
                }
            }
            , error: (err: any) => {
              console.log(err.error.text, 'Error encountered');
              this.toastr.error(err.error.text, 'Error in copying the excel data to database');
            }
          })
          
          //make the FILE INPUT button invisible
          this.uploadExcel=false;
      }
  }
}
