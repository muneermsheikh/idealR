import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IAssessmentBank } from 'src/app/_models/admin/assessmentBank';
import { IAssessmentBankQ } from 'src/app/_models/admin/assessmentBankQ';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { QbankService } from 'src/app/_services/hr/qbank.service';


@Component({
  selector: 'app-category-qbank-modal',
  templateUrl: './category-qbank-modal.component.html',
  styleUrls: ['./category-qbank-modal.component.css']
})
export class CategoryQBankModalComponent implements OnInit{

  form: FormGroup = new FormGroup({});
  assessment: IAssessmentBank | undefined;
  
  @Output() cancelRegister = new EventEmitter();
  @Output() updateEvent = new EventEmitter<boolean>();
  
  user?: User;
  
  totalMarks = 0;
  
  constructor(private fb: FormBuilder, private confirm: ConfirmService,
    public bsModalService: BsModalService, 
    private service: QbankService, private toastr: ToastrService){}
  
  ngOnInit(): void {

      if(this.assessment) {
        this.CreateAndInitializeFormArray(this.assessment!);
        this.calcualteTotals();  
      }
  }

  CreateAndInitializeFormArray(item: IAssessmentBank) {

    if(item) {
      this.form = this.fb.group({
        id: [item!.id ?? 0],
        professionId: [item.professionId, Validators.required],
        professionName: [item.professionName, Validators.required],
       
        assessmentBankQs: this.fb.array(
            item.assessmentBankQs.map(q => (
              this.fb.group({
                  id: [q!.id ?? 0],
                  assessmentBankId: q.assessmentBankId,
                  assessmentParameter: [q.assessmentParameter, Validators.required],
                  qNo: [q.qNo, [Validators.required, Validators.min(1), Validators.max(25)]],
                  isStandardQ: [q.isStandardQ],
                  isMandatory: [q.isMandatory, Validators.required],
                  question: [q.question, Validators.required],
                  maxPoints: [q.maxPoints, Validators.required]
              })
            ))
        )
    })
    }
    
  }

  get assessmentBankQs() : FormArray {
    return this.form.get("assessmentBankQs") as FormArray
  }

  newQ(): FormGroup {
    var maxSrNo = this.assessmentBankQs.length===0 ? 1 
    : Math.max(...this.assessmentBankQs.value.map((x:IAssessmentBankQ) => x.qNo))+1;

    return this.fb.group({
          id: 0,
          assessmentBankId: this.form.get('id'),
          assessmentParameter: ['', Validators.required],
          qNo: [maxSrNo, Validators.required],
          subject: ['', Validators.required],
          question: ['', Validators.required],
          maxPoints: [0, Validators.required],
          isMandatory: [false, Validators.required]
    })
  }

  addQ() {
    this.assessmentBankQs.push(this.newQ())
  }

  removeQ(itemIndex: number) {
      this.assessmentBankQs.removeAt(itemIndex);
  }

  cancel() {
    this.cancelRegister.emit(false);
    this.bsModalService.hide();
  }

  updateQ() {

    this.service.update(this.form.value).subscribe({
      next: (succeeded: boolean) => {
        if(succeeded) {
            this.updateEvent.emit(true);
            this.toastr.success('Order Assessment Item successfully updated', 'success');
            this.bsModalService.hide();
        } else {
          this.toastr.warning('failed to update the order assessment item', 'Failed to update');
        }
        }
      })
  }

  calcualteTotals() {
    this.totalMarks = this.form.get('assessmentBankQs')?.value.map((x: IAssessmentBankQ) => x.maxPoints).reduce((a: number, b: number) => +a + +b);
  }

}
