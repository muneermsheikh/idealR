import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Observable, Subscription } from 'rxjs';
import { ICandidateBriefDto } from 'src/app/_dtos/admin/candidateBriefDto';
import { IOrderItemBriefDto } from 'src/app/_dtos/admin/orderItemBriefDto';
import { IChecklistHRDto } from 'src/app/_dtos/hr/checklistHRDto';
import { ICandidateAssessment } from 'src/app/_models/hr/candidateAssessment';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CVAssessService } from 'src/app/_services/cvassess.service';

@Component({
  selector: 'app-candidate-assessment',
  templateUrl: './candidate-assessment.component.html',
  styleUrls: ['./candidate-assessment.component.css']
})
export class CandidateAssessmentComponent implements OnInit {

  @Output() updateAssessment = new EventEmitter();      //outputs form value for update in the parent
  
  @Input() cvAssessment: ICandidateAssessment | undefined;
  @Input() cvBrief: ICandidateBriefDto | undefined;
  @Input() requireInternalReview: boolean = false;
  @Input() orderItemSelected: IOrderItemBriefDto | undefined;
  

  @Input() checklist: IChecklistHRDto|undefined;
  
  //parent to child 
  @Input() events?: Observable<ICandidateAssessment | undefined> ;      //emitted by parent whenever the value cvAssessment (of type ICandidateAssessment) changes
  
  private eventsSubscription: Subscription = new Subscription;

  totalPoints: number=0;
  totalGained: number=0;
  percentage: number=0;
  qDesigned: boolean = false;
  
  validationErrors: string[]=[];

  form: FormGroup = new FormGroup({});

  assessmentResults=[{"grade": "Excellent"},{"grade": "Very Good"}, {"grade": "Good"}, {"grade": "Poor"} ];

  constructor(private fb: FormBuilder, private service: CVAssessService
      , private toastr: ToastrService
      , private confirmService: ConfirmService) { }

  ngOnInit(): void {
    //receive emtited data from parent whenever the value CVAssessment changes
    
    if(this.events && this.cvAssessment) 
      {
        this.eventsSubscription = this.events!.subscribe(data => {
          this.candidateAssessmentItems.clear();
          this.cvAssessment = data;
          this.initializeAndPopulateFormArray(this.cvAssessment!);
        })
        this.calculatePercentage();
      }
  
      this.qDesigned = this.orderItemSelected!.assessmentQDesigned;
  }

 
  addCandidateAssessmentItem() {
    this.qDesigned==true;
    this.toastr.info('qDesigned set to true');
    this.candidateAssessmentItems.push(this.newCandidateAssessmentItem());
  }

  
  newCandidateAssessmentItem(): FormGroup {
    return this.fb.group({
      id: 0, candidateAssessmentId: 0, questionNo: 0,
      question: ['', Validators.required ], 
      assessed: false, 
      isMandatory: false, 
      maxPoints: 0, 
      points: [0, [Validators.min(0),   this.matchValues('maxPoints')]],
      remarks: ''
    })
  }
  
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  removeCandidateAssessmentItem(i: number) {
    this.candidateAssessmentItems.removeAt(i);
    this.candidateAssessmentItems.markAsDirty();
    this.candidateAssessmentItems.markAsTouched();
  }

  deleteAssessment() {
    this.service.deleteAssessment(this.cvAssessment!.id).subscribe(response => {
        if (response) {
          this.toastr.success('Candidate Assessment object deleted');
          this.cvAssessment=undefined;
          this.initializeTotals();
        } else {
          this.toastr.warning('failed to delete the assessment object');
        }
    }, error => {
      this.toastr.error('error in deleting the object', error);
    });
  }
  
  initializeTotals() {
    this.totalGained =0;
    this.totalPoints=0;
    this.percentage=0;

  }

  initializeAndPopulateFormArray(assess: ICandidateAssessment) {
      this.form = this.fb.group({
        id:0, orderItemId: 0, candidateId: 0, assessedOn: '', 
        assessResult: '', remarks: '',

        candidateAssessmentItems: this.fb.array(
          assess.candidateAssessmentItems.map(x => (
            this.fb.group({
              id: x.id, 
              candidateAssessmentId: [x.candidateAssessmentId, Validators.required],
              questionNo: [x.questionNo, Validators.required], 
              question: [x.question, Validators.required],
              maxPoints: [x.maxPoints, Validators.required], 
              points: x.points,
              remarks: x.remarks
            })
          ))
        )
    })
  }
  
  createForm() {
    this.form = this.fb.group({
      id:0, orderItemId: 0, candidateId: 0, assessedOn: '', assessResult: '', remarks: '',
      candidateAssessmentItems: this.fb.array([])
    })
  }

  
  get candidateAssessmentItems(): FormArray {
    return this.form.get('candidateAssessmentItems') as FormArray;
  }

  
  maxMarksTotal() {

    this.totalPoints =  this.candidateAssessmentItems.value.map((x:any) => x.maxPoints).reduce((a:number, b: number) => a + b,0);
    //this.totalPoints =  this.candidateAssessmentItems.value.filter(x => x.assessed===true). map(x => x.maxPoints).reduce((a, b) => a + b, 0);
    this.calculatePercentage();
  }

  pointsGainedTotal(i: number){
    console.log('item:', this.candidateAssessmentItems.value[i]);
    var pt = this.candidateAssessmentItems.value[i].points;
    var mx = this.candidateAssessmentItems.value[i].maxPoints;

    if( pt > mx ) {
      this.toastr.warning("Max points for this parameter is:" + mx + ".  You cannot exceed this number");
      this.candidateAssessmentItems.at(i).get('points')?.setValue(0);
      this.candidateAssessmentItems.at(i).get('assessed')?.setValue(false);
      this.calculatePercentage();
      return;
    }
    
    this.totalGained = this.candidateAssessmentItems.value.map((x: any) => x.points).reduce((a:number,b:number) => a+b,0);
    this.calculatePercentage();
    this.candidateAssessmentItems.at(i).get('assessed')?.setValue(true);
    //(<FormArray>this.form.controls['candidateAssessmentItems']).at(i).get("assessed").setValue(true);   //set value of the DOM assessed to true
  }

  calculatePercentage() {
    this.maxMarksTotal();
    this.totalGained = this.candidateAssessmentItems.value.map((x:any) => x.points).reduce((a:number,b:number) => a+b,0);

    this.percentage = (this.totalGained===undefined || this.totalPoints === undefined) ? 0: Math.round(100*this.totalGained / this.totalPoints);
  }

  //modals
  openChecklistModal() {

  }
  
  calculateGrade() {
    this.calculatePercentage();
    var grade="";
    if(this.percentage < 41) {
      grade = "Poor";
    } else if (this.percentage > 40 && this.percentage <= 60) {
      grade = "Average";
    } else if (this.percentage > 60 && this.percentage <= 70) {
      grade = "Good";
    } else if (this.percentage < 70 && this.percentage <= 80) {
      grade = "Very Good";
    } else {
      grade = "Excellent";
    }

    return grade;
  }


  update() {  
    if(this.form.dirty) {
      this.cvAssessment!.assessResult = this.calculateGrade();
      this.updateAssessment.emit(this.form.value);
    }
  }

  ngOnDestroy() {
    this.eventsSubscription.unsubscribe();
  }

  closeAssessment() {
    this.updateAssessment
  }
}
