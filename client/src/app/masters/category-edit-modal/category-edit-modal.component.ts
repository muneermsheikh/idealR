import { Component, EventEmitter, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IProfession } from 'src/app/_models/masters/profession';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-category-edit-modal',
  templateUrl: './category-edit-modal.component.html',
  styleUrls: ['./category-edit-modal.component.css']
})
export class CategoryEditModalComponent {

    @Output() updateEvent = new EventEmitter<IProfession>();
    
    category: IProfession | undefined;
    title: string='';

    constructor(public bsModalRef: BsModalRef, private confirm: ConfirmService){}

    updateClicked() {
      if(this.category?.professionName !=='') {
        this.updateEvent.emit(this.category);
        this.bsModalRef.hide();
      }
    }
}
