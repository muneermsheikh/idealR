import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { IReturnStringsDto } from 'src/app/_dtos/admin/returnStringsDto';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { CategoryService } from 'src/app/_services/category.service';

@Component({
  selector: 'app-excel-conversion-menu',
  templateUrl: './excel-conversion-menu.component.html',
  styleUrls: ['./excel-conversion-menu.component.css']
})
export class ExcelConversionMenuComponent {

  uploadExcel=false;
  uploadNaukriExcel=false;
  uploadCustomerExcel=false;
  uploadEmployeeExcel=false;
  uploadOrderExcel=false;
  uploadCandidateExcel=false;
  uploadProfessionExcel=false;

  user?: User;
  
  formData = new FormData();

  constructor(private toastr:ToastrService, private accountService: AccountService,
      private catService: CategoryService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user;
        //console.log('user', this.user);
      }
    })
  }
  
  //Professions
  //prospective
  toggleProfessionExcel() {
    this.uploadProfessionExcel=!this.uploadProfessionExcel;
    this.uploadNaukriExcel=false;
    this.uploadExcel=false;
    this.uploadCustomerExcel=false;
    this.uploadCandidateExcel=false;
    this.uploadEmployeeExcel=false;
    this.uploadOrderExcel=false;

    console.log('rglprof', this.uploadProfessionExcel);
  }
  
  //prospective
  toggleProspectiveExcel() {
    this.uploadExcel=!this.uploadExcel;
    this.uploadNaukriExcel=false;
    this.uploadCustomerExcel=false;
    this.uploadCandidateExcel=false;
    this.uploadEmployeeExcel=false;
    this.uploadOrderExcel=false;
    this.uploadProfessionExcel=false;
  }

  //prospective
  toggleNaukriProspectiveExcel() {
    this.uploadNaukriExcel=!this.uploadNaukriExcel;
    this.uploadExcel=false;
    this.uploadCustomerExcel=false;
    this.uploadCandidateExcel=false;
    this.uploadEmployeeExcel=false;
    this.uploadOrderExcel=false;
    this.uploadProfessionExcel=false;
  }


  exportProspectiveFile() {
    this.accountService.copyProspectiveXLSFileToDB(this.formData).subscribe({
      next: (response: IReturnStringsDto) => {
        //console.log('exportprospective: response:', response);
        if(response.errorString !== '') this.toastr.warning(response.errorString, 'Warning')
        if(response.successString !== '') this.toastr.success(response.successString, 'Success');
      }
      , error: (err: any) => {
        console.log(err.error.text, 'Error encountered');
        this.toastr.error(err.error.text, 'Error in copying the excel data to database');
      }
    })
  }
  
  exportNaukriProspectiveFile() {
    this.accountService.copyProspectiveNaukriXLSFileToDB(this.formData).subscribe({
      next: (response: IReturnStringsDto) => {
        //console.log('exportprospective: response:', response);
        if(response.errorString !== '') this.toastr.warning(response.errorString, 'Warning');
        if(response.successString !== '') this.toastr.success(response.successString, 'Success');
        
      }
      , error: (err: any) => {
        console.log(err.error.text, 'Error encountered');
        this.toastr.error(err.error.text, 'Error in copying the excel data to database');
      }
    })
  }

  onFileInputChange(event: Event)
  {
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];
    this.formData = new FormData();

    if(f.size > 0) this.formData.append('file', f, f.name);
  
  }

  onNaukriProspectiveFileInputChange(event: Event) {
      
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];
    this.formData = new FormData();

    if(f.size > 0) this.formData.append('file', f, f.name);
        
  }
  

  onProspectiveFileInputChange(event: Event) {
      
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];
    this.formData = new FormData();

    if(f.size > 0) this.formData.append('file', f, f.name);
        
  }

  closeProfessionFileInput() {
    this.uploadProfessionExcel=false;
  }
  
  closeProspectiveFileInput() {
    this.uploadExcel=false;
  }

  closeNaukriProspectiveFileInput() {
    this.uploadNaukriExcel=false;
  }


  //customer
  
  toggleCustomerExcel() {
    this.uploadCustomerExcel = !this.uploadCustomerExcel;
    this.uploadExcel = false;
    this.uploadNaukriExcel=false;
    this.uploadCandidateExcel=false;
    this.uploadEmployeeExcel=false;
    this.uploadOrderExcel=false;
  }

  exportCustomerFile() {
    this.accountService.copyCustomerXLSFileToDB(this.formData).subscribe({
      next: (response: string) => {
        if(response === '') {
          this.toastr.success(response + ' Customer file(s) copied to database', 'success');
          this.uploadCustomerExcel=false;
          } else {
            this.toastr.warning(response, 'Failed to copy the excel data to database')
          }
      }
      , error: (err: any) => {
        console.log(err.error.details, 'Error encountered');
        this.toastr.error(err.error.details, 'Error in copying the excel data to database');
      }
    })
  }

  closeCustomerFileInput() {
    this.uploadCustomerExcel=false;
  }

  onCustomerFileInputChange(event: Event) {
    this.formData = new FormData();
    
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];

    if(f.size > 0)  this.formData.append('file', f, f.name);
        
  }

  //candidate
  toggleCandidatesExcel() {
    this.uploadCandidateExcel = !this.uploadCandidateExcel;
    this.uploadExcel=false;
    this.uploadNaukriExcel=false;
    this.uploadCustomerExcel=false;
    this.uploadEmployeeExcel=false;
    this.uploadOrderExcel=false;

  }

  exportProfessionFile() {
    this.catService.copyProfessionXLSFileToDB(this.formData).subscribe({
      next: (response: string) => {
        if(response === '') {
          this.toastr.success(response + ' file(s) copied to database', 'success');
          this.uploadCandidateExcel=false;
          } else {
            this.toastr.warning(response, 'Failed to copy the excel data to database')
          }
      }
      , error: (err: any) => {
        console.log(err.error.details, 'Error encountered');
        this.toastr.error(err.error.details, 'Error in copying the excel data to database');
      }
    })
  }
  
  exportCandidateFile() {
    this.accountService.copyCandidateXLSFileToDB(this.formData).subscribe({
      next: (response: string) => {
        if(response === '') {
          this.toastr.success(response + ' file(s) copied to database', 'success');
          this.uploadCandidateExcel=false;
          } else {
            this.toastr.warning(response, 'Failed to copy the excel data to database')
          }
      }
      , error: (err: any) => {
        console.log(err.error.details, 'Error encountered');
        this.toastr.error(err.error.details, 'Error in copying the excel data to database');
      }
    })
  }

  closeCandidateFileInput() {
    this.uploadCandidateExcel=false;
  }

  onCandidateFileInputChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];

    if(f.size > 0)  this.formData.append('file', f, f.name);
        
  }

  //employee

  toggleEmployeesExcel() {
    this.uploadEmployeeExcel = !this.uploadEmployeeExcel;
    this.uploadExcel=false;
    this.uploadNaukriExcel=false;
    this.uploadCustomerExcel=false;
    this.uploadCandidateExcel=false;
    this.uploadOrderExcel=false;
  }

  exportEmployeeFile() {
    
    this.accountService.copyEmployeeXLSFileToDB(this.formData).subscribe({
      next: (response: string) => {
        if(response === '' || response === null) {
          this.toastr.success(response + ' file(s) copied to database', 'success');
          this.uploadEmployeeExcel=false;
          } else {
            this.toastr.warning(response, 'Failed to copy the excel data to database')
          }
      }
      , error: (err: any) => {
        console.log(err.error.details, 'Error encountered');
        this.toastr.error(err.error.details, 'Error in copying the excel data to database');
      }
    })
  }

  closeEmployeeFileInput() {
    this.uploadEmployeeExcel=false;
  }

  onEmployeeFileInputChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];

    if(f.size > 0)  this.formData.append('file', f, f.name);
        
  }

  //order
  toggleOrdersExcel() {
    this.uploadOrderExcel = !this.uploadOrderExcel;
    this.uploadExcel=false;
    this.uploadNaukriExcel=false;
    this.uploadCustomerExcel=false;
    this.uploadCandidateExcel=false;
    this.uploadEmployeeExcel=false;
  }

  exportOrderFile() {
    this.accountService.copyOrderXLSFileToDB(this.formData).subscribe({
      next: (response: string) => {
        if(isNaN(Number(response))) {
          this.toastr.success(response + ' records copied to database', 'success');
          this.uploadEmployeeExcel=false;
          } else {
            this.toastr.warning(response, 'Failed to copy the excel data to database')
          }
      }
      , error: (err: any) => {
        console.log(err.error.details, 'Error encountered');
        this.toastr.error(err.error.details, 'Error in copying the excel data to database');
      }
    })
  }

  closeOrderFileInput() {
    this.uploadOrderExcel=false;
  }

  onOrderFileInputChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];

    this.formData = new FormData();

    if(f.size > 0)  this.formData.append('file', f, f.name);
        
  }


}
