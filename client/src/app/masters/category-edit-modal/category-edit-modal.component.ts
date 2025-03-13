import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalService } from 'ngx-bootstrap/modal';
import { IProfession } from 'src/app/_models/masters/profession';
import { categoryRefParam } from 'src/app/_models/params/Admin/categoryRefParam';

@Component({
  selector: 'app-category-edit-modal',
  templateUrl: './category-edit-modal.component.html',
  styleUrls: ['./category-edit-modal.component.css']
})
export class CategoryEditModalComponent implements OnInit{

  @Input() Category: IProfession | undefined;  
  title: string='';

  @Output() updateEvent = new EventEmitter<IProfession>();
 
  constructor (public bsModalService: BsModalService){};

  ngOnInit(): void {
    console.log('category:', this.Category);
  }
  UpdateClicked() {
    if(this.Category?.professionName !== '') {
      this.updateEvent.emit(this.Category);
      this.bsModalService.hide();
    }
  }
}
