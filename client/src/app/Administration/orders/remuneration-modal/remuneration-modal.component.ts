import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IRemunerationDto } from 'src/app/_dtos/admin/remunerationDto';
import { OrderService } from 'src/app/_services/admin/order.service';

@Component({
  selector: 'app-remuneration-modal',
  templateUrl: './remuneration-modal.component.html',
  styleUrls: ['./remuneration-modal.component.css']
})
export class RemunerationModalComponent implements OnInit{

  @Input() updateSelectedRemuneration = new EventEmitter();
  remun?: IRemunerationDto;  // any;
  
  closeBtnName: string='';

  form: FormGroup = new FormGroup({});
  
  constructor(private service: OrderService, public bsModalRef: BsModalRef, private fb: FormBuilder ) {
   }

  ngOnInit(): void {
    console.log('remun in modal', this.remun);
  }

 
  confirm() {
    this.updateSelectedRemuneration.emit(this.remun);
    
    this.bsModalRef.hide();
  }

  decline() {
    this.bsModalRef.hide();
  }
}
