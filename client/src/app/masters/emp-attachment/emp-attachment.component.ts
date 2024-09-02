import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { IEmployee } from 'src/app/_models/admin/employee';
import { IEmployeeAttachment } from 'src/app/_models/admin/employeeAttachment';
import { IUserAttachment } from 'src/app/_models/hr/userAttachment';
import { EmployeeService } from 'src/app/_services/admin/employee.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { FileService } from 'src/app/_services/file.service';

@Component({
  selector: 'app-emp-attachment',
  templateUrl: './emp-attachment.component.html',
  styleUrls: ['./emp-attachment.component.css']
})
export class EmpAttachmentComponent implements OnInit{

  empAttachments: IEmployeeAttachment[]  = [];

  //from calling program
  empId = 0;
  empName = '';
  position = '';


  form: FormGroup = new FormGroup({});

  filesToUpload: File[]=[];
  userFiles: File[]=[];
  public progress: number=0;
  lastTimeCalled=0;

  @Output() updateEvent = new EventEmitter<boolean[]>();
    
    title: string='';

    updateClicked() {
      
      this.updateEvent.emit(this.form.value);
        this.bsModalRef.hide();
      
    }

  constructor(private fb: FormBuilder, public bsModalRef: BsModalRef, private confirm: ConfirmService,
    private toastr: ToastrService, private downloadService: FileService, private service: EmployeeService){}

  
  fileTypes = [
    {"fileType": "CV"}, {"fileType": "Certificate"}, {"fileType": "Aadhar"}, {"fileType": "Photograph"}
  ]

  ngOnInit(): void {
    this.service.getEmployeeAttachments(this.empId).subscribe({
      next: (response: IEmployeeAttachment[]) => this.empAttachments = response,
      error: (err: any) => this.toastr.error(err.error.details, 'Error encountered')
    })

    this.initializeForm(this.empAttachments);
  }

  initializeForm(emps: IEmployeeAttachment[]) {

      this.form = this.fb.group({
       
        employeeAttachments: this.fb.array(
          emps.map(n => (
            this.fb.group({
              id: [n.id], employeeId: [n.employeeId, Validators.required], fileName: [n.fileName, Validators.required],
              fileType: [n.fileType, Validators.required], fullPath: [n.fullPath, Validators.required]
            })
          ))
        )
        
      })
  }


  get employeeAttachments(): FormArray {
    return this.form.get("employeeAttachments") as FormArray
  }

  newEmployeeAttachment(): FormGroup {
    return this.fb.group({
        id: 0, employeeId: [this.empAttachments[0]?.id, Validators.required],
        fileName: ['', Validators.required], fileType: ['', Validators.required],
        fullPath: ['']
    })
  }

  addEmployeeAttachment() {
    this.employeeAttachments.push(this.newEmployeeAttachment());
  }

  removeEmployeeAttachment(index: number) {
    this.employeeAttachments.removeAt(index);
    this.employeeAttachments.markAsDirty();
  }

  download(index: number) {
    var attachment = this.employeeAttachments.at(index).value;
    if(attachment.id ===0) {
      this.toastr.warning('this item has no primary key value');
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

  newEmployeeAttachmentWithFile(f:File): FormGroup {
    return this.fb.group({
      id:0,
      employeeId: this.empAttachments===undefined ? 0 : this.empAttachments[0]?.id,
      attachmentType: ['', Validators.required],
      fileName: [f.name, Validators.required],
      fileType: '',
      fullPath: ''
    })
  }

  onFileInputChange(event: Event) {
    const target = event.target as HTMLInputElement;
    //var empAttachment = this.employeeAttachments.at(index).value;
    this.filesToUpload = [];

    const files = target.files as FileList;
    const f = files[0];
    //empAttachment.fileName = f.name;
    this.filesToUpload.push(f);
    this.employeeAttachments.push(this.newEmployeeAttachmentWithFile(f));
  }

  update() {
    var microsecondsDiff: number= 28000;
    var nowDate: number =Date.now();
    
    if(nowDate < this.lastTimeCalled+ microsecondsDiff) return;
    
    this.lastTimeCalled=Date.now();
    var formData = new FormData();
    const formValue = this.form.value;

    formData.append('data', formValue);
    
    if(this.userFiles.length > 0) {
      this.userFiles.forEach(f => {
        formData.append('file', f, f.name);
      })
    }

    this.service.updateOrAddEmployeeAttachments(formData).subscribe({
      next: (response: boolean[]) => {
        /*response.forEach(x => {
          var index = this.employeeAttachments.value.findIndex((y:any) => y.fileName===x.fileName);
          if(index !==-1) this.employeeAttachments.at(index).get('fullPath')?.setValue(x.fullPath);
        })*/
       this.toastr.info('suuccess');
      }
    })
  }

}
