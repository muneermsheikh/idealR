import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { ISelDecisionDto } from 'src/app/_dtos/admin/selDecisionDto';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-selection-line',
  templateUrl: './selection-line.component.html',
  styleUrls: ['./selection-line.component.css']
})
export class SelectionLineComponent {

  @Input() selection: ISelDecisionDto | undefined;
  @Output() displayEmploymentEvent = new EventEmitter<number>();
  @Output() deleteSelectionEvent = new EventEmitter<number>();
  @Output() editSelectionEvent = new EventEmitter<ISelDecisionDto>();
  @Output() remindCandidateForDecisionEvent = new EventEmitter<number>();

  displayEmployment() {
    if(this.selection) this.displayEmploymentEvent.emit(this.selection.selDecisionId);
  }

  editSelection() {
    this.editSelectionEvent.emit(this.selection);
  }

  deleteSelection() {
    if(this.selection) this.deleteSelectionEvent.emit(this.selection.selDecisionId);
  }

  remindCandidateForDecision() {
    if(this.selection) this.remindCandidateForDecisionEvent.emit(this.selection.selDecisionId);
  }




}
