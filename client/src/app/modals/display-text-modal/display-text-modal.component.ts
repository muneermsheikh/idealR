import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-display-text-modal',
  templateUrl: './display-text-modal.component.html',
  styleUrls: ['./display-text-modal.component.css']
})
export class DisplayTextModalComponent implements OnInit{

 
  @Output() textChangedEvent = new EventEmitter<string>();
  
  displayText = '';
  title = '';

  displayCopy = '';

  ngOnInit(): void {
    this.displayCopy = this.displayText;
  }

  
 constructor(public bsModalRef: BsModalRef) { }

 emitText() {
  if(this.displayCopy !== this.displayText) {
    this.textChangedEvent.emit(this.displayText);
  } else {
    this.textChangedEvent.emit('');
  }
  this.bsModalRef.hide();
 }
}
