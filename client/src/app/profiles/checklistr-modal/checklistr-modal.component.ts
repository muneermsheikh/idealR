import { Component, OnInit } from '@angular/core';
import { Form, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IChecklistHRDto } from 'src/app/_dtos/hr/checklistHRDto';

@Component({
  selector: 'app-checklistr-modal',
  templateUrl: './checklistr-modal.component.html',
  styleUrls: ['./checklistr-modal.component.css']
})
export class ChecklistrModalComponent implements OnInit {

  checklist: IChecklistHRDto | undefined;

  form: FormGroup = new FormGroup({});

  constructor(private fb: FormBuilder){}

  ngOnInit(): void {
    
  }

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
            mandatoryTrue: x.mandatoryTrue
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
      checklistHRId:this.checklist?.id,
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
}
