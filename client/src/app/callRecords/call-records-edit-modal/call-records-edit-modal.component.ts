import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICallRecordResult } from 'src/app/_dtos/admin/callRecordResult';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-call-records-edit-modal',
  templateUrl: './call-records-edit-modal.component.html',
  styleUrls: ['./call-records-edit-modal.component.css']
})
export class CallRecordsEditModalComponent implements OnInit {

  @Input() updateCallRecordEvent = new EventEmitter<ICallRecord>();
  
  callRecord: ICallRecord | undefined;
  candidatName='';
  
  bsValueDate = new Date();
  bsValue = new Date();
  user?: User;
  
  form: FormGroup = new FormGroup({});

  contactResults: ICallRecordResult[] = [];

  constructor(private bsModalRef: BsModalRef, private toastr: ToastrService,
    private fb: FormBuilder, private confirm: ConfirmService){}

  ngOnInit(): void {
    if(this.callRecord) this.InitializeForm(this.callRecord);
  }

  InitializeForm(call: ICallRecord) {

    this.form = this.fb.group({
      id: call.id,  categoryRef: call.categoryRef, resumeId: call.personId,
      status: call.status, statusDate: call.statusDate, 
      subject: call.subject, concludedOn: call.concludedOn,
      
      callRecordItems: this.fb.array(
        call.callRecordItems.map(x => (
          this.fb.group({
            id: x.id, callRecordId: x.callRecordId, incomingOutgoing: x.incomingOutgoing, 
            dateOfContact: x.dateOfContact, username: x.username, phoneNo: x.phoneNo,
            contactResult: x.contactResult, gistOfDiscussions: x.gistOfDiscussions,
            nextAction: x.nextAction, nextActionOn: x.nextActionOn,
            advisoryBy: x.advisoryBy, composeSMS: x.advisoryBy
          })
        ))
      )
    })
  }

  get callRecordItems(): FormArray {
    return this.form.get("callRecordItems") as FormArray
  }

  newCallRecordItem(): FormGroup {
    return this.fb.group({
      id: 0, callRecordId: this.callRecord?.id, incomingOutgoing: "",
      dateOfContact: new Date(), username: this.user?.userName, 
      contactResult: "", gistOfDiscussions: "", composeEmailMessage: false,
      composeSMS: false, nextAction: "", nextActionOn: ""
    })
  }

  addHistoryItem() {
    this.callRecordItems.push(this.newCallRecordItem());
  }

  removeDepItem(index: number) {
    
      this.confirm.confirm("Confirm Delete", 
        "This will also delete the selected Call Record Item. If you confirm to delete, remember to save this transaction")
        .subscribe({next: confirmed => {
          if(confirmed) this.callRecordItems.removeAt(index);
      }})
  }

  updateCallRecord() {
    var formdata = this.form.value;
    this.updateCallRecordEvent.emit(this.callRecord);
    this.bsModalRef.hide();
  }

  close() {
    this.bsModalRef.hide();
  }
}
