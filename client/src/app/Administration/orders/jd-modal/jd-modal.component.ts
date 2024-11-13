import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IJDDto } from 'src/app/_dtos/admin/jdDto';
import { IJobDescription } from 'src/app/_models/admin/jobDescription';
import { OrderService } from 'src/app/_services/admin/order.service';

@Component({
  selector: 'app-jd-modal',
  templateUrl: './jd-modal.component.html',
  styleUrls: ['./jd-modal.component.css']
})
export class JdModalComponent implements OnInit {

  @Output() updateSelectedJD = new EventEmitter<boolean>();
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

    if(this.jd) {
      var jd: IJobDescription = {id: this.jd.id, orderItemId: this.jd.orderItemId, 
        orderNo: this.jd.orderNo, jobDescInBrief: this.jd.jobDescInBrief, 
        qualificationDesired: this.jd.qualificationDesired,
        minAge: this.jd.minAge, maxAge: this.jd.maxAge, expDesiredMax: this.jd.expDesiredMax,
        expDesiredMin: this.jd.expDesiredMin};
      
      this.service.updateJD(jd).subscribe({
        next: (response: boolean) => {
          this.updateSelectedJD.emit(response);
          if(response) {
            this.bsModalRef.hide()
          }
        }, error: (err: any) => this.updateSelectedJD.emit(false)
      })
    }
  }


  decline() {
    this.bsModalRef.hide();
  }
}
