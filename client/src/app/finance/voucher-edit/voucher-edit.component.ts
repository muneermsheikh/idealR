import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap } from 'rxjs';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { coa, ICOA } from 'src/app/_models/finance/coa';
import { IVoucher, Voucher } from 'src/app/_models/finance/voucher';
import { IVoucherAttachment, VoucherAttachment } from 'src/app/_models/finance/voucherAttachment';
import { IVoucherEntry } from 'src/app/_models/finance/voucherEntry';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { FileService } from 'src/app/_services/file.service';
import { COAService } from 'src/app/_services/finance/coa.service';
import { VouchersService } from 'src/app/_services/finance/vouchers.service';
import { CoaEditModalComponent } from '../coa-edit-modal/coa-edit-modal.component';
import { HttpErrorResponse, HttpEventType, HttpResponse } from '@angular/common/http';
import { Pagination } from 'src/app/_models/pagination';

@Component({
  selector: 'app-voucher-edit',
  templateUrl: './voucher-edit.component.html',
  styleUrls: ['./voucher-edit.component.css']
})
export class VoucherEditComponent implements OnInit{

  @Output() public onUploadFinished = new EventEmitter();

  voucher?: IVoucher;
  entries: IVoucherEntry[]=[];

  coas: ICOA[]=[];
  emps: IEmployeeIdAndKnownAs[]=[];
  user?: User;
  
  selectedAccountId: number=0;
  
  bsValue? : Date; //= new Date();
  bsRangeValue= new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();

  minTransDate = new Date();
  maxTransDate = new Date();

  returnUrl: string='';
  routeId: string='';
  routeResumeId: string='';


  isAddMode: boolean = false;
  isEditable: boolean = false;

  loading = false;
  submitted = false;
  bolNavigationExtras:boolean=false;

  totalAmountDR: number = 0;
  totalAmountCR: number = 0;
  voucherAmount = 0;

  diff: string='';
  iDiff=0;

  form: FormGroup = new FormGroup({});
  pagination: Pagination|undefined;

  bsModalRef?: BsModalRef;

  suggestedDefaultCoaIdDR: number=0;
  suggestedDefaultCoaIdCR: number=0;
  suggestedDefaultAmountDR: number=0;
  suggestedDefaultAmountCR: number=0;

  //fileupload variables
  attachmentid: number=0;
  fileAttachments: IVoucherAttachment[]=[];
  voucherFiles: File[] = [];
  
  public progress: number=0;
  public message: string='';  
  isMultipleUploaded = false;
  isSingleUploaded = false;
  urlAfterUpload = '';
  percentUploaded = [0];
  lastTimeCalled: number= Date.now();

  //filedownoads
  response?: {dbPath: ''};
  
  constructor(private service: VouchersService, 
    private coaService: COAService,
    private fileService: FileService,
    private activatedRoute: ActivatedRoute, 
    private router: Router,
    private toastr: ToastrService, 
    private fb: FormBuilder,
    private modalService: BsModalService,
    private confirmService: ConfirmService) { 
         
        this.routeId = this.activatedRoute.snapshot.params['id'];
        if(this.routeId==undefined) this.routeId='';
        if(this.routeId==='' 
          //&&  (this.voucher===null || this.voucher===undefined)
        ) this.isAddMode=true;

        //read navigationExtras
        let nav: Navigation | null= this.router.getCurrentNavigation();
          
        if (nav?.extras && nav.extras.state) {
            this.bolNavigationExtras=true;
            if(nav.extras.state['userObject']) this.user = nav.extras.state['userObject'];
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            this.isEditable = nav.extras.state['toedit'] || this.isAddMode;
        }
          
        this.minDate.setDate(this.minDate.getDate() - 365);
        this.maxDate.setDate(this.maxDate.getDate() + 365);
        this.minTransDate.setDate(this.minTransDate.getDate()-365);   //max o2 day back
        //this.bcService.set('@FinanceTransaction',' ');
    }

    ngOnInit(): void {
      
      this.activatedRoute.data.subscribe(data => {
        this.coas = data['coas'];
        this.voucher = data['voucher'];
        
      })

      if(this.voucher !== undefined && this.voucher !== null) {
        this.createForm(this.voucher);
        this.recalculateTotal();
      }
      
    }

