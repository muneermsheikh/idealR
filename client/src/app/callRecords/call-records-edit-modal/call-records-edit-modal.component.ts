import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { ICallRecordResult } from 'src/app/_dtos/admin/callRecordResult';
import { CallRecordStatusReturnDto } from 'src/app/_dtos/admin/callRecordStatusReturnDto';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
import { User } from 'src/app/_models/user';
import { CallRecordsService } from 'src/app/_services/call-records.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-call-records-edit-modal',
  templateUrl: './call-records-edit-modal.component.html',
  styleUrls: ['./call-records-edit-modal.component.css']
})
export class CallRecordsEditModalComponent implements OnInit {

  @Output() passCallRecordEvent = new EventEmitter<ICallRecord | null>();
  
  callRecord: ICallRecord | undefined;
  candidatName='';
  
  bsValueDate = new Date();
  bsValue = new Date();
  user?: User;
  
  form: FormGroup = new FormGroup({});

  contactResults: ICallRecordResult[] = [];
  nextAction: ICallRecordResult[]=[{status:"Call again"}, {"status":"correct phone no"}, {status: "supervisor to advise"}, {status: "conclude"}];
  advisoriesBy: ICallRecordResult[]=[{status: "Mail"}, {status: "SMS"}, {status: "Both"}];
  inOutList: ICallRecordResult[]=[{status: "In"}, {status: "Out"}];

  constructor(private bsModalRef: BsModalRef, private toastr: ToastrService,
    private fb: FormBuilder, private confirm: ConfirmService, private service: CallRecordsService){
      this.contactResults = this.service.getCallRecordStatus()
    }

  ngOnInit(): void {
    if(this.callRecord) this.InitializeForm(this.callRecord);
  }

  InitializeForm(call: ICallRecord) {

    this.form = this.fb.group({
      id: call.id,  categoryRef: call.categoryRef, personId: call.personId, personName: call.personName,
      personType: call.personType, username: call.username, status: call.status, statusDate: call.statusDate, email: call.Email, phoneNo: call.phoneNo,
      subject: call.subject, concludedOn: call.concludedOn,
      
      callRecordItems: this.fb.array(
        call.callRecordItems.map(x => (
          this.fb.group({
            id: x.id, callRecordId: x.callRecordId, incomingOutgoing: x.incomingOutgoing, 
            dateOfContact: x.dateOfContact, username: x.username, phoneNo: x.phoneNo,
            contactResult: x.contactResult, gistOfDiscussions: x.gistOfDiscussions,
            nextAction: x.nextAction, nextActionOn: x.nextActionOn, email: x.email,
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
    var lastItem = this.callRecordItems.at(this.callRecordItems.length-1);
    return this.fb.group({
      id: 0, callRecordId: this.callRecord?.id, incomingOutgoing: "out",
      dateOfContact: new Date(), username: this.user?.userName ?? "", 
      contactResult: "", gistOfDiscussions: "", advoryBy: "mail",
      nextAction: "", nextActionOn: new Date(), phoneNo: lastItem.get("phoneNo")?.value,
      email: lastItem.get("email")?.value, 
    })
  }

  addCallRecordItem() {
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
        this.passCallRecordEvent.emit(formdata);
        this.bsModalRef.hide();
    }
  
  close() {
    this.passCallRecordEvent.emit(null);
    this.bsModalRef.hide();
  }
}
