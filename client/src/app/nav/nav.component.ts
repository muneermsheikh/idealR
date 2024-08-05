import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CallRecordsService } from '../_services/call-records.service';
import { filter, switchMap } from 'rxjs';
import { User } from '../_models/user';
import { CallRecordsEditModalComponent } from '../callRecords/call-records-edit-modal/call-records-edit-modal.component';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{

  model: any = {};
  stPhoneNo: string='';    //for callRecord search
  user?: User;
  uploadExcel=false;

  menuitems =  [{menuitem:'Select a menu'},{menuitem:'Candidate List'}, {menuitem: 'CVs ready for referrals'}, {menuitem:'import excel sheet into Prospective candidates'}];
  menuItemSelected='';

  constructor(public accountService: AccountService, private router: Router, private service: CallRecordsService,
      private toastrService: ToastrService, private toastr: ToastrService, 
      private bsModalRef: BsModalRef, private modalService: BsModalService ){}
  
  ngOnInit(): void {
    var usr = this.accountService.currentUser$.subscribe({
      next: response => this.user=response!
    })
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log('logged in user:', response);
        this.toastrService.success('logged in successfully');
        this.model = {};
      }
    })

  }

  displayCallRecord(){
    if(this.stPhoneNo=='') {
      return;
    }
    this.service.getCallRecordFromPhoneNo(this.stPhoneNo).subscribe({
      next: callrecord => {
        if(callrecord === null) {
          this.toastr.warning('Phone No. ' + this.stPhoneNo + ' is not on record');
        } else {

        }
      }
    })

  }
  

  editCallRecord() {
    
    if(this.stPhoneNo === '') return;

    const observableOuter =  this.service.getCallRecordFromPhoneNo(this.stPhoneNo);
    
    observableOuter.pipe(
      filter((response) => response !==null),
      switchMap((response) => {
        console.log('nav.ts, switchMap', response);
        const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
            callRecord: response,
            status: "",
            candidateName: "",
            categoryRef: ""
          }
        }
        
        this.bsModalRef = this.modalService.show(CallRecordsEditModalComponent, config);
        const observableInner = this.bsModalRef.content.updateCallRecordEvent;
        return observableInner
      })
    ).subscribe((succeeded) => {
        if(succeeded) {
          this.toastr.success('Call Record was updated', 'success')
        } else {
          this.toastr.warning('failed to update the call Record', 'Failed to update')
        }
    })
        
   }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  OpenAvailableCandidatesComponent() {
    this.navigateByRoute(0, "/candidates/availableToRef", false);
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

