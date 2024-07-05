import { Component, HostListener, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { Subject, catchError, of, switchMap, take, tap } from 'rxjs';
import { ICandidateBriefDto } from 'src/app/_dtos/admin/candidateBriefDto';
import { IOrderItemBriefDto } from 'src/app/_dtos/admin/orderItemBriefDto';
import { ICandidateAssessedShortDto } from 'src/app/_dtos/hr/candidateAssessedShortDto';
import { ICandidateAssessmentAndChecklistDto } from 'src/app/_dtos/hr/candidateAssessmentAndChecklistDto';
import { IChecklistHRDto } from 'src/app/_dtos/hr/checklistHRDto';
import { IHelp } from 'src/app/_models/admin/help';
import { CandidateAssessment, ICandidateAssessment } from 'src/app/_models/hr/candidateAssessment';
import { IChecklistHRItem } from 'src/app/_models/hr/checklistHRItem';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CandidateAssessmentService } from 'src/app/_services/hr/candidate-assessment.service';
import { ChecklistService } from 'src/app/_services/hr/checklist.service';
import { HelpModalComponent } from 'src/app/modals/help-modal/help-modal.component';
import { ChecklistModalComponent } from 'src/app/profiles/checklist-modal/checklist-modal.component';

@Component({
  selector: 'app-cv-assess',
  templateUrl: './cv-assess.component.html',
  styleUrls: ['./cv-assess.component.css']
})
export class CvAssessComponent implements OnInit {

  cvBrief: ICandidateBriefDto | undefined;
  openOrderItems: IOrderItemBriefDto[]=[];
  orderItemSelected: IOrderItemBriefDto | undefined;
  orderItemSelectedId: number=0;
  cvAssessment: ICandidateAssessment | undefined;   //full assessment of the candidate

  helpfile = this.getHelp("candidate assessment");
  //checklist$: Observable<IChecklistHRDto>;

  assessmentAndChecklist: ICandidateAssessmentAndChecklistDto | undefined;

  existingAssessmentsDto: ICandidateAssessedShortDto[]=[];    //summary of existing assessments of the selected candidate

//checklist
  checklist: IChecklistHRDto | undefined;
  //checklistDto: IChecklistHRDto | undefined;
  //editedChecklistDto: IChecklistHRDto | undefined;
  checklistitems: IChecklistHRItem[]=[];
  bsModalRef: BsModalRef | undefined;

  user?: User | undefined;
  totalPoints: number=0;
  totalGained: number=0;
  percentage: number=0;
  qDesigned: boolean = false;
  requireInternalReview: string="N";
  lastOrderItemIdSelected: number=-1;

  routeId: string='';
  bolNavigationExtras: boolean=false;
  returnUrl: string='';

  form: FormGroup = new FormGroup({});
  validationErrors: string[] = [];

  bDisplayHelp:boolean=false;
  displayText: string='Show Help';

  //emit to child
  orderItemChangedEventSubject: Subject<ICandidateAssessment | undefined> = new Subject<ICandidateAssessment | undefined>();

  assessmentResults=[{"grade": "Excellent"},{"grade": "Very Good"}, {"grade": "Good"}, {"grade": "Poor"} ];

  @HostListener('window:beforeunload', ['event']) unloadNotification($event: any) {
    if(this.form.dirty) {$event.returnValue=true}
  }

  constructor(private fb: FormBuilder, 
    private service: CandidateAssessmentService,
    //private assessmentService: OrderAssessmentService,
    private candidateAssessmentService: CandidateAssessmentService,
    private bsModalService: BsModalService,
    private checklistService: ChecklistService,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private accountsService: AccountService, 
    private confirmService: ConfirmService,
    private router: Router
  ) {
      this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
            
            this.routeId = this.activatedRoute.snapshot.params['id'];
            if(this.routeId==undefined) this.routeId='';

            let nav: Navigation | null  = this.router.getCurrentNavigation() ;
            
            if (nav?.extras && nav.extras.state) {
                this.bolNavigationExtras=true;
                if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

                if( nav.extras.state['cvbrief']) this.cvBrief = nav.extras.state['cvbrief'] as ICandidateBriefDto;
               }
    }

