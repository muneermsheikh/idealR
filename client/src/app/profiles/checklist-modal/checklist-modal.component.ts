import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { IChecklistHRDto } from 'src/app/_dtos/hr/checklistHRDto';
import { IChecklistHR } from 'src/app/_models/hr/checklistHR';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { ChecklistService } from 'src/app/_services/hr/checklist.service';

@Component({
  selector: 'app-checklist-modal',
  templateUrl: './checklist-modal.component.html',
  styleUrls: ['./checklist-modal.component.css']
})
export class ChecklistModalComponent implements OnInit {
  
  @Input() updateObj = new EventEmitter();    //emits string.  blank string is success, else the string is err description

  chklst?: IChecklistHRDto;
  form: FormGroup = new FormGroup({});
  username = '';

  checklistForm?: NgForm;

  constructor(public bsModalRef: BsModalRef, private confirmService: ConfirmService, 
      private toastr: ToastrService, private checklistService: ChecklistService, 
      private fb: FormBuilder) { }

  ngOnInit(): void {
    if(this.chklst) this.Initialize(this.chklst);
  }

  verifyData() {

      var errString = '';

      if(this.chklst) 
      {
        errString = (this.form.value.charges > 0 
            && this.form.get('charges')?.value !== this.form.get('chargesAgreed')?.value  // this.form.value.chklst!.chargesAgreed 
            && !this.form.value.exceptionApproved )
            ? "exception approved must accompany exception made by and approved on" : "";

        var nonMatching = this.form.value.checklistHRItems // this.chklst?.checklistHRItems
            .filter((x:any) => x.mandatoryTrue && !x.accepts)
            .map((x: any) => x.parameter).join(', ');
 
        if (nonMatching!=='') 
              errString += "Mandatory parameters " + nonMatching + "  not accepted";
            
      } else {
          this.chklst!.checklistedOk=true;
          this.chklst!.checkedOn=new Date();
        }
    
        return errString;
  
    }
    
  ChargesDoubleClicked() {

    if(this.chklst !== undefined && this.form.value.chargesAgreed === 0) {
      this.form.value.chargesAgreed=this.chklst.charges;
      this.form.get('chargesAgreed')?.setValue(this.chklst.charges);
    }
  }

  checkednoerror() {
    if(this.chklst?.exceptionApproved && (this.chklst.exceptionApprovedBy==='' || this.chklst.exceptionApprovedOn.getFullYear() < 2000)) {
      //this.toastr.warning('exception approved must accompany exception made by and approved on');
      return false;
    }
    var nonMatching = this.chklst?.checklistHRItems.filter(x => x.mandatoryTrue && !x.response).map(x => x.parameter).join(', ');
    if (nonMatching!=='') {
      this.confirmService.confirm("Mandatory parameters not accepted", "Following mandatory parameters not accepted" + nonMatching,
        "Update as it is", "Cancel and edit response").subscribe(result => {
          if (result) {
            this.chklst!.checklistedOk=false;
            this.chklst!.checkedOn=new Date();
            console.log('result is:', result);
            return true;
          } else {
            console.log('result is:', result);
            return false;
          }
        })
      
      //this.toastr.warning('error - flg Mandatory parameters have not been accepted :', nonMatching );
      //return false;
    } else {
      this.chklst!.checklistedOk=true;
      this.chklst!.checkedOn=new Date();
    }
    return true;
  }

  Edited() 
  {
    var err = this.verifyData();

    if(err !== '') {
      this.toastr.warning(err);
      console.log('checklist data not verified', err);
      return;
    }
    //no error
    
    this.form.get('checklistedOk')?.setValue(true);
    this.form.get('userLoggedName')?.setValue(this.username);
    
    var formdata = this.form.value;

    if(formdata.id ===0) {
      this.checklistService.saveNewChecklistHR(formdata).subscribe({
        next: (response: IChecklistHR) => {
          if(response===null) {
            this.toastr.warning('Failed to insert the new checklist', 'Failure');
          } else {
            this.toastr.success('saved the new checklist', 'Success');
            this.updateObj.emit('');    //success
            this.bsModalRef.hide();
          }
        },
        error: (err: any) => this.toastr.error(err, 'Error in saving the checklist')
      })
    } else {
      this.checklistService.updateChecklist(formdata).subscribe({
        next: () => {
          this.updateObj.emit("");
          this.bsModalRef.hide();
        },
        error: err => {
          console.log('failed to update the Checklist data', err);
          this.updateObj.emit('failed to update the Checklist ' + err);
          this.bsModalRef.hide();
        }
      })
    }
      
    
  }

  //formArrays
  
  Initialize(chk: IChecklistHRDto) {

    this.form = this.fb.group({
      id:chk.id, candidateId: chk.candidateId, applicationNo: chk.applicationNo,
      candidateName: chk.candidateName, categoryRef: chk.categoryRef, OrderItemId: chk.orderItemId,
      userLoggedName: chk.userLoggedName, orderItemId: chk.orderItemId, hrExecUsername: chk.hrExecUsername,
      checkedOn: chk.checkedOn, charges: chk.charges, chargesAgreed: chk.chargesAgreed,
      exceptionApproved: chk.exceptionApproved, exceptionApprovedOn: chk.exceptionApprovedOn,
      exceptionApprovedBy: chk.exceptionApprovedBy, checklistedOk: chk.checklistedOk,
      assessmentIsNull: chk.assessmentIsNull,

      checklistHRItems: this.fb.array(
        chk.checklistHRItems.map(x => (
          this.fb.group({
            id: x.id, 
            checklistHRId: [x.checklistHRId, Validators.required],
            srNo: x.srNo,
            parameter: [x.parameter, Validators.required],
            accepts: [x.accepts],
            exceptions: x.exceptions,
            mandatoryTrue: [x.mandatoryTrue],
            response: x.response
          })
        ))
      )
  })
  }

  get checklistHRItems(): FormArray {
    return this.form.get('checklistHRItems') as FormArray;
  }


  addChecklistItem() {
    this.checklistHRItems.push(this.newChecklistItem());
  }

  newChecklistItem(): FormGroup {
    return this.fb.group({
      id: 0, 
      checklistHRId:this.chklst?.id,
      srNo: [0, Validators.required],
      parameter: ['', Validators.required],
      accepts: [false],
      exceptions: '',
      mandatoryTrue: false
    })
  }

  removeChecklistItem(i: number) {
    this.checklistHRItems.removeAt(i);
    this.checklistHRItems.markAsDirty();
    this.checklistHRItems.markAsTouched();
  }

  
  deleteChecklist() {
    var id=this.chklst!.id;
    var confirmMsg = 'confirm delete this Checklist. ' +
      'WARNING: this cannot be undone';

    const observableInner = this.checklistService.deleteChecklistHR(id);
    const observableOuter = this.confirmService.confirm('confirm Delete', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => {
          return observableInner
        })
    ).subscribe(response => {
      if(response) {
        this.toastr.success('Checklist deleted', 'deletion successful');
        console.log('subscribed response:', response);
        this.bsModalRef.hide();
      } else {
        this.toastr.error('Error in deleting the checklist', 'failed to delete')
      }
      
    });

   
  }

  
}
