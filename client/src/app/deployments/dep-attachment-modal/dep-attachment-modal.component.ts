import { Component, EventEmitter, Input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IDeployStatusAndName } from 'src/app/_models/masters/deployStage';
import { IDep } from 'src/app/_models/process/dep';
import { IDepItem } from 'src/app/_models/process/depItem';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { DeployService } from 'src/app/_services/deploy.service';

@Component({
  selector: 'app-dep-attachment-modal',
  templateUrl: './dep-attachment-modal.component.html',
  styleUrls: ['./dep-attachment-modal.component.css']
})
export class DepAttachmentModalComponent {

  dep: IDep | undefined;      //emitted from Deploy-Line component
  @Input() updateDep = new EventEmitter<IDep>();

  depItemOfAttachment: IDepItem|undefined;
  indexId = 0;

  ecnr=false;

  form: FormGroup = new FormGroup({});

  candidateName: string = '';
  categoryRef = '';
  companyName = '';

  filesToUpload: File[]=[];
  lastTimeCalled: number=0;

  bsValue = new Date();
  bsRangeValue= new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();


  //depStatuses: IDeployStage[]=[];
  depStatusAndNames: IDeployStatusAndName[]=[];
  
  constructor(public bsModalRef: BsModalRef, private toastr:ToastrService, 
    private confirm: ConfirmService, private fb: FormBuilder, 
    private service: DeployService) {}
  
  ngOnInit(): void {
    if(this.dep) this.InitializeForm(this.dep);
  }

  InitializeForm(dep: IDep) {

    this.form = this.fb.group({
      
        id: [dep.id],
        cVRefId: [dep.cvRefId],   
        orderItemId: [dep.orderItemId],
        customerId: dep.customerId,
        selectedOn: dep.selectedOn,
        currentStatus: dep.currentStatus,
        ecnr: dep.ecnr,
    
        depItems: this.fb.array(
          dep.depItems.map(x => (
            this.fb.group({
                id: x.id,
                depId: x.depId,
                transactionDate: [x.transactionDate, Validators.required],
                sequence: [x.sequence, Validators.required],
                fullPath: x.fullPath
            })
          ))
        )
    })
  }

  get depItems(): FormArray {
    return this.form.get("depItems") as FormArray
  }

  onFileInputChange(event: Event, index: number) {
    const target = event.target as HTMLInputElement;
    var depitem = this.depItems.at(index).value;
    var seq = depitem.sequence;
    if(!this.sequenceValidForUpload(seq)) {
      this.toastr.warning('The item has a sequence that does not need any file attachment', 'Invalid Item for Upload');
      return;
    }
    this.filesToUpload = [];
    if(depitem.fullPath !== null && depitem.fullPath !== '') {
      this.toastr.warning('one file is already listed as uploaded.  If you want to replace earlier upload, then delete the upload in order to add another upload', 'Only 1 file can be uploaded for a transaction');
      return;
    }

    this.depItemOfAttachment = depitem;
    this.indexId = index;

    const files = target.files as FileList;
    const f = files[0];
      depitem.fullPath = f.name;
      this.filesToUpload.push(f);
    
    return;
  }

  uploadAttachment() {
    var microsecondsDiff: number= 28000;
    var nowDate: number =Date.now();
    
    if(nowDate < this.lastTimeCalled+ microsecondsDiff) return;
    
    this.lastTimeCalled=Date.now();
    let formData = new FormData();
    
    if(this.filesToUpload.length > 0) {
      this.filesToUpload.forEach(f => {
        formData.append("file", f, f.name);
      })
    }

    formData.append("data", JSON.stringify(this.depItemOfAttachment));

    this.service.uploadAttachmentForItem(formData).subscribe({
      next: (response:string) => {
        this.depItems.at(this.indexId).get('fullPath')?.setValue(response);
        this.toastr.success('file uploaded and attachment path updated', 'Success')
      },
      error: (err: any) => this.toastr.error(err.error.details, 'error while uploading the attachment')
    })

  }

  sequenceValidForUpload(seq: number) {

    switch(seq) {
      case 100: case 150: case 400: case 700: case 1100:case 1300:
        return true;
        break;
      default:
        return false;
    }

  }

  download(index: number) {
    var fullpath = this.depItems.at(index).get('fullPath')?.value;
    if(fullpath===null || fullpath==='') {
      this.toastr.warning('No file upload data available', 'Bad Request');
      return;
    }

    this.service.downloadAttachment(fullpath).subscribe({
      next: (blob: Blob) => {
        const a = document.createElement('a');
        const objectUrl = URL.createObjectURL(blob);
        a.href = objectUrl;
        var i=fullpath.lastIndexOf('\\');
        if(i !== -1) {
          a.download = fullpath.substring(i+1);
        } else {
          a.download = 'filename.ext'
        }
        
        a.click();
        URL.revokeObjectURL(objectUrl);
      }
      , error: (err: any) => this.toastr.error(err.error.details, 'Error encountered while downloading the file ')
    })
  }

  deleteAttachment(index: number) {
    var fullpath = this.depItems.at(index).get('fullPath')?.value;
    if(fullpath===null || fullpath==='') {
      this.toastr.warning('No file upload data available', 'Bad Request');
      return;
    }


  }
}
