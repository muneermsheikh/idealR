import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IDeployStatusAndName } from 'src/app/_models/masters/deployStage';
import { IDep } from 'src/app/_models/process/dep';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { DeployService } from 'src/app/_services/deploy.service';

@Component({
  selector: 'app-deploy-edit-modal',
  templateUrl: './deploy-edit-modal.component.html',
  styleUrls: ['./deploy-edit-modal.component.css']
})
export class DeployEditModalComponent implements OnInit {
  
  @Input() dep: IDep | undefined;      //emitted from Deploy-Line component
  @Input() updateDep = new EventEmitter<IDep>();

  ecnr=false;

  form: FormGroup = new FormGroup({});

  candidateName: string = '';
  categoryRef = '';
  companyName = '';

  //depStatuses: IDeployStage[]=[];
  depStatusAndNames: IDeployStatusAndName[]=[];
  
  bsValue = new Date();
  bsRangeValue= new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();

  constructor(public bsModalRef: BsModalRef, private toastr:ToastrService, 
    private confirmService: ConfirmService, private fb: FormBuilder 
    , private service: DeployService, private activatedRoute: ActivatedRoute) {
      
      /*this.service.getDepStatusAndNames().subscribe({
        next: (response:IDeployStatusAndName[]) => this.depStatusAndNames = response
      })
      */
    }

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
                nextSequence: x.nextSequence,
                nextSequenceDate: x.nextSequenceDate,
                fullPath: x.fullPath
            })
          ))
        )
    })
  }

  get depItems(): FormArray {
    return this.form.get("depItems") as FormArray
  }

  newDepItem(): FormGroup {

    return this.fb.group({
      id: 0,
      depId: this.form.get("id")?.value,
      transactionDate: [new Date(), Validators.required],
      sequence: [this.depItems.value[this.depItems.length].map((x:any) => x.nextSequence), Validators.required],
      nextSequence: this.service.getNextSequence(this.depItems.value[this.depItems.length].map((x:any) => x.nextSequence), this.ecnr),
      nextSequenceDate: this.service.getNextStageDate(this.depItems.value[this.depItems.length].map((x:any) => x.nextSequence)),
      fullPath: ''
    })
    

  }

  addDepItem() {
    this.depItems.push(this.newDepItem);
  }

  removeDepItem(index: number) {
    var msg ="This will also delete ";
    msg += +index+1;
    msg += " transactions that are after the date of this transaction. " +
        " THE DELETION WILL TAKE EFFECT ONLY AFTER YOU UPDATE THIS FORM"
    
    this.confirmService.confirm("Confirm Delete", msg).subscribe({next: confirmed => {
        if(confirmed) {
          for(let i=+index+1; i--; i <= index) {   //depItems is in desc order
            console.log(index, i);
            this.depItems.removeAt(i);
          }
        }
      }})

  }

  updateDeployment() {
      
    /*var err='';
    var items = this.form.get('depItems');
    const depitems = items!.pipe(map((x:IDepItem) => x.sort((a: { transactionDate: number; }, b: { transactionDate: number; }) => a.transactionDate - b.transactionDate)));

    for(let i=1; i< depitems.length; i++) {
      const tDate1 = depitems.at(i-1).transactionDate;
      const tDate2 = depitems.at(i).transactionDate;
      const seq1 = this.service.getDeployStatusOfASequence(depitems.at(i-1).sequence);
      const seq2 = this.service.getDeployStatusOfASequence(depitems.at(i).sequence);
  
      if(tDate1 > tDate2) {
        console.log(seq1 + ' at ' +  tDate1 +  ' is earlier than ' + seq2 + ' at ' + tDate2);  
        
        err = seq1 + ' is prior to sequence ' + seq2 + ' but is dated later';
          break;
        }
    }
    if(err !== '') {
      this.toastr.warning(err, 'transaction dates not in order');
      return;
    }
      */
    if(this.form.dirty) {
      this.toastr.info('toaster info', 'testing');
      var formdata = this.form.value;
      this.updateDep.emit(formdata);
      this.bsModalRef.hide();
    } else {
      this.toastr.warning('form is not dirty', 'no changes made', {"closeButton": true, "timeOut": 0, "extendedTimeOut":0});
    }
  }

  download(index: number) {
    var depitem = this.depItems.at(index).value;
    var filename = depitem.fullPath;
    if(filename==='') {
      this.toastr.warning('this transaction has no file uploaded', 'Invalid selection');
      return;
    }

    this.service.downloadAttachment(filename).subscribe({
      next: (blob: Blob) => {
        const a = document.createElement('a');
        const objectUrl = URL.createObjectURL(blob);
        a.href = objectUrl;
        var i=filename.lastIndexOf('\\');
        if(i !== -1) {
          a.download = filename.substring(i+1);
        } else {
          a.download = 'filename.ext'
        }
        
        a.click();
        URL.revokeObjectURL(objectUrl);
      }
      , error: (err: any) => this.toastr.error(err.error.details, 'Error encountered while downloading the file ')
    })

  }

  /* TODO - IMPLEMENT THIS
  deploySequenceChanged(sequence: number, newSequence: number, ecnr: boolean) {

    var sequenceChangeIsOk = this.service.NextSequenceProposedIsOkay(sequence, newSe)
  }
  */

  
  close() {
    this.bsModalRef.hide();
  }

 
}
