import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ICallRecordDto } from 'src/app/_dtos/admin/callRecordDto';
import { ICallRecordItemAddedReturnValueDto } from 'src/app/_dtos/admin/callRecordItemAddedReturnValueDto';
import { ICallRecordItemToAddDto } from 'src/app/_dtos/admin/callRecordItemToAddDto';
import { ICallRecordItem } from 'src/app/_models/admin/callRecordItem';
import { CallRecordsService } from 'src/app/_services/call-records.service';

@Component({
  selector: 'app-register-call-records',
  templateUrl: './register-call-records.component.html',
  styleUrls: ['./register-call-records.component.css']
})
export class RegisterCallRecordsComponent {

    @Input() id: number=0;  //prospective.id or candidate.id
    @Input() status: string = '';
    @Input() categoryRef: string = '';
    @Input() personId: string = '';
    @Input() personType: string = '';
    @Output() callStatusEvent = new EventEmitter<string>();     //to change status in calling program
    @Output() eventClickedEvent = new EventEmitter<boolean>();  //to enable (send Mail/SMS) options in calling program
    
    eventClicked:boolean=false;

    statusDate: Date = new Date();

    Interested=false;
    NotInterested=false;
    CallMissed=false;
    adviseByMail=false;
    adviseBySMS=false;
  

  constructor(private callRecService: CallRecordsService, private toastr: ToastrService){}
  
  InterestedClicked(id: number) {
    this.Interested = true;
    this.NotInterested=false;
    this.CallMissed=false;
    this.eventClicked = !this.eventClicked;
    this.eventClickedEvent.emit(this.eventClicked);
  }

  NotInterestedClicked(id: number) {
    this.Interested = false;
    this.NotInterested = true;
    this.CallMissed = false;
    this.eventClicked = !this.eventClicked;
    this.eventClickedEvent.emit(this.eventClicked);
  }

  CallMissedClicked(id: number) {
    this.NotInterested = false;
    this.CallMissed = true;
    this.Interested = false;
    this.eventClicked = !this.eventClicked;
    this.eventClickedEvent.emit(this.eventClicked);
  }

  Interested_keen(id:number) {
      this.editOrAddCallRecordItem("Interested, and keen", id);
      this.Interested=false; //hide the options
  }

  Interested_doubtful(id: number) {
    this.editOrAddCallRecordItem("Interested, but doubtful", id);
    this.Interested=false; //hide the options
  } 

  Interested_undecided(id: number) {
    this.editOrAddCallRecordItem("Interested, undecided", id);
    this.Interested=false; //hide the options
  }

  NI_DueToSalary(id: number) {
      this.editOrAddCallRecordItem("Declined-Low remuneration", id);
      this.NotInterested=false;
  }

  NI_ForOverseas(id: number) {
    this.editOrAddCallRecordItem("Declined for overseas", id)
    this.NotInterested = false;
  }

  NI_DueToSC (id: number) {
    this.editOrAddCallRecordItem("Declined - SC Not agreed", id);
    this.NotInterested=false;
  }

  NI_DueToUnknownReasons(id: number) {
    this.editOrAddCallRecordItem("Declined - Other Reasons", id);
    this.NotInterested=false;
  }

  CallMissed_WrongNo(id: number) {
    this.editOrAddCallRecordItem("wrong number", id);
    this.CallMissed=false;
  }

  CallMissed_NoResponse(id: number) {
    this.editOrAddCallRecordItem("Not Responding", id);
    this.CallMissed = false;
  }

  CallMissed_CallLater(id: number) {
    this.editOrAddCallRecordItem("Call Later", id);
    this.CallMissed = false;
  }

  editOrAddCallRecordItem(callRecordStatus:  string, prospectiveId: number) {

    if(this.status === callRecordStatus) {
      this.toastr.warning("Status is ALREADY " + callRecordStatus, "Status need not be changed");
      return;
    }
    
    var callrecordItem: ICallRecordItem = {id:0, callRecordId:0, incomingOutgoing:"Out", phoneNo:"", 
      email: "", dateOfContact: new Date(), username: "", contactResult: callRecordStatus, 
      gistOfDiscussions: "Clicked by user", nextAction: "", nextActionOn: new Date, advisoryBy: ""};

    var callrecordDto: ICallRecordItemToAddDto = {personId: this.personId.toString(),  personType: this.personType,
        categoryRef: this.categoryRef, adviseByMail: this.adviseByMail, adviseBySMS:this.adviseBySMS, 
        callRecordItem: callrecordItem};
      console.log('callRecordDto', callrecordDto);

    this.callRecService.updateCallRecordItem(callrecordDto).subscribe({
      next: (response: ICallRecordItemAddedReturnValueDto) => {
        console.log('response:', response);
        if(response !== null && response.errorString !== '') {
            this.status = response.contactResult;
            this.callStatusEvent.emit(this.status);
            this.statusDate = response.dateOfContact;
            var msgComposed = response.messageComposed ? "A message to the candidate has been composed and available " +
              "to view/edit in Messages Section (under type 'CandidateInterest')" : "No message was composed.";

            this.toastr.success("Call record item registered as -"+ callRecordStatus + ". " + msgComposed, "Success");
        }
      }, error: (err: any) => this.toastr.error(err.error?.details, "Error encountered")
    })
    
  }

}