    createForm(vch: IVoucher) {
      
      this.form = this.fb.group({
        id: vch.id,
        divn: [vch.divn, Validators.maxLength(1)],
        voucherNo: [vch.voucherNo, Validators.required],
        voucherDated: [vch.voucherDated, Validators.required],
        cOAId: [vch.cOAId, Validators.required],
        amount: [vch.amount, [Validators.required, Validators.min(1)]],
        totalAmountDR: [this.totalAmountDR, this.matchValues('amount')],
        narration: vch.narration,
        username: vch.username,

        voucherEntries: this.fb.array(
          vch.voucherEntries.map(x => (
            this.fb.group({
              id: x.id, financeVoucherId: x.voucherId, cOAId: x.cOAId, 
              accountName: x.accountName, transDate: x.transDate, 
              dr: x.dr, cr: x.cr, narration: x.narration,
              drEntryApproved: x.drEntryApproved, drEntryApprovedOn: x.drEntryApprovedOn,
              drEntryApprovedByUsername: x.drEntryApprovedByUsername
            })
          )))
        
        , voucherAttachments: this.fb.array(
          vch.voucherAttachments.map(x => (
            this.fb.group({
              id: x.id,
              voucherId: x.voucherId,
              fileName: x.fileName,
              url: x.url,
              uploadedByUsername: x.uploadedByUsername,
              dateUploaded: x.dateUploaded,
              attachmentSizeInBytes: x.attachmentSizeInBytes
            })
          ))
        ) 
      });
      
      /*this.form.controls['amount'].valueChanges.subscribe({
        next: () => this.form.controls['totalAmountDR'].updateValueAndValidity()
      })
      */
    }

      
    matchValues(matchTo: string): ValidatorFn {
      return (control: AbstractControl) => {
        return control.value === control.parent?.get(matchTo)?.value ? null : {isMatching: true}
      }
    }

    //voucher entries
    get voucherEntries(): FormArray {
      return this.form.get("voucherEntries") as FormArray;
    }
    
    addVoucherEntry() {
      if(this.voucherEntries.length==0) { 
          this.setDefaultEntryValues();
          this.voucherEntries.push(this.newDRVoucherEntry());
          this.voucherEntries.push(this.newCRVoucherEntry());
      } else {
        this.iDiff = this.totalAmountDR - this.totalAmountCR;
        if(this.iDiff < 0) {    //CREDIT
          this.suggestedDefaultAmountDR = Math.abs(this.iDiff);
          this.voucherEntries.push(this.newDRVoucherEntry());
        } else {
          this.suggestedDefaultAmountCR = Math.abs(this.iDiff);
          this.suggestedDefaultCoaIdCR = this.form.controls['coa'].value;

          this.voucherEntries.push(this.newCRVoucherEntry());
        }
      }
      this.recalculateTotal();
    }
    
    newDRVoucherEntry(): any {
      return this.fb.group({
        id: 0,
        financeVoucherId: this.voucher?.id === undefined ? 0 : this.voucher.id, 
        transDate: new Date(), 
        cOAId: this.suggestedDefaultCoaIdDR, 
        accountName:'', 
        dr: this.suggestedDefaultAmountDR, 
        cr: 0,
        narration: '',
        drEntryApproved: false,
        //approved: [false, {disabled:!this.user?.roles.includes('finance')}],
        drEntryApprovedOn: new Date(),
        drEntryApprovedByUsername: this.user?.userName,
        /*filePath: '',
        fileName:'',
        fileType: '',
        fileSize:0*/
      })
    }

    newCRVoucherEntry(): any {
      var craccountid=this.form.controls['cOAId'].value;
      return this.fb.group({
        id: 0,
        financeVoucherId: this.voucher?.id === undefined ? 0 : this.voucher.id, 
        transDate: new Date(), 
        cOAId: craccountid ?? this.suggestedDefaultCoaIdCR, 
        accountName:'', 
        cr: this.suggestedDefaultAmountCR ,
        dr: 0,
        narration: '',
        drEntryApproved: false,
        drEntryApprovedOn: new Date(),
        drEntryApprovedByUsername: this.user?.userName,
        /*filePath: '',
        fileName:'',
        fileType: '',
        fileSize:0*/
      })
    }
    
    removeVoucherEntry(i: number) {
      this.voucherEntries.removeAt(i);
      this.voucherEntries.markAsDirty();
      this.voucherEntries.markAsTouched();
    } 

//voucher attachments
    newVoucherAttachment(): any {
      return this.fb.group({
        id: 0, 
        voucherId: this.voucher?.id ?? 0,
        fileName: ['', Validators.required],
        url: [{value:'',disabled: true}],
        attachmentSizInBytes: 0,
        dateUploaded: new Date(),
        uploadedByEmployeeId: 0
      })
    }

    addVoucherAttachmnt() {
        this.voucherAttachments.push(this.newVoucherAttachment());
    }

    removeVoucherAttachment(i: number) {
      this.voucherAttachments.removeAt(i);
      this.voucherAttachments.markAsDirty();
      this.voucherAttachments.markAsTouched();
     } 

