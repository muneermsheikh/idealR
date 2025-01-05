import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ISelPendingDto } from 'src/app/_dtos/admin/selPendingDto';

@Component({
  selector: 'app-selection-pending-line',
  templateUrl: './selection-pending-line.component.html',
  styleUrls: ['./selection-pending-line.component.css']
})
export class SelectionPendingLineComponent implements OnInit {

  @Input() pending: ISelPendingDto | undefined;

  @Output() selectedEvent = new EventEmitter<ISelPendingDto>;

  ngOnInit(): void {
    
  }

  SelectedClicked() {
    this.selectedEvent.emit(this.pending);
  }
}
