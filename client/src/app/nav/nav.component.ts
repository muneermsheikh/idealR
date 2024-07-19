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

}

