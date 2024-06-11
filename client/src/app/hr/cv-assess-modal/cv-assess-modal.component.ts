import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ICandidateAssessmentDto } from 'src/app/_dtos/hr/candidateAssessmentDto';
import { CandidateAssessmentService } from 'src/app/_services/hr/candidate-assessment.service';

@Component({
  selector: 'app-cv-assess-modal',
  templateUrl: './cv-assess-modal.component.html',
  styleUrls: ['./cv-assess-modal.component.css']
})
export class CvAssessModalComponent {

  @Output() candAssessEvent = new EventEmitter<ICandidateAssessmentDto>();

  assess: ICandidateAssessmentDto | undefined;
  totalPoints=0;
  totalMaxPoints=0;
  
  form: FormGroup = new FormGroup({});  

  constructor(public bsModalRef: BsModalRef, private assessService: CandidateAssessmentService,
    private activatedRoute: ActivatedRoute,
    private fb: FormBuilder, private router:Router) {}

  ngOnInit(): void {
      if(this.assess !== undefined) {
        this.InitializeFormArray(this.assess);
        this.calculateTotalPoints();
      }
  }

  InitializeFormArray(assessmt: ICandidateAssessmentDto) {

    this.form = this.fb.group({
      applicationNo: [assessmt.applicationNo, Validators.required],
      candidateId: [assessmt.candidateId, Validators.required],
      candidateName: [assessmt.candidateName, Validators.required],

      customerName: [assessmt.customerName, Validators.required],
      orderItemId: [assessmt.orderItemId, Validators.required],
      categoryRef: [assessmt.categoryRef, Validators.required],
      orderId: [assessmt.orderId, Validators.required],
      requireInternalReview: assessmt.requireInternalReview,
      assessedByUsername: assessmt.assessedByUsername,

      professionName: [assessmt.professionName, Validators.required],
      assessResult: [assessmt.assessResult, Validators.required],
      assessedOn: assessmt.assessedOn,

      checklistHRId: assessmt.checklistHRId,
      checklistedByName: assessmt.checklistedByName,
      

      items: this.fb.array(
        assessmt.assessmentItemsDto.map(item => (
          this.fb.group({
            id: item.id,
            candidateAssessmentId: item.candidateAssessmentId,
            questionNo: [item.questionNo, Validators.required],
            question: [item.question, Validators.required],
            assessed: [item.assessed],
            maxPoints: [item.maxPoints, Validators.required],
            points: [item.points],
            remarks: item.remarks
          })
        ))
      )

      
    })
  }

 get items() : FormArray {
    return this.form.get("items") as FormArray
 }


 close() {
  this.candAssessEvent.emit(this.assess);
  this.bsModalRef.hide();
 }

 calculateTotalPoints() {
  this.totalMaxPoints =  this.items.value.map((x:any) => x.maxPoints).reduce((a:number, b: number) => a + b,0);
  this.totalPoints =  this.items.value.map((x:any) => x.points).reduce((a:number, b: number) => a + b,0);
 }
 
}
