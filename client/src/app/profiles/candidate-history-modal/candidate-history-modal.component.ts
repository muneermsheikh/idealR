import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { ICandidateDto } from 'src/app/_dtos/candidateDto';
import { CandidateService } from 'src/app/_services/candidate.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-candidate-history-modal',
  templateUrl: './candidate-history-modal.component.html',
  styleUrls: ['./candidate-history-modal.component.css']
})
export class CandidateHistoryModalComponent implements OnInit{

  @Input() CandidateId: number = 0;
  @Output() Displayed = new EventEmitter<boolean>();
  
  candidateDto: ICandidateDto | undefined;

  constructor(private bsModalRef: BsModalRef, private toastr: ToastrService,
    private confirm: ConfirmService, private service: CandidateService,
    private bsModalService: BsModalService ) {}

  ngOnInit(): void {
    this.service.candidateHistory(this.CandidateId).subscribe({
      next: (response: ICandidateDto) => {
        this.candidateDto = response
      },
      error: (err: any) => this.toastr.error(err.error?.details, 'Error encountered')
    })
  }

  close() {
    this.Displayed.emit(false);
    this.bsModalRef.hide();
  }

  closeOk() {
    this.Displayed.emit(true);
    this.bsModalRef.hide();
  }

}
