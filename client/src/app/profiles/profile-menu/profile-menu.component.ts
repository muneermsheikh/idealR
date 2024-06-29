import { Component } from '@angular/core';
import { Route, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-profile-menu',
  templateUrl: './profile-menu.component.html',
  styleUrls: ['./profile-menu.component.css']
})
export class ProfileMenuComponent {

  user?: User

  uploadExcel=false;

  constructor(private router: Router, private accountService: AccountService, 
      private toastr: ToastrService, private confirm: ConfirmService){

    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user)
          this.user = user;
      }
    })
  }

  OpenAvailableCandidatesComponent() {
    this.navigateByRoute(0, "/candidates/availablecandidates", false);
  }

  OpenCandidatesListingComponent() {
      this.navigateByRoute(0, "/candidates/listing", false);
  }

  OpenCallRecordMenu() {
    this.navigateByRoute(0, "/callRecords/callRecordList", false);
  }

  navigateByRoute(id: number, routeString: string, editable: boolean) {
    
    let route = id===0 ? routeString : routeString + '/' + id;
    
    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            toedit: editable, 
            returnUrl: '/administration/orders' 
          } }
      );
  }
  
  importProspectiveExcel() {
      this.uploadExcel = !this.uploadExcel;
  }

  OpenActiveProspectiveList() {
    this.navigateByRoute(0, "/candidates/prospective", false);
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

