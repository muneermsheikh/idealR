import { Component, Inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IReturnStringsDto } from 'src/app/_dtos/admin/returnStringsDto';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-fileupload',
  templateUrl: './fileupload.component.html',
  styleUrls: ['./fileupload.component.css']
})
export class FileuploadComponent {

  uploadExcel: boolean=false;

  constructor(private toastr:ToastrService, private accountService: AccountService){}

  onFileInputChange(event: Event) {
    var formData = new FormData();
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];

    if(f.size > 0) 
    {
        formData.append('file', f, f.name);
        
        this.accountService.copyProspectiveXLSFileToDB(formData).subscribe({
          next: (response: IReturnStringsDto) => {
            if(response.successString !== '') {
              this.toastr.success(response.successString, 'success')
              } else {
                this.toastr.warning(response.errorString, 'Failed to copy the excel data to database')
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

  
  
  importCustomerExcel() {
    this.uploadExcel = !this.uploadExcel;
  }
  

  
}
