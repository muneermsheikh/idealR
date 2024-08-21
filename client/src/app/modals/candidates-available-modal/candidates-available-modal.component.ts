import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CvsMatchingProfAvailableDto } from 'src/app/_dtos/hr/cvsMatchingProfAvailableDto';
import { InterviewService } from 'src/app/_services/hr/interview.service';

@Component({
  selector: 'app-candidates-available-modal',
  templateUrl: './candidates-available-modal.component.html',
  styleUrls: ['./candidates-available-modal.component.css']
})
export class CandidatesAvailableModalComponent implements OnInit{

  @Output() emittedEvent = new EventEmitter<CvsMatchingProfAvailableDto[]>();
  
  professionid: number = 0;

  cvs: CvsMatchingProfAvailableDto[]=[];
  cvsSelected: CvsMatchingProfAvailableDto[]=[];

  constructor(private service: InterviewService, public bsModalRef: BsModalRef){}

  ngOnInit(): void {
    
    this.service.getMatchingCandidates(this.professionid).subscribe({
      next: response => this.cvs=response
    })

  }

  SelectCVs(cv: CvsMatchingProfAvailableDto) {
 
    if(!cv.checked) {
      this.cvsSelected.push(cv);
    } else {
      var index = this.cvsSelected.findIndex(x => x.candidateId===cv.candidateId);
      if(index !==-1) this.cvsSelected.splice(index, 1);
    }
  }

  emitCVs() {
    if(this.cvsSelected.length > 0) {
      this.emittedEvent.emit(this.cvsSelected);
      this.bsModalRef.hide();
    }
  } 
}
