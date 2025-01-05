import { Component, EventEmitter, Output } from '@angular/core';
import { BsModalService } from 'ngx-bootstrap/modal';
import { IIndustryType } from 'src/app/_models/admin/industryType';

@Component({
  selector: 'app-industry-edit-modal',
  templateUrl: './industry-edit-modal.component.html',
  styleUrls: ['./industry-edit-modal.component.css']
})
export class IndustryEditModalComponent {

  industry: IIndustryType | undefined;
  title: string='';

  @Output() updateEvent = new EventEmitter<IIndustryType>();
 
  constructor (public bsModalService: BsModalService){};

  UpdateClicked() {
    if(this.industry?.industryGroup !== '') {
      this.updateEvent.emit(this.industry);
      this.bsModalService.hide();
    }
  }

}
