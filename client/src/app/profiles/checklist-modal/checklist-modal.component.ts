import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IChecklistHRDto } from 'src/app/_dtos/hr/checklistHRDto';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { ChecklistService } from 'src/app/_services/hr/checklist.service';

@Component({
  selector: 'app-checklist-modal',
  templateUrl: './checklist-modal.component.html',
  styleUrls: ['./checklist-modal.component.css']
})
export class ChecklistModalComponent implements OnInit {
  
  @Input() updateObj = new EventEmitter();    //eits string.  blank string is success, else the string is err description
  
  chklst?: IChecklistHRDto;
 
  //checklistedOk=false;

  checklistForm?: NgForm;

  constructor(public bsModalRef: BsModalRef, private confirmService: ConfirmService, 
      private toastr: ToastrService, private checklistService: ChecklistService) { }

  ngOnInit(): void {
    
  }

  verifyData() {

      var errString = '';

      if(this.chklst) 
      {
        errString = (this.chklst.exceptionApproved 
            && this.chklst.charges !== this.chklst.chargesAgreed && this.chklst.charges > 0)
            ? "exception approved must accompany exception made by and approved on" : "";

        var nonMatching = this.chklst?.checklistHRItems.filter(x => x.mandatoryTrue && !x.accepts).map(x => x.parameter).join(', ');
        if (nonMatching!=='') 
              errString += "Mandatory parameters " + nonMatching + "  not accepted";
            
      } else {
          this.chklst!.checklistedOk=true;
          this.chklst!.checkedOn=new Date();
        }
    
        return errString;
  
    }
    
  

  ChargesSingleClicked() {
    console.log('single click');
  }

  ChargesDoubleClicked() {
    console.log('doube cicked, value:', this.chklst?.charges, 'charges agreed', this.chklst?.chargesAgreed);
    if(this.chklst?.chargesAgreed === 0) {
      this.chklst.chargesAgreed=this.chklst.charges;
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
    var formdata = this.checklistForm!.value;

    this.toastr.info('entered Edited', formdata);

    if(!this.chklst) {
      this.toastr.info('chklist object null');
      console.log('modal object null');
      return;
    }
    
      var err = this.verifyData();

      if(err !== '') {
        this.toastr.warning(err);
        console.log('checklist data not verified', err);
        return;
      }
      //no error
      this.chklst.checklistedOk=true;
      this.chklst.checkedOn=new Date();

      this.checklistService.updateChecklist(this.chklst).subscribe({
        next: () => this.updateObj.emit(""),
        error: err => {
          console.log('failed to update the Checklist data', err);
          this.updateObj.emit('failed to update the Checklist ' + err);
        }
      })
        
        this.bsModalRef.hide();
    
  }

}
