import { Component, HostListener, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { Subject, catchError, of, switchMap, take, tap } from 'rxjs';
import { ICandidateBriefDto } from 'src/app/_dtos/admin/candidateBriefDto';
import { IOrderItemBriefDto } from 'src/app/_dtos/admin/orderItemBriefDto';
import { CandidateAssessedDto, ICandidateAssessedDto } from 'src/app/_dtos/hr/candidateAssessedDto';
import { ICandidateAssessmentAndChecklistDto } from 'src/app/_dtos/hr/candidateAssessmentAndChecklistDto';
import { ICandidateAssessmentWithErrorStringDto } from 'src/app/_dtos/hr/candidateAssessmentWithErrorStringDto';
import { IChecklistHRDto } from 'src/app/_dtos/hr/checklistHRDto';
import { IHelp } from 'src/app/_models/admin/help';
import { IOrderItemAssessmentQ } from 'src/app/_models/admin/orderItemAssessmentQ';
import { CandidateAssessment, ICandidateAssessment } from 'src/app/_models/hr/candidateAssessment';
import { CandidateAssessmentItem, ICandidateAssessmentItem } from 'src/app/_models/hr/candidateAssessmentItem';
import { IChecklistHRItem } from 'src/app/_models/hr/checklistHRItem';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CandidateAssessmentService } from 'src/app/_services/hr/candidate-assessment.service';
import { ChecklistService } from 'src/app/_services/hr/checklist.service';
import { OrderAssessmentService } from 'src/app/_services/hr/orderAssessment.service';
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

  existingAssessmentsDto: ICandidateAssessedDto[]=[];    //summary of existing assessments of the selected candidate

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
  requireInternalReview: boolean=false;
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
    private assessmentService: OrderAssessmentService,
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
                if(nav.extras.state['.returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

                if( nav.extras.state['cvbrief']) this.cvBrief = nav.extras.state['cvbrief'] as ICandidateBriefDto;
                //if(nav.extras.state.openorderitems) this.existingAssessmentsDto = nav.extras.state.openorderitems as IAssessmentsOfACandidateIdDto[]; 
                //if(nav.extras.state.openorderitems) this.openOrderItems = nav.extras.state.openorderitems as IOrderItemBriefDto[];
            }
            //this.bcService.set('@CVAssess',' ');
     }

  ngOnInit(): void {
  
    this.activatedRoute.data.subscribe((data: any) => {
      this.openOrderItems = data.openOrderItemsBrief; // data['openOrderItemsBrief'];
      this.existingAssessmentsDto = data.assessmentsDto;  // data['assessmentsDto'];
    })

    //console.log('openOrderItems', this.openOrderItems);
  }

 /* initializeTotals() {
    this.totalGained =0;
    this.totalPoints=0;
    this.percentage=0;

  }
*/

  chooseSelectedOrderItem() {
    
      //the combobox selected is updated by ngModel to orderItemSelectedId
      if (this.lastOrderItemIdSelected === this.orderItemSelectedId && this.lastOrderItemIdSelected !== 0) return;
   
      this.orderItemSelected = this.openOrderItems.find(x => x.orderItemId===this.orderItemSelectedId);
      if(this.orderItemSelected === undefined) return;
      this.requireInternalReview = this.orderItemSelected.requireInternalReview;
      this.orderItemSelectedId = this.orderItemSelected.orderItemId;

      if(this.requireInternalReview) {
          this.qDesigned = this.orderItemSelected!.assessmentQDesigned;

          if (!this.qDesigned) {
            console.log('assessment Q not designed');
            this.toastr.error('assessment Questions not designed - from component');
            return;
          }
      }
   
        this.service.getCandidateAssessment(this.cvBrief!.id, this.orderItemSelectedId)
        .subscribe((response: any): void => 
        {
          this.assessmentAndChecklist = response;
          
          this.checklist = response.checklistHRDto;
          console.log('response :', response);

          this.cvAssessment = this.assessmentAndChecklist!.assessed;
          this.qDesigned = this.orderItemSelected!.assessmentQDesigned;

          if (this.cvAssessment) {
            //this.orderItemChangedEventSubject.next(this.cvAssessment);
            
            if(this.cvAssessment)  this.initializeAndPopulateFormArray(this.cvAssessment);
            this.calculatePercentage();

          } else {
            this.toastr.warning('the candidate has not been assessed for the category selected.');
          }
        }, (error: string | undefined) => {
          this.toastr.error('failed to retrieve candidate assessment', error);
          //this.candidateAssessmentItems.clear();
        })
        
        this.lastOrderItemIdSelected = this.orderItemSelectedId;

  }

  returnToCalling() {
    this.router.navigateByUrl(this.returnUrl || '' );
  } 

  createNewAssessment(){

      if (this.cvAssessment !== null) return;
      
      var cvAssess: ICandidateAssessment;
      var cvItems: ICandidateAssessmentItem[]=[];

      if (!this.requireInternalReview) {    //create assessment header without assessment question items
        cvAssess = new CandidateAssessment(
            this.orderItemSelectedId, this.cvBrief!.id, this.user!.userName, new Date(),0, cvItems );
      } else { 
        var items: IOrderItemAssessmentQ[];
        this.assessmentService.getOrderItemAssessmentQs(this.orderItemSelectedId)
          .subscribe(response => {
            items = response;
            if (items.length === 0) {
              this.toastr.warning('The Order Item selected requires internal assessment of candidates, for which ' + 
                'assessment Questions for the Order Item must be designed.  The Selected order item has not been designed');
              this.orderItemChangedEventSubject.next(undefined);        
              return null;
            }
            
            items.forEach(i => {
              var cItem = new CandidateAssessmentItem(i.questionNo,i.isMandatory, i.question, i.maxPoints);
              cvItems.push(cItem);
            })
            this.cvAssessment = new CandidateAssessment(
                this.orderItemSelectedId, this.cvBrief!.id, this.user!.userName, new Date(), 
                this.checklist!.id, cvItems );
              this.orderItemChangedEventSubject.next(this.cvAssessment);   
            
            return;
        }, error => {
            this.toastr.error('failed to create Assessment item', error);
            this.orderItemChangedEventSubject.next(this.cvAssessment!);
            return null;
        })
      }
  }

  updateAssessment() {
  
    if(!this.cvAssessment) {
      this.toastr.info('CV Assessment object null');
      console.log('CV Assessment null');
    }
    
   if(this.cvAssessment) {
    return this.service.updateCandidateAssessment(this.form.value).subscribe((response: any) => {
        if (response) {
          this.toastr.success('updated the Candidate Assessment');
        } else {
          this.toastr.warning('failed to update the candidate assessment');
        }
      }, (error: string | undefined) => {
        this.toastr.error('failed to update the candidate assessment', error);
      })
    } else {
      this.toastr.error('No valid assessment form');
      return false;
    }
  }

  shortlistForForwarding() {
      if(!this.checklist) {
        this.toastr.warning('no checklisting done on the candidate');
        return;
      }
      if (this.cvAssessment !==null) {
        this.toastr.warning('this candidate is already assessed.');
        return;
      }

      //create new CV Assessment
        return this.service.insertNewCVAssessment(false,this.orderItemSelectedId, this.cvBrief!.id).subscribe(
          (response: ICandidateAssessmentWithErrorStringDto) => {
          
          if ((response.errorString==='' || response.errorString ===undefined || response.errorString === null) ) {
            
            this.cvAssessment = response.candidateAssessment;
            
            //update existingAssessmentsDto to update new assessment on the screen
            if (this.cvAssessment !==null) {
              //this.patchForm(this.cvAssessment);    //this is done in child form
                var dto = new CandidateAssessedDto(); // = new AssessmentOfACandidateDto();
                dto.checklistedByName = this.user!.userName;
                dto.checklistedOn = this.checklist!.checkedOn;
                dto.customerName = this.orderItemSelected!.customerName;
                dto.professionName = this.orderItemSelected!.categoryName;
                dto.categoryRef = this.orderItemSelected!.categoryRefAndName;
                dto.assessedOn = new Date();
                dto.assessedByName = this.user!.knownAs;

                this.existingAssessmentsDto.push(dto);
                this.toastr.success('shortlisted for forwarding to client');
            }
          } else if (response.errorString !== '') {
            this.toastr.error('failed to shortlist the candidate, ', response.errorString);
            console.log(response.errorString);
          }
        }, error => {
          this.toastr.error('error in creating the shortlisting', error);
          this.validationErrors = error;
        })
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

  createNewChecklist(){
    if(!this.checklist) 
    {
      this.checklistService.getChecklist(this.cvBrief!.id, this.orderItemSelectedId).subscribe({
        next: (response: IChecklistHRDto) => {
          this.checklist = response;
        },
        error: error => this.toastr.error('failed to retrieve checklist', error)
      })
    } else {
      this.toastr.warning('Order Item should be selected');
    }
    
  }

  openSwitchMapModalChecklist() {
    
    if(this.orderItemSelectedId && !this.checklist && this.cvBrief) {

      const apiCallExternal = this.checklistService.getChecklist(this.cvBrief?.id, this.orderItemSelectedId);
      const apiCallInternal = this.bsModalRef?.content.updateObj;

      apiCallExternal.pipe(
        switchMap(response => 
          apiCallInternal.pipe(
            
            catchError(err => {
              return of();
            }),
            tap(res => this.toastr.success('modal call subscribed')),
          )),
          catchError(err => {
            this.toastr.error('error in subscribing to bsModalRef contents', err);
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
  
  initializeAndPopulateFormArray(assess: ICandidateAssessment) {

      this.form = this.fb.group({
        id:assess.id, orderItemId: assess.orderItemId, candidateId: assess.candidateId, 
        assessedOn: assess.assessedOn, assessResult: assess.assessResult, remarks: assess.remarks,
        
        candidateAssessmentItems: this.fb.array(
          assess.candidateAssessmentItems.map(x => (
            this.fb.group({
              id: x.id, 
              assessed: [x.assessed],
              isMandatory: [x.isMandatory],
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
 
  get candidateAssessmentItems(): FormArray {
    return this.form.get('candidateAssessmentItems') as FormArray;
  }

  
  maxMarksTotal() {

    this.totalPoints =  this.candidateAssessmentItems.value.map((x:any) => x.maxPoints).reduce((a:number, b: number) => a + b,0);
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
    this.totalPoints =  this.candidateAssessmentItems.value.map((x:any) => x.maxPoints).reduce((a:number, b: number) => a + b,0);
    this.totalGained = this.candidateAssessmentItems.value.map((x:any) => x.points).reduce((a:number,b:number) => a+b,0);

    this.percentage = (this.totalGained===undefined || this.totalPoints === undefined) ? 0: Math.round(100*this.totalGained / this.totalPoints);
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

}