    get voucherAttachments(): FormArray {
      return this.form.get("voucherAttachments") as FormArray;
    }
    //end of attachments
    
    recalculateTotal() {
      this.totalAmountDR = +this.voucherEntries.value.map((x: any) => x.dr).reduce((a:number,b:number) => a+b,0);
      this.totalAmountCR = +this.voucherEntries.value.map((x: any) => x.cr).reduce((a:number,b:number) => a+b,0);
      var d = this.totalAmountDR - this.totalAmountCR;
      this.diff = Math.abs(d).toString();
      this.diff += d > 0 ? ' DR' : ' CR';
      this.iDiff = Math.abs(d);
    }

    updateVoucherAmount()
    {
        this.voucherAmount = this.form.controls['amount'].value;
    }

    setDefaultEntryValues(): any {
      var icOAId = this.form.controls['cOAId'].value;
      var vAmount = +this.form.controls['amount'].value;

      var accountclass = this.coas.filter(x => x.id == icOAId).map(x => x.accountClass);
      
      this.suggestedDefaultAmountCR = vAmount; //petty cash
      this.suggestedDefaultAmountDR = vAmount;
      
      switch(accountclass[0]) {
        case 'exp':  //refeshment
          this.suggestedDefaultCoaIdCR = this.suggestedDefaultAmountCR > 5000 ? 14: 2;  //Bank or petty cash
          this.suggestedDefaultCoaIdDR =  icOAId;
          break;

        case 'salary':   //salary
        case 'businessvisit': //business visits
          this.suggestedDefaultCoaIdDR = icOAId;
          this.suggestedDefaultCoaIdCR =  14;  //Afreen kapur c/a
          break;
          break;
        
        case 'candidate':
          this.suggestedDefaultCoaIdDR = icOAId;
          this.suggestedDefaultCoaIdCR = 20;    //sales recruitment
          break;

        default:
          break;
      }

    }

    deleteVoucher() {
      
      const observableInner = this.service.deleteVoucher(this.voucher!.id);
      const observableOuter = this.confirmService.confirm('confirm Delete', 'Are you sure you want to delete this Voucher, along with all its contents?');

      observableOuter.pipe(
          filter((confirmed) => confirmed),
          switchMap(() => {
            return observableInner
          })
      ).subscribe(response => {
        if(response) {
          this.toastr.success('Voucher deleted', 'deletion successful');
        } else {
          this.toastr.error('Error in deleting the checklist', 'failed to delete')
        }
        
      });
    }

    returnToCaller() {
      this.router.navigateByUrl(this.returnUrl || '');
    }

    addNewCOA() {

      var _coa: ICOA = new coa;
      const config = {
        class:'modal-dialog-centered modal-lg',
        initialState: {
          title: 'add a new Chart Of Account',
          coa: _coa
        }
      };

      this.bsModalRef = this.modalService.show(CoaEditModalComponent, config);
      // First API call
      this.bsModalRef.content.editCOAEvent
      .pipe(
          // Logging has a side effect, so use tap for that
          //tap(icoa => console.log('First API success', icoa)),
          // Call the second if first succeeds
          switchMap((coaObj:ICOA) => this.coaService.addNewCOA(coaObj).pipe(
              // Handle your error for second call, return of(val) if succeeds or throwError() if you can't handle it here
              catchError(err => { 
                  console.log('Error for coaService.AddNewCOA', err);
                  return of(); 
              }),
              // Logging is a side effect, so use tap for that
              //tap(res => console.log('Second API success', res)),
              //update DOM
              
          )),
          // Handle your error for first call, return of(val) if succeeds or throwError() if you can't handle it here
          catchError(err => { 
            //console.log('Error for first API- ICOA objct frm odal form)', err);
            return of(); 
          }) 
      ).subscribe(
          // handle both successfull
          (coaAdded : any) => {
            this.coas.push(coaAdded)
          //console.log('Both APIs succeeded, result from 2) is returned', coaAdded);
        }),
        // handle uncaught errors
        (err : any) => {
          console.log('Any error NOT handled in catchError() or if you returned a throwError() instead of of() inside your catchError()', err);
        } 
    }

    onFileInputChange(event: Event, voucherId: number) {
      const target = event.target as HTMLInputElement;
      const files = target.files as FileList;
      const f = files[0];
      
      var newAttachment =  new VoucherAttachment();
      newAttachment.voucherId=voucherId ?? 0;
      newAttachment.fileName= f.name;
      newAttachment.attachmentSizeInBytes= Math.round(f.size/1024)
      newAttachment.dateUploaded=new Date;      
      this.fileAttachments.push(newAttachment);
      this.voucherFiles.push(f);

      //add to the formArray

      var newFileAttachment =  this.fb.group({
        voucherId: this.voucher?.id ?? 0,
        fileName: f.name,
        attachmentSizeInBytes: Math.round(f.size/1024)
      })
      this.voucherAttachments.push(newFileAttachment);

    }

