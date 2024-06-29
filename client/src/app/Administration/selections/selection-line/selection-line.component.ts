import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ISelDecisionDto } from 'src/app/_dtos/admin/selDecisionDto';
import { IEmployment } from 'src/app/_models/admin/employment';
import { ISelectionDecision } from 'src/app/_models/admin/selectionDecision';
import { SelectionService } from 'src/app/_services/hr/selection.service';

@Component({
  selector: 'app-selection-line',
  templateUrl: './selection-line.component.html',
  styleUrls: ['./selection-line.component.css']
})
export class SelectionLineComponent {

  @Input() selection: ISelDecisionDto | undefined;
  @Output() editEmploymentEvent = new EventEmitter<IEmployment | null>();
  @Output() deleteSelectionEvent = new EventEmitter<number>();
  @Output() editSelectionEvent = new EventEmitter<ISelectionDecision>();
  @Output() remindCandidateForDecisionEvent = new EventEmitter<number>();

  constructor(private service: SelectionService){}

  displayEmployment() {
    this.service.getEmploymentFromSelectionId(this.selection!.id).subscribe({
        next: (response) => {
          if(response !==null) {
            this.editEmploymentEvent.emit(response);
          } else {
            this.editEmploymentEvent.emit(null);
          }
          
        }
    })
  }

  editSelection() {
    this.service.getSelectionBySelDecisionId(this.selection?.id).subscribe({
      next: (response) => {
        console.log('response from selection-line', response);
        this.editSelectionEvent.emit(response);
      }
    })
    
  }

  deleteSelection() {
    if(this.selection) this.deleteSelectionEvent.emit(this.selection.id);
  }

  remindCandidateForDecision() {
    if(this.selection) this.remindCandidateForDecisionEvent.emit(this.selection.id);
  }

  



}
