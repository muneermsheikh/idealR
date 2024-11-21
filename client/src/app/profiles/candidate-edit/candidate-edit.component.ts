import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormArray, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { map, of, switchMap, timer } from 'rxjs';
import { IApiReturnDto } from 'src/app/_dtos/admin/apiReturnDto';
import { ICustomerNameAndCity } from 'src/app/_models/admin/customernameandcity';
import { ICandidate } from 'src/app/_models/hr/candidate';
import { IQualification } from 'src/app/_models/hr/qualification';
import { IProfession } from 'src/app/_models/masters/profession';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { QualificationService } from 'src/app/_services/admin/qualification.service';
import { CandidateService } from 'src/app/_services/candidate.service';
import { FileService } from 'src/app/_services/file.service';

@Component({
  selector: 'app-candidate-edit',
  templateUrl: './candidate-edit.component.html',
  styleUrls: ['./candidate-edit.component.css']
})
export class CandidateEditComponent implements OnInit {

  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent | undefined;
  activeTab: TabDirective | undefined;

  @ViewChild("password", { static: true }) passwordElement: ElementRef | undefined;
 
  myPassword = '';

  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;
  //memberPhotoUrl: string='';

  candidate: ICandidate|undefined;
  professions: IProfession[]=[];
  agents: ICustomerNameAndCity[]=[];
  qualifications: IQualification[]=[] ;
  
  errors: string[]=[];

  //id: string='';
  isAddMode: boolean=false;
  strAddMode='';
  loading = false;
  submitted = false;
  bsValueDate = new Date();

  fileToUpload: File | null = null;
  filesToUpload: File[] = [];

  selectedProfession='';
  events: Event[] = [];

  //file upload
  //source:
  @Output() public onUploadFinished = new EventEmitter();
  //isCreate: boolean;
  progress = { loaded : 0 , total : 0 };

  //public progress: number=0;
  public message: string='';  
  isMultipleUploaded = false;
  isSingleUploaded = false;
  urlAfterUpload = '';
  percentUploaded = 0;
  attachmentType: string = '';
  userFiles: File[] = [];
  imageFile: File | undefined;
  response: { dbPath: ''; } | undefined;
  attachmentid: number=0;
  
  routeId='';
  user?: User;
  returnUrl = '/candidates';

  attachmentTypes = [
    {'typeName':'CV'}, 
    {'typeName':'Educational Certificate'},
    {'typeName':'Experience Certificate'}, 
    {'typeName':'Passport Copy'}, 
    {'typeName':'Photograph'},
    {'typeName':'affidavit'},
    {'typeName':'Air Ticket'},
    {'typeName':'Travel Insurance'},
    {'typeName':'Medical Fit certificate'},
    {'typeName':'Visa Copy'},
    {'typeName':'Job Offer'}
  ]
  
  lastTimeCalled: number= Date.now();
  //end of file upload

  constructor(
    private accountService: AccountService, 
    private toastrService: ToastrService, 
    private fb: FormBuilder, 
    private activatedRoute: ActivatedRoute,
    private candidateService: CandidateService,
    private router: Router, private toastr: ToastrService,
    passwordElement: ElementRef,
    private qualService: QualificationService,
    private downloadService: FileService
    ) {
        
      this.passwordElement = passwordElement;    
      this.routeId = this.activatedRoute.snapshot.params['id'];
          //this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);

          this.router.routeReuseStrategy.shouldReuseRoute = () => false;

          //navigationExtras
          let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
              if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

              if( nav.extras.state['user']) {
                this.user = nav.extras.state['user'] as User;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
              //if(nav.extras.state.object) this.orderitem=nav.extras.state.object;
          }
    }

  ngOnInit(): void {

    this.activatedRoute.data.subscribe( data => {
        this.professions = data['categories'],
        this.qualifications = data['qualifications'],
        this.agents = data['agents'],
        this.candidate = data['candidate'];
        this.isAddMode = this.candidate?.id===0;
        this.strAddMode = this.isAddMode ? "Entering new candidate" : "Editing Application No." + this.candidate?.applicationNo;
    });

    if(this.candidate) this.InitializeAndPopulateFormArray(this.candidate);
  }

