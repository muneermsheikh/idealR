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
import { IUserAttachment } from 'src/app/_models/hr/userAttachment';
import { IUserExp } from 'src/app/_models/hr/userExp';
import { IProfession } from 'src/app/_models/masters/profession';
import { IUserPhone } from 'src/app/_models/params/Admin/userPhone';
import { IUserProfession } from 'src/app/_models/params/Admin/userProfession';
import { IUserQualification } from 'src/app/_models/params/Admin/userQualification';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { QualificationService } from 'src/app/_services/admin/qualification.service';
import { CandidateService } from 'src/app/_services/candidate.service';

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
  memberPhotoUrl: string='';

  candidate: ICandidate|undefined;
  professions: IProfession[]=[];
  agents: ICustomerNameAndCity[]=[];
  qualifications: IQualification[]=[] ;
  
  errors: string[]=[];

  //id: string='';
  isAddMode: boolean=false;
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
    {'typeName':'CV'}, {'typeName':'Educational Certificate'},{'typeName':'Experience Certificate'}, {'typeName':'Passport Copy'}, {'typeName':'Photograph'}  
  ]
  
  lastTimeCalled: number= Date.now();
  //end of file upload

  constructor(
    private accountService: AccountService, 
    private toastrService: ToastrService, 
    private fb: FormBuilder, 
    private activatedRoute: ActivatedRoute,
    private candidateService: CandidateService,
    private router: Router,
    passwordElement: ElementRef,
    private qualService: QualificationService
    ) {
        //initialize controls
            this.registerForm = this.fb.group({
              id: 0,
              userType: ['candidate', Validators.required],
              applicationNo:0,        
              gender: ['male', Validators.required],
              nationality: ['Indian', Validators.required],
              knownAs: ['kadir', Validators.required],
              username: 'kadir.hassan@gmail.com',
              firstName: ['Kadir', Validators.required],
              secondName: 'Hassan',
              familyName: 'Shaikh',
              dOB: '1990-10-10T12:00:00',
              placeOfBirth: 'Latur',
              aadharNo: '123456654321',
              photoUrl: '',
              ppNo: 'X-3945999',
              ecnr: [true],
              referredBy: 9,    //direct
              referredByName: '',
              
              password: ['']  , /*[Validators.required, 
                Validators.minLength(4), Validators.maxLength(8)]], */
              confirmPassword: '',  // [Validators.required, this.matchValues('password')]],
              
            //}),
            //userAddressForm: this.fb.group({
              address: '',
              address2: '',
              city: ['Mumbai', Validators.required],
              pin: '401107',
              district:'',
              country: ['India'],
              email: ['kadir.hassan@gmail.com', Validators.email],
            //})
            
            userPhones: this.fb.array([]),
            userQualifications: this.fb.array([]), 
            userProfessions: this.fb.array([]),
            //userPassports: this.fb.array([]),
            //entityAddresses: this.fb.array([]),
            userExperiences: this.fb.array([]),
            userAttachments: this.fb.array([])
        
          });
        
          if(this.registerForm.controls['password']) {
            this.registerForm.controls['password'].valueChanges.subscribe({
              next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
            })
        
          }
        //end of initilize controls
      
      
      this.passwordElement = passwordElement;    
      this.routeId = this.activatedRoute.snapshot.params['id'];
          //this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);

          this.routeId = this.activatedRoute.snapshot.params['id'];
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
       
    });

    this.initializeForm();

    //if(!this.isAddMode) this.editCandidate(this.candidate!);
    if(!this.isAddMode && this.candidate) this.InitializeAndPopulateFormArray(this.candidate);
  }

  InitializeAndPopulateFormArray(cv: ICandidate) {

    console.log('candidate in initialize: ', cv);
    
    this.registerForm = this.fb.group({
      id: [cv.id, Validators.required], 
      userType: [cv.userType, Validators.required],
      applicationNo: [cv.applicationNo, Validators.required],
      gender: [cv.gender, Validators.required],
      nationality: ['Indian', Validators.required],
      firstName: [cv.firstName, Validators.required],
      secondName: [cv.secondName],
      familyName: [cv.familyName],
      knownAs: [cv.knownAs, Validators.required],
      dOB: [cv.dOB],
      placeOfBirth: [cv.placeOfBirth],
      aadharNo: [cv.aadharNo],
      ppNo: [cv.ppNo],
      ecnr: [cv.ecnr],
      referredBy: [cv.referredBy, Validators.required],
      referredByName: [cv.referredByName],

      address: [cv.address],
      city: [cv.city, Validators.required],
      pin: [cv.pin],
      district: [cv.district],
      email: [cv.email, Validators.required],

      userPhones: this.fb.array(
        cv.userPhones.map(x => (
          this.fb.group({
            id: x.id, candidateId: [x.candidateId, Validators.required],
            mobileNo: [x.mobileNo, Validators.required],
            isMain: [x.isMain],
            remarks: [x.remarks, Validators.required],
            isValid: [x.isValid]
          })
        ))
      ),

      userQualifications: this.fb.array(
        cv.userQualifications.map(x => (
          this.fb.group({
            id: x.id, candidateId: [x.candidateId, Validators.required],
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
            fileName: x.fileName,
            attachmentSizeInBytes: x.attachmentSizeInBytes,
            url: x.url, attacahmentType: x.attachmentType
          })
        ))
      )

    })
  }

  initializeForm() {
    this.registerForm = this.fb.group({
        id: 0,
        userType: ['candidate', Validators.required],
        applicationNo:0,        
        gender: ['male', Validators.required],
        nationality: ['Indian', Validators.required],
        knownAs: ['kadir', Validators.required],
        username: 'kadir.hassan@gmail.com',
        firstName: ['Kadir', Validators.required],
        secondName: 'Hassan',
        familyName: 'Shaikh',
        dOB: '1990-10-10T12:00:00',
        placeOfBirth: 'Latur',
        aadharNo: '123456654321',
        photoUrl: '',
        ppNo: 'X-3945999',
        ecnr: true,
        referredBy: 9,    //direct
        referredByName: '',
        
        password: ['']  , /*[Validators.required, 
          Validators.minLength(4), Validators.maxLength(8)]], */
        confirmPassword: '',  // [Validators.required, this.matchValues('password')]],
        
      //}),
      //userAddressForm: this.fb.group({
        address: '',
        address2: '',
        city: ['Mumbai', Validators.required],
        pin: '401107',
        district:'',
        country: ['India'],
        email: ['kadir.hassan@gmail.com', Validators.email],
      //})
      
      userPhones: this.fb.array([]),
      userQualifications: this.fb.array([]), 
      userProfessions: this.fb.array([]),
      //userPassports: this.fb.array([]),
      //entityAddresses: this.fb.array([]),
      userExperiences: this.fb.array([]),
      userAttachments: this.fb.array([])

    });

    if(this.registerForm.controls['password']) {
      this.registerForm.controls['password'].valueChanges.subscribe({
        next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
      })

    }


  }

  //edit form

  editCandidate(cv: ICandidate) {
    //const dob = this.getDateOnly(this.registerForm.controls['dOB'].value!);
    //const values = {...this.registerForm.value!, dateOfBirth: dob!};
    this.registerForm.patchValue( {
      id: cv.id, userType: cv.userType , applicationNo: cv.applicationNo, gender: cv.gender, 
      firstName: cv.firstName, secondName: cv.secondName, familyName: cv.familyName, knownAs: cv.knownAs, 
      referredBy: cv.referredBy, dOB: cv.dOB, placeOfBirth: cv.placeOfBirth, aadharNo: cv.aadharNo, 
      ppNo: cv.ppNo, ecnr: cv.ecnr, city: cv.city, pin: cv.pin, nationality: cv.nationality, 
      email: cv.email, companyId: cv.companyId, notificationDesired: cv.notificationDesired
    });
      if(cv.photoUrl) this.memberPhotoUrl = 'https://localhost:5001/api/assets/images/' + cv.photoUrl;
      
      //if (cv.userProfessions) 
        this.registerForm.setControl('userProfessions', this.setExistingProfs(cv.userProfessions));
      //if (cv.userPhones) 
        this.registerForm.setControl('userPhones', this.setExistingPhones(cv.userPhones));
      //if (cv.userQualifications) 
        this.registerForm.setControl('userQualifications', this.setExistingQ(cv.userQualifications));
      //if (cv.userExperiences) 
        this.registerForm.setControl('userExperiences', this.setExistingExps(cv.userExperiences));
      //if (cv.userAttachments) 
        this.registerForm.setControl('userAttachments', this.setExistingAttachments(cv.userAttachments));
    
  }

  setExistingPhones(userphones: IUserPhone[]) {

    console.log('setExistingphones', userphones);

    userphones?.forEach(ph => {
      this.userPhones.push(
        this.fb.group({
          id: ph.id,
        candidateId: ph.candidateId,
        mobileNo: ph.mobileNo,
        isMain: ph.isMain,
        remarks: ph.remarks,
        isValid: ph.isValid
        }))
    })
    
}

  setExistingQ(userQ: IUserQualification[]) {
      userQ?.forEach(q => {
        this.userQualifications.push(
          this.fb.group({
            id: q.id,
            candidateId: q.candidateId,
            qualificationId: q.qualificationId,
            qualification: q.qualification,
            isMain: q.isMain
          })
        )
      })  
    }

  setExistingProfs(userProfs: IUserProfession[]) {
    console.log('setExistingProfs', userProfs);
    userProfs?.forEach(p => {
      this.userProfessions.push(
        this.fb.group({
          professionId: p.professionId,
          isMain: false,
          candidateId: p.candidateId
        })
      )
    })

   
    }

  setExistingExps(userExps: IUserExp[]){
    userExps.forEach(p => {
      this.userExperiences.push(
        this.fb.group({
          id: p.id,
        candidateId: p.candidateId,
        employer: p.employer,
        position: p.position,
        salaryCurrency: p.salaryCurrency,
        monthlySalaryDrawn: p.monthlySalaryDrawn,
        workedFrom: p.workedFrom,
        workedUpto: p.workedUpto
        })
      )
    })
   
  }  

  setExistingAttachments(userAttachs: IUserAttachment[]) {

    userAttachs.forEach(p => {
      this.userAttachments.push(
        this.fb.group({
          id: p.id,
          candidateId: p.candidateId,
          appUserId: p.appUserId,
          fileName: p.fileName,
          attachmentSizeInBytes: p.attachmentSizeInBytes,
          url: p.url,
          attachmentType: p.attachmentType
        })
      )
    })
       
  } 
  
  
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
      mobileNo: ['', Validators.required],
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
      isMain: [false, Validators.required]
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
      attachmentType: ['', Validators.required],
      fileName: ['', Validators.required],
      attachmentSizeInBytes: 0,
      url: ''
    })
  }

  newUserAttachmentWithFile(f:File): FormGroup {
    return this.fb.group({
      id:0,
      candidateId: this.candidate===undefined ? 0 : this.candidate.id,
      attachmentType: ['', Validators.required],
      fileName: [f.name, Validators.required],
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
          this.editCandidate(this.candidate);
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
    /*if (!this.candidate && this.passwordElement?.nativeElement === undefined) {
      this.toastrService.error('Password not provided');
      return;
    }
    */
    //this.myPassword = this.passwordElement!.nativeElement!.value;

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
            
            if(response.errorMessage!==null) {
              this.toastrService.error('failed to save the candidate data', response.errorMessage);
            } else {
              this.toastrService.success('candidate saved, with Application No. ' + response.returnInt.toString(), 'Profile successfully inserted');
              this.registerForm.setValue({'applicationNo': response.returnInt});
            }},
          error: error => this.toastrService.error('failed to insert the candidate', error)
    })} else {
        this.toastrService.info('udating ...');
        this.candidateService.UpdateCandidateWithFiles(formData).subscribe({
          next: (response: string) => {
            if(response !== '') {
              this.toastrService.error('failed to update the candidate', response);
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

  download(index: number) {

  }

  IsNewAttachment(index: number): boolean {
    var id = this.userAttachments.value.map((x:any) => x.id).findIndex((x:any) => x.id ===0);
    return id !== undefined;
  }
}
