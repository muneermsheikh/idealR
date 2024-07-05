import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent {

  uploadExcel=false;

  constructor(private toastr:ToastrService, private accountService: AccountService) {}

  close() {
    
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
