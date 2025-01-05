import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICheckedDto } from 'src/app/_dtos/hr/checkedDto';
import { IInterviewMatchingCategoryDto } from 'src/app/_dtos/hr/interviewMatchingCategoryDto';
import { IIntervwItemCandidate } from 'src/app/_models/hr/intervwItemCandidate';
import { User } from 'src/app/_models/user';
import { CandidateService } from 'src/app/_services/candidate.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { InterviewService } from 'src/app/_services/hr/interview.service';

@Component({
  selector: 'app-interview-brief-pending-modal',
  templateUrl: './interview-brief-pending-modal.component.html',
  styleUrls: ['./interview-brief-pending-modal.component.css']
})
export class InterviewBriefPendingModalComponent implements OnInit{
 
  interviews: IInterviewMatchingCategoryDto[]=[];

  categoryMatching: string[] = [];
  candidateId: number=0;
  candidateName: string = '';
  applicationNo=0;
  interviewDate: Date=new Date();

  @Output() EmitEvent = new EventEmitter<boolean>();

  user?: User;

  selected: ICheckedDto[] = [];

  orderNoToGet = 0;

  totalCount: number=0;

  bsModalRef: BsModalRef | undefined;
  
  constructor(private service: InterviewService,
    private toastr: ToastrService, private modalService: BsModalService, 
    private confirm: ConfirmService, private candService: CandidateService) {}

  ngOnInit(): void {  
   
    this.service.getInterviewItemsMatchingCategory(this.categoryMatching).subscribe({
      next: (response: IInterviewMatchingCategoryDto[]) => {
        if(response !==null && response.length > 0) {
          this.interviews = response;
        }
      }
    })
  }

  //interviews array will have only one row with selected as true
  selectedClicked(id: number, chk: boolean) {
    var index = this.selected.findIndex(x => x.id === id);

    if(index === -1) {
      var sel: ICheckedDto = {id:id, checked: chk};
      this.selected.push (sel);
    } else {
      this.selected[index].checked = chk;
    }
    if(chk) this.selected.filter(x => x.id !==id)[0].checked=false;
    this.interviews.find(x => x.id == id)!.checked = chk;
    this.interviews.find(x => x.id === id)!.checked = chk;
  }

  close() {
    this.EmitEvent.emit(false);
    this.modalService.hide();
  }
  
  assignToInterviewItem() {

    var intervwItemId = this.interviews.filter(x => x.checked)[0].id;

    var interviewCandidate: IIntervwItemCandidate = {
          id: 0,
          interviewItemId: intervwItemId,
          intervwItemId: intervwItemId,
          candidateId: this.candidateId,
          applicationNo: this.applicationNo,
          candidateName: this.candidateName,
          passportNo: '',
          scheduledFrom: this.interviewDate,
          reportedAt: undefined,
          interviewedAt: undefined,
          interviewStatus: 'Not Interviewed',
          interviewerRemarks: '',
          prospectiveCandidateId: 0,
          personId :'',
          attachmentFileNameWithPath: ''
     }

    this.service.insertInterviewItemCandidate(interviewCandidate).subscribe({
      next: (response: boolean) => this.EmitEvent.emit(response)
    })

    this.modalService.hide();
  }
}