  ngOnInit(): void {
  
    this.activatedRoute.data.subscribe((data: any) => {
      this.openOrderItems = data.openOrderItemsBrief; // data['openOrderItemsBrief'];
      this.existingAssessmentsDto = data.assessmentsDto;  // data['assessmentsDto'];
    })

    //console.log('openOrderItems', this.openOrderItems);
  }



  chooseSelectedOrderItem() {
      //the combobox selected is updated by ngModel to orderItemSelectedId

      if (this.lastOrderItemIdSelected >=0 && this.orderItemSelectedId == this.lastOrderItemIdSelected ) return;

      this.orderItemSelected = this.openOrderItems.find(x => x.orderItemId===this.orderItemSelectedId);
      if(this.orderItemSelected === undefined) return;
      this.requireInternalReview = this.orderItemSelected.requireInternalReview;
      this.orderItemSelectedId = this.orderItemSelected.orderItemId;

      if(this.requireInternalReview) {
          this.qDesigned = this.orderItemSelected!.assessmentQDesigned;

          if (!this.qDesigned) {
            this.toastr.error('assessment Questions not designed - from component');
            return;
          }
      }
      
      this.service.getCandidateAssessment(this.cvBrief!.id, this.orderItemSelectedId).subscribe({
        next: (response: any) => {
            this.assessmentAndChecklist = response;
            
            this.checklist = response.checklistHRDto;

            this.cvAssessment = this.assessmentAndChecklist!.assessed;
            this.qDesigned = this.orderItemSelected!.assessmentQDesigned;

            if (this.cvAssessment) {
              //this.orderItemChangedEventSubject.next(this.cvAssessment);
              this.cvAssessment.candidateAssessmentItems===null 
                ? this.initializeWithoutArray(this.cvAssessment)
                : this.initializeWithArray(this.cvAssessment);
          
            } else {
              this.toastr.warning('the candidate has not been assessed for the category selected.');
            }
        }, error: (err: any) => this.toastr.error(err, 'failed to retrieve candidate assessment')
      })

      this.lastOrderItemIdSelected = this.orderItemSelectedId;

  }

  returnToCalling() {
    
    this.router.navigateByUrl(this.returnUrl || '' );
  } 

  updateAssessment() {
  
    var formdata = this.form.value;

    if(this.cvAssessment) {
      this.toastr.info('cvassessmentid', this.cvAssessment.id.toString()) ;

      if(this.cvAssessment && this.cvAssessment.candidateAssessmentItems === null) {
        var newCVAssessment = new CandidateAssessment();
        newCVAssessment.assessResult = this.form.get('assessResult')!.value; 
        newCVAssessment.assessedByName=this.user!.userName;
        newCVAssessment.assessedOn=new Date(); 
        newCVAssessment.candidateId=this.cvAssessment.candidateId;
        newCVAssessment.hrChecklistId=this.cvAssessment.hrChecklistId; 
        newCVAssessment.id=this.cvAssessment.id;
        newCVAssessment.orderItemId=this.cvAssessment.orderItemId; 
        newCVAssessment.requireInternalReview=this.cvAssessment.requireInternalReview;

        formdata = newCVAssessment;
      };


      if(this.cvAssessment.id === 0) 
        {
          return this.service.saveNewCandidateAssessment(formdata).subscribe({
            next: (response: ICandidateAssessment) => {
                if(response === null) {
                  this.toastr.warning('Failed to insert the CV Assessment', 'Failure')
                } else {
                  this.toastr.success('Inserted the CV Assessment', 'Success')
                }},
              error: (err: any) => this.toastr.error(err, 'failed to save the Assessment')
            })
        } else {

          this.service.updateCandidateAssessment(formdata).subscribe({
            next: (response: boolean) => {
              if(response) {
                this.toastr.success('updated the Candidate Assessment');
              } else {
                this.toastr.warning('failed to update the candidate assessment');
              }
            },
            error: (err: any) => this.toastr.error(err, 'failed to update the candidate assessment')
          })
          
         }
    }
    
    return;
  }

