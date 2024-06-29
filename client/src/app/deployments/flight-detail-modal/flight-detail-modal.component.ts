import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICandidateFlight } from 'src/app/_models/process/candidateFlight';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { DeployService } from 'src/app/_services/deploy.service';
import { CandidateEditComponent } from 'src/app/profiles/candidate-edit/candidate-edit.component';

@Component({
  selector: 'app-flight-detail-modal',
  templateUrl: './flight-detail-modal.component.html',
  styleUrls: ['./flight-detail-modal.component.css']
})
export class FlightDetailModalComponent implements OnInit {

  @Output() candidteFlightDeleteEvent = new EventEmitter<number>(); //candidateflight.id
  @Output() candidateFlightEditEvent = new EventEmitter<ICandidateFlight>();

  flight: ICandidateFlight|undefined;

  
  bsValue = new Date();
  bsRangeValue= new Date();
  bsValueDate = new Date();

  form: FormGroup = new FormGroup({});

  constructor(public bsModalRef: BsModalRef, private toastr:ToastrService,
      private confirm: ConfirmService, private service: DeployService,
      private fb: FormBuilder ){}
  
  ngOnInit(): void {
    console.log('candidateflight in modal:', this.flight);
    
  }

  deleteCandidateFlight() {
    var confirmed = this.confirm.confirm('confirm DELETE', 'Are you sure you want to delete this candidate Flight?')
      .subscribe(confirmed => {
        if(confirmed) {
          this.candidteFlightDeleteEvent.emit(this.flight?.id);
          this.bsModalRef.hide();
        }
      })
  }

  saveCandidateFlight() {

    console.log('flight:', this.flight);
      this.candidateFlightEditEvent.emit(this.flight);
  }
}
