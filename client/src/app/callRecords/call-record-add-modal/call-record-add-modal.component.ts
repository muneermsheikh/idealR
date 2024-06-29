import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap } from 'rxjs';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { IContactResult } from 'src/app/_models/admin/contactResult';
import { CallRecordsService } from 'src/app/_services/call-records.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-call-record-add-modal',
  templateUrl: './call-record-add-modal.component.html',
  styleUrls: ['./call-record-add-modal.component.css']
})
export class CallRecordAddModalComponent implements OnInit {

  @Output() callRecordAddEvent = new EventEmitter<ICallRecord>();
  
  callRecord: ICallRecord| undefined;
  contactResult: string='';

  dateRegistered=new Date();
  contactResults: IContactResult[]=[];

  bsValueDate=new Date();
  bsValue = new Date();

  constructor(public bsModalRef: BsModalRef, private confirm: ConfirmService, 
    private service: CallRecordsService, private toastr: ToastrService){}

  ngOnInit(): void {
     if(this.callRecord) this.callRecord.callRecordItems[0].dateOfContact = new Date();
  }

  updateClicked() {

    var confirmMsg = 'confirm update this Call Record. ';

    const observableInner = this.service.updateCallRecord(this.callRecord);
    const observableOuter = this.confirm.confirm('confirm Update', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap((confirmed) => {
          return observableInner
        })
        , catchError (err => {
          this.toastr.error(err, 'Error in updating the record');
          return of();
        })
    ).subscribe(response => {
      if(response) {
        this.callRecordAddEvent.emit(response);
        this.bsModalRef.hide();
      } else {
        this.toastr.warning('Failed to update the Call Record')
      }
      
    });

  }

  
}



