import { Component, EventEmitter, Input } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ICustomerOfficialDto } from 'src/app/_models/admin/customerOfficialDto';

@Component({
  selector: 'app-choose-agents-modal',
  templateUrl: './choose-agents-modal.component.html',
  styleUrls: ['./choose-agents-modal.component.css']
})
export class ChooseAgentsModalComponent {

  @Input() updateSelectedOfficialIds = new EventEmitter();
  //order: IOrderBriefDto;
  agents: ICustomerOfficialDto[]=[]; // IChooseAgentDto[]; 
  title: string='';
  constructor(public bsModalRef: BsModalRef) { }

  
  updateAgentsSelected() {
    this.updateSelectedOfficialIds.emit(this.agents);
    this.bsModalRef.hide();
  }
}
