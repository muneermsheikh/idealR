import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IJDDto } from 'src/app/_dtos/admin/jdDto';
import { OrderService } from 'src/app/_services/admin/order.service';

@Component({
  selector: 'app-jd-modal',
  templateUrl: './jd-modal.component.html',
  styleUrls: ['./jd-modal.component.css']
})
export class JdModalComponent implements OnInit {

  @Input() updateSelectedJD = new EventEmitter();
  //jds: any;
  title: string='';
  jd?: IJDDto;

  closeBtnName: string='';

  form: FormGroup= new FormGroup({});
    
  //jd: IJobDescription;

  constructor(private service: OrderService, public bsModalRef: BsModalRef, private fb: FormBuilder ) {
   }

  ngOnInit(): void {
    //this.createForm();
    //this.form.patchValue(this.bsModalRef.content);
  }

  confirm() {
    
    this.updateSelectedJD.emit(this.jd);

    this.bsModalRef.hide();
  }


  decline() {
    this.bsModalRef.hide();
  }
}