  InitializeAndPopulateFormArray(cv: ICandidate) {
  
    this.registerForm = this.fb.group({
      id: [cv.id, Validators.required], 
      applicationNo: [cv.applicationNo],
      gender: [cv.gender, [Validators.required, Validators.maxLength(1)]],
      nationality: ['Indian'],
      firstName: [cv.firstName, Validators.required],
      secondName: [cv.secondName],
      familyName: [cv.familyName],
      knownAs: [cv.knownAs, Validators.required],
      dOB: [cv.dOB],
      placeOfBirth: [cv.placeOfBirth],
      aadharNo: [cv.aadharNo],
      ppNo: [cv.ppNo],
      ecnr: [cv.ecnr],
      customerId: [cv.customerId],
      referredByName: [cv.referredByName],

      address: [cv.address],
      city: [cv.city, Validators.required],
      country: [cv.country ?? 'India'],
      pin: [cv.pin],
      district: [cv.district],
      email: [cv.email, Validators.required],

      userPhones: this.fb.array(
        cv.userPhones.map(x => (
          this.fb.group({
            id: x.id, candidateId: [x.candidateId],
            mobileNo: [x.mobileNo, [Validators.required, Validators.maxLength(15), Validators.minLength(10)]],
            isMain: [x.isMain],
            remarks: [x.remarks],
            isValid: [x.isValid]
          })
        ))
      ),

      userQualifications: this.fb.array(
        cv.userQualifications.map(x => (
          this.fb.group({
            id: x.id, candidateId: [x.candidateId],
            qualificationId: [x.qualificationId, Validators.required],
            isMain: [x.isMain]
          })
        ))
      ),

      userProfessions: this.fb.array(
        cv.userProfessions.map(x => (
          this.fb.group({
            id: x.id, candidateId: x.candidateId,
            professionId: [x.professionId, Validators.required],
            isMain: [x.isMain]
          })
        ))
      ),

      userExperiences: this.fb.array(
        cv.userExperiences.map(x => (
          this.fb.group({
            id: x.id, candidateId: x.candidateId,
            employer: x.employer, position: x.position,
            salaryCurrency: x.salaryCurrency, 
            monthlySalaryDrawn: x.monthlySalaryDrawn,
            workedFrom: x.workedFrom, workedUpto: x.workedUpto
          })
        ))
      ),

      userAttachments: this.fb.array(
        cv.userAttachments.map(x => (
          this.fb.group({
            id: x.id, candidateId: x.candidateId,
            appUserId: x.appUserId,
            attachmentType: x.attachmentType,
            name: [x.name, Validators.required],
            attachmentSizeInBytes: x.attachmentSizeInBytes,
            url: x.url
          })
        ))
      )

    })

    if(this.registerForm.controls['password']) {
      this.registerForm.controls['password'].valueChanges.subscribe({
        next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
      })

    } 
  }

  //edit form
  //getters
  get userPhones() : FormArray {
    return this.registerForm.get("userPhones") as FormArray
  }

  get userQualifications() : FormArray {
    return this.registerForm.get("userQualifications") as FormArray
  }
  
  get userProfessions() : FormArray {
    return this.registerForm.get("userProfessions") as FormArray
  }

  get userExperiences() : FormArray {
    return this.registerForm.get("userExperiences") as FormArray
  }

