import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-input-modal',
  templateUrl: './input-modal.component.html',
  styleUrls: ['./input-modal.component.css']
})
export class InputModalComponent {

  @Output() outputEvent = new EventEmitter<string>();
  
  inputValue: string = '';

  title: string= '';

  
  constructor(private toastr: ToastrService, public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  returnInputValue() {
    if (this.inputValue==='' ) {
        this.toastr.warning('no value entered');
        return;
    }

    this.outputEvent.emit(this.inputValue);

    this.bsModalRef.hide();
  }
  
}