  openShortlistCandidateHelpModal() {
    
      if(this.helpfile===null) {
        this.toastr.info("No help file available for the topic 'Candidate Assessment'");    
        return;
      }

      const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          help: this.helpfile,
        }
      }
      this.bsModalRef = this.bsModalService.show(HelpModalComponent, config);
      this.bsModalRef.content.updateSelectedRoles.subscribe(() => {
      })
  }
  
  private getHelp(topic: string): IHelp|null {
    this.confirmService.getHelp(topic).subscribe((response: any) => {
      this.helpfile = response;
      return;
    }, (error: any) => {
      console.log('failed to retrieve roles array', error);
    })
    return null;
  }

  openSwitchMapModalChecklist() {
    console.log('switchmodal checklist:', this.checklist);

    if(this.checklist && this.cvBrief) {

      const apiCallExternal = this.checklistService.getChecklist(this.cvBrief?.id, this.orderItemSelectedId);
      const apiCallInternal = this.bsModalRef?.content.updateObj;

      apiCallExternal.pipe(
        switchMap(response => 
          apiCallInternal.pipe(
            
            catchError(err => {
              return of();
            }),
            tap(res => {
              this.toastr.success('modal call subscribed');
              if(res==='') {
                //checklist has changed, update the object
                this.service.getCandidateAssessment(this.cvBrief!.id, this.orderItemSelectedId)
                  .subscribe({
                    next: (response: ICandidateAssessmentAndChecklistDto) => {
                      this.assessmentAndChecklist = response;
                      this.checklist = response.checklistHRDto;
                      this.cvAssessment = this.assessmentAndChecklist!.assessed;
                      this.qDesigned = this.orderItemSelected!.assessmentQDesigned;
          
                      if (this.cvAssessment) {
                        this.cvAssessment.candidateAssessmentItems === null 
                          ? this.initializeWithoutArray(this.cvAssessment)
                          : this.initializeWithoutArray(this.cvAssessment);
                     } 
                    }
                  })

              }
            }),
          )),
          catchError(err => {
            this.toastr.error(err, 'error in subscribing to bsModalRef contents');
            return of();
          })
      ).subscribe(
        () => {
          this.toastr.success('inner subscription success');
        },
        err => {
          console.log('error unhandled in catchError')
        }
      )
    }    
  }

  //checklist modal
  openChecklistModal() {

    if(this.checklist ===null) {
      this.toastr.warning('An order item must be selected, to display related checklist', "Order Item not selected");
      return;
    }

      const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
          chklst: this.checklist //this.getChecklistHRDto(this.cvBrief.id, this.orderItemSelectedId),
        }
      }
      
      this.bsModalRef = this.bsModalService.show(ChecklistModalComponent, config);

      this.bsModalRef.content.updateObj.subscribe((response: string) => {
          if(response !== '') {
            this.toastr.warning("Failed to update the Checklist object -" + response);
          } else {
            this.toastr.success('Updated the checklist object');
          }
      })
                
    }

  CheckChecklist(model: IChecklistHRDto) {
    var errors: string[]=[];
    if(model.charges > 0 && model.chargesAgreed != model.charges && !model.exceptionApproved ) errors.push('Charges not agreed');
    model.checklistHRItems.forEach(p => {
      if(p.mandatoryTrue && !p.accepts) errors.push(p.parameter + ' is mandatory, and not resolved');
    })

    return errors;
  }

 
  DisplayHelp() {
    this.bDisplayHelp = !this.bDisplayHelp;
    this.displayText = this.bDisplayHelp ? 'Hide help text' : 'display Help Text';
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
    if(!this.cvAssessment!.id) {
      this.toastr.warning('CV Assessment object is undefined.  It cannot be deleted unless it is saved first');
      return;
    }
    this.service.deleteAssessment(this.cvAssessment!.id).subscribe(response => {
        if (response) {
          this.toastr.success('Candidate Assessment object deleted');
          this.cvAssessment=undefined;
        } else {
          this.toastr.warning('failed to delete the assessment object');
        }
    }, error => {
      this.toastr.error('error in deleting the object', error);
    });
  }
  
  initializeWithArray(assess: ICandidateAssessment) {

      this.form = this.fb.group({
        id:assess.id, orderItemId: assess.orderItemId, candidateId: assess.candidateId, 
        assessedOn: assess.assessedOn, assessResult: assess.assessResult, remarks: assess.remarks,
        requireInternalReview: assess.requireInternalReview,
        
        candidateAssessmentItems: this.fb.array(
          assess.candidateAssessmentItems.map(x => (
            this.fb.group({
              id: x!.id, 
              assessed: [x!.assessed],
              isMandatory: [x!.isMandatory],
              candidateAssessmentId: [x!.candidateAssessmentId, Validators.required],
              questionNo: [x!.questionNo, Validators.required], 
              question: [x!.question, Validators.required],
              maxPoints: [x!.maxPoints, Validators.required], 
              points: x!.points,
              remarks: x!.remarks
            })
          ))
        )
    })

    this.calculatePercentage();
  }

  
  initializeWithoutArray(assess: ICandidateAssessment) {

    this.form = this.fb.group({
      id:assess.id, orderItemId: assess.orderItemId, candidateId: assess.candidateId, 
      assessedOn: assess.assessedOn, assessResult: assess.assessResult, remarks: assess.remarks,
      requireInternalReview: assess.requireInternalReview
    });
     
}

  get candidateAssessmentItems(): FormArray {
    return this.form.get('candidateAssessmentItems') as FormArray;
  }

  
  maxMarksTotal() {

    this.totalPoints =  this.candidateAssessmentItems.value.map((x:any) => x.maxPoints).reduce((a:number, b: number) => a + b,0);
    this.calculatePercentage();
  }

  pointsGainedTotal(i: number){

    if(this.candidateAssessmentItems !== null) {
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
    }
    
    
    //(<FormArray>this.form.controls['candidateAssessmentItems']).at(i).get("assessed").setValue(true);   //set value of the DOM assessed to true
  }

  calculatePercentage() {
    
    this.totalPoints =  this.candidateAssessmentItems.value.map((x:any) => x.maxPoints).reduce((a:number, b: number) => a + b,0);
    this.totalGained = this.candidateAssessmentItems.value.map((x:any) => x.points).reduce((a:number,b:number) => a+b,0);

    this.percentage = (this.totalGained===undefined || this.totalPoints === undefined) ? 0: Math.round(100*this.totalGained / this.totalPoints);
  }

  
  calculateGrade() {
    
    if(this.cvAssessment?.candidateAssessmentItems !== null) {
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
    } else {
      return "Not Assessed";
    }
  
    
  }


  displayAssessment(candidateAssessmentId: number) {


    this.candidateAssessmentService.getCandidateAssessmentById(candidateAssessmentId).subscribe({
      next: (response: any) => {
        if(response) {
          if(response.assessed !==null && response.assessed.candidateAssessmentItems === null) {
            this.initializeWithoutArray(response.assessed)
          } else if(response.assessed !== null && response.assessed.candidateAssessmentItems !== null) {
            this.initializeWithArray(response.assessed)
          }
          this.cvAssessment = response.assessed;
          this.checklist = response.checklistHRDto;
          console.log('checklist:', this.checklist);
        } else {
          this.toastr.warning('Normally, this should not happen, but we failed to retrieve the Candidate Assessment', 'failed')
        }
      },
      error: (err: any) => this.toastr.error(err, 'Error in retrieving the Candidate Assessment')
    })

  }
}