  //add controls to formarrays
  newUserPhone(): FormGroup {
    return this.fb.group({
      candidateId: this.candidate?.id,
      mobileNo: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(15)]],
      isMain: false,
      remarks: ''
    })
  }
  addPhone() {
    this.userPhones.push(this.newUserPhone());
  }
  removeUserPhone(i:number) {
    this.userPhones.removeAt(i);
  }

  
  newQualification(): FormGroup {
    return this.fb.group({
      candidateId: this.candidate?.id,
      qualificationId: 0,
      qualificationName: '',
      isMain: [false]
    })
  }
  addQualification() {
    this.userQualifications.push(this.newQualification());
  }
  removeUserQualification(i:number) {
    this.userQualifications.removeAt(i);
  }

  newUserProfession(): FormGroup {
    return this.fb.group({
      professionId: [0, Validators.required],
      isMain: [false, Validators.required],
      candidateId: this.candidate?.id
    })
  }

  addUserProfession() {
    this.userProfessions.push(
      this.fb.group({
        candidateId: [0, Validators.required],
        professionId: [0, Validators.required],
        isMain: [false, Validators.required]
      })
      //this.newUserProfession()
    );
  }

  removeUserProfession(i:number) {
    this.userProfessions.removeAt(i);
  }
  
  newUserExp(): FormGroup {
    return this.fb.group({
      candidateId: this.candidate?.id,
      srNo: [0, Validators.required],
      position: ['', Validators.required],
      company: ['', Validators.required],
      workedFrom: '',
      workedUpto: ''
    })
  }

  addUserExp() {
    this.userExperiences.push(this.newUserExp());
  }

  removeUserExp(i:number) {
    this.userExperiences.removeAt(i);
  }
    
  //userAttachments
  get userAttachments() : FormArray {
    return this.registerForm.get("userAttachments") as FormArray
  }
  newUserAttachment(): FormGroup {
    return this.fb.group({
      id:0,
      candidateId: this.candidate===undefined ? 0 : this.candidate.id,
      attachmentType: [''],
      name: ['', Validators.required],
      attachmentSizeInBytes: 0,
      url: ''
    })
  }

  newUserAttachmentWithFile(f:File): FormGroup {
    return this.fb.group({
      id:0,
      candidateId: this.candidate===undefined ? 0 : this.candidate.id,
      attachmentType: ['', Validators.required],
      name: [f.name, Validators.required],
      attachmentSizeInBytes: f.size,
      url: ''
    })
  }

  addUserAttachment() {
    this.userAttachments.push(this.newUserAttachment());
  }
  removeUserAttachment(i:number) {
    this.userAttachments.removeAt(i);
  }


  //get data from api
  getCVById(id: number) {
      return this.candidateService.getCandidate(id).subscribe({
        next: response => {
          this.candidate = response;
          //this.candidate  = {...this.candidate, userType: 'candidate'};
          this.InitializeAndPopulateFormArray(this.candidate);
        },
        error: error => this.toastrService.error('failed to get candidate to edit:', error)
      })
  }

   getQualifications(){
    return this.qualService.getQualificationList().subscribe({
      next: (response: any) => this.qualifications=response,
      error: (error: any) => this.toastrService.error('error in qualificatin', error)
    })
  }
  
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
    this.router.navigateByUrl(this.returnUrl);
  }

  selectTab(tabId: number) {
    this.memberTabs!.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
  }

  validateEmailNotTaken(): AsyncValidatorFn {
    return control => {
      //return timer(500).pipe(
      return timer(10).pipe(
        switchMap(() => {
          if (!control.value) {
            return of(null);
          }
          return this.accountService.checkEmailExists(control.value).pipe(
            map(res => {
              return res ? {emailExists: true} : null;
            })
          );
        })
      )
    }
  }

  validatePPNotTaken(): AsyncValidatorFn {
    return control => {
      return timer(10).pipe(
        switchMap(() => {
          if (!control.value) {
            return of(null);
          }
          return this.accountService.checkPPExists(control.value).pipe(
            map(res => {
              if (res !== null) this.toastrService.warning('that passport number is taken by ' + res);
              return res ? {ppExists: true} : null;
            })
          );
        })
      )
    }
  }

  handleFileInput(files: any) {
    //var f: files.target.files;
    //this.fileToUpload = files.item(0);
    this.fileToUpload = files[0];
    this.filesToUpload.push(files[0]);
  }

  getAgentValue(agt: ICustomerNameAndCity)
  {
    console.log('agent selected', agt);
  }

  getProfessionValue(prof: IProfession) {
    console.log('ngx dropdown selected', prof);
  } 
  
  getValues( $event: any) {
    console.log('ngx dropdown selected', this.selectedProfession);
  }

  onFileInputChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const files = target.files as FileList;
    const f = files[0];
    if(f.size > 0) {
      this.userFiles.push(f);
      this.userAttachments.push(this.newUserAttachmentWithFile(f));
    }
    
  }

  savewithattachments = () => {
    var microsecondsDiff: number= 28000;
    var nowDate: number =Date.now();
    
    if(nowDate < this.lastTimeCalled+ microsecondsDiff) return;
    
    this.lastTimeCalled=Date.now();
    const formData = new FormData();
    const formValue = this.registerForm.value;
    
    if(this.userFiles.length > 0) {
      this.userFiles.forEach(f => {
        formData.append('file', f, f.name);
      })
    }

    formData.append('data', JSON.stringify(this.registerForm.value));

    if(!this.candidate && formData.get('password') === undefined) {
      this.toastrService.info('Password not provided');
      return;
    }

    if(this.candidate!.id ===0 ) {   //insert new cv
      this.toastrService.info('inserting ...');

        this.candidateService.registerNewWithFiles(formData).subscribe({
          next: (response: IApiReturnDto) => {
            console.log('response in angular:', response);
            if(response.errorMessage!=='') {
              this.toastrService.error('failed to save the candidate data', response.errorMessage);
            } else {
              this.toastrService.success('candidate saved, with Application No. ' + response.returnInt.toString(), 'Profile successfully inserted');
              this.registerForm.setValue({'applicationNo': response.returnInt});
            }},
          error: error => {
            this.toastrService.error('failed to insert the candidate', error.error.details);

          }
    })} else {
        this.toastrService.info('udating ...');
        this.candidateService.UpdateCandidateWithFiles(formData).subscribe({
          next: (response: IApiReturnDto) => {
            //console.log('returned from api:', response);
            if(response?.errorMessage !== '') {
              this.toastrService.error('failed to update the candidate', response.errorMessage);
            } else {
              this.toastrService.success('updated the candidate successfully');
              this.router.navigateByUrl(this.returnUrl);
            }},
            error: error => {
              this.toastrService.error('failed to update the candidate', error)
              console.log(error);
            }
          })
    }
  }


  uploadFinished = (event: any) => { 
    this.response = event; 
  }

  
  downloadattachment(index: number) {
    var attachment = this.userAttachments.at(index).value;
    if(attachment.id ===0) {
      this.toastrService.warning('this item has no primary key value');
      return;
    }
    var attachmentid=attachment.id;
    var filenameWithExtn = attachment.name;

    this.downloadService.download(attachmentid).subscribe({
      next: (blob: Blob) => {
        const a = document.createElement('a');
        const objectUrl = URL.createObjectURL(blob);
        a.href = objectUrl;
        a.download = filenameWithExtn;

        a.click();
        URL.revokeObjectURL(objectUrl);
      },
      error: (err: any) => this.toastr.error(err.error.details, 'Error while downloading')
    })
  }


  IsNewAttachment(index: number): boolean {
    var id = this.userAttachments.value.map((x:any) => x.id).findIndex((x:any) => x.id ===0);
    return id !== undefined;
  }
  
}
