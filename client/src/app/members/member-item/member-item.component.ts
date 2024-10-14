import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-item',
  templateUrl: './member-item.component.html',
  styleUrls: ['./member-item.component.css']
})
export class MemberItemComponent {

  @Input() member: Member | undefined;
  @Output() editEvent = new EventEmitter<Member>();
  @Output() deleteEvent = new EventEmitter<number>();

  editClicked() {
    this.editEvent.emit(this.member);
  }

  deleteClicked() {
    this.deleteEvent.emit(this.member?.id)
  }

}
