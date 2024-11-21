import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICallRecordResult } from 'src/app/_dtos/admin/callRecordResult';
import { ICallRecord } from 'src/app/_models/admin/callRecord';
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
  userName: string = '';
  
  form: FormGroup = new FormGroup({});

  contactResults: ICallRecordResult[] = [];
  nextAction: ICallRecordResult[]=[{status:"Call again"}, {"status":"Correct Phone No"}, {status: "Escalate To Supervisor"}, {status: "Conclude"}];
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
      id: call.id,  categoryRef: call.categoryRef, 
      personId: [call.personId, Validators.required], personName: call.personName,
      personType: call.personType, username: [call.username ?? this.userName], 
      status: call.status, statusDate: call.statusDate, email: call.Email, phoneNo: call.phoneNo,
      subject: [call.subject, Validators.required], concludedOn: call.concludedOn,
      
      callRecordItems: this.fb.array(
        call.callRecordItems.map(x => (
          this.fb.group({
            id: x.id, callRecordId: x.callRecordId, 
            incomingOutgoing: [x.incomingOutgoing, [Validators.required, Validators.maxLength(3)]], 
            dateOfContact: [x.dateOfContact, Validators.required], 
            username: [x.username ?? this.userName], 
            phoneNo: x.phoneNo, contactResult: [x.contactResult, Validators.required], 
            gistOfDiscussions: [x.gistOfDiscussions], nextAction: x.nextAction, nextActionOn: x.nextActionOn, 
            email: x.email, advisoryBy: x.advisoryBy, composeSMS: x.advisoryBy
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
      id: 0, callRecordId: this.callRecord?.id, incomingOutgoing: ["out", Validators.required],
      dateOfContact: [new Date(), Validators.required], username: [this.userName ?? "", Validators.required], 
      contactResult: ["", Validators.required], gistOfDiscussions: "", advisoryBy: "mail",
      nextAction: "", nextActionOn: new Date(), phoneNo: lastItem.get("phoneNo")?.value,
      email: lastItem.get("email")?.value 
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
        this.form.get('username')?.setValue(this.userName);
        var formdata = this.form.value;
        this.passCallRecordEvent.emit(formdata);
        this.bsModalRef.hide();
    }
  
  contactResultChanged(event: any, index: number) {
      switch (event.status.toLowerCase()) {
        case "not responding": case "will revert later":
          this.callRecordItems.at(index).get('nextAction')?.setValue('Call Again');
          break;
        case "wrong number":
          this.callRecordItems.at(index).get('nextAction')?.setValue('Correct Phone Number');
          break;
        case "not interested": case "declined-family issues":
            case "declined for overseas": "declined-low remuneration"
          this.callRecordItems.at(index).get('nextAction')?.setValue('Concluded');
          break;
        case "declined - sc not agreed": 
          this.callRecordItems.at(index).get('nextAction')?.setValue('Escalate To Supervisor');
          break;
        default:
          this.callRecordItems.at(index).get('nextAction')?.setValue('Concluded');
          break;
      }
  }
  
  close() {
    this.passCallRecordEvent.emit(null);
    this.bsModalRef.hide();
  }
}