    uploadFileAndFormData = () => {
      if(this.form.invalid) {
        this.toastr.warning("Invalid form");
        return;
      }
     
      var microsecondsDiff: number= 28000;
      var nowDate: number =Date.now();
      
      //console.log('nowDate', nowDate, ' last time', this.lastTimeCalled);
      if(nowDate < this.lastTimeCalled+ microsecondsDiff) {
        console.log('repeat call dialowed at', nowDate, ' last time called at', this.lastTimeCalled);
        return;
      }

      //console.log('upload file proceeded to api at ', nowDate, ', last time called: ', this.lastTimeCalled);

      this.lastTimeCalled=Date.now();
      
      const formData = new FormData();
      const formValue = this.form.value;

      /*const mData = JSON.stringify(this.form.value);
      formData.append('data', mData); 
      //console.log('formData with candidate object:', formData);
      
      if(this.voucherFiles.length > 0) {
        this.voucherFiles.forEach( f => {
          formData.append('file', f, f.name);     //files are downloaded separately at the API
          //console.log('formData with candidate object + attachment:', formData);
        })
      }
      */

        /*var voucherentries: IVoucherEntry[] = this.voucherEntries.value;
        var voucherattachments: IVoucherAttachment[]=this.voucherAttachments.value;

        var newVoucher: IVoucher = {id: formValue.id, partyName: formValue.partyName, 
            accountName:formValue.accountName, voucherNo: formValue.voucherNo, 
            username: formValue.username, reviewedByUsername:formValue.reviewedByUsername,
            divn: formValue.divn, voucherDated: this.form.controls['voucherDated'].value,
            amount: formValue.amount, narration: formValue.narration, cOAId: this.form.controls['cOAId'].value,
            reviewedOn: formValue.reviewedOn,
            voucherEntries: voucherentries
            , voucherAttachments: voucherattachments 
          };
        */
        if(this.voucher!.id === 0) {   //insert new cv
        formData.append('data', JSON.stringify(this.form.value));

        this.service.insertVoucherWithUploads(formData).subscribe({
            next: (event: any) => 
            {
              if (event.type === HttpEventType.UploadProgress)
                this.progress = Math.round(100 * event.loaded / event.total);
              else if (event.type === HttpEventType.Response) 
              {
                  this.message = 'Upload success.';
                  console.log('event', event);

                  this.form.get('voucherNo')?.setValue(event.body.returnInt);
                  //console.log('response returned from api', event);
                  //this.onUploadFinished.emit(event.body);
                  this.returnToCaller();
                  this.toastr.success('the Voucher is created, and assigned Voucher No. '+  event.body.returnInt);
              } 
            },
          error: (err: HttpErrorResponse) => console.log(err)
          });

      } else {

          formData.append('data', JSON.stringify(this.form.value));

          this.service.updateWithFiles(formData).subscribe({
            next: (event: any) => {
              if (event.type === HttpEventType.UploadProgress)
                  this.progress = Math.round(100 * event.loaded / event.total);
              else if (event.type === HttpEventType.Response) {
                  this.message = 'Upload success.';
                
                  this.toastr.success("Voucher Updated and Files uploaded");
                  this.onUploadFinished.emit(event.body);
              }
            },
          error: (err: HttpErrorResponse) => console.log(err)
          });
          
      }
    }

    uploadFinished = (event: any) => { 
      this.response = event; 
    }

    download (i: number) {
      this.attachmentid = this.voucherAttachments.at(i).get('id')?.value;
      if(this.attachmentid===0) return;
  
      var filename = this.voucherAttachments.at(i).get('fileName')?.value;
        this.fileService.download(this.attachmentid).subscribe((event: any) => {
            if(event.type===HttpEventType.DownloadProgress)  
                this.progress = Math.round((100 * event.loaded) / event.total);
            else if(event.type===HttpEventType.Response) {
                this.message='download success';
                this.downloadFile(event, filename);
            }
        });
    }

    private downloadFile = (data: HttpResponse<Blob>, filename: string) => {

      const downloadedFile = new Blob([data.body!], { type: data.body?.type });
      const a = document.createElement('a');
      a.setAttribute('style', 'display:none;');
      document.body.appendChild(a);
      a.download = filename;
      a.href = URL.createObjectURL(downloadedFile);
      a.target = '_blank';
      a.click();
      document.body.removeChild(a);
    }
}
