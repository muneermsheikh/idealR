import { Component, EventEmitter, Input } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { catchError, of, switchMap, tap } from 'rxjs';
import { ISelectionDecision } from 'src/app/_models/admin/selectionDecision';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { SelectionService } from 'src/app/_services/hr/selection.service';

@Component({
  selector: 'app-selection-modal',
  templateUrl: './selection-modal.component.html',
  styleUrls: ['./selection-modal.component.css']
})
export class SelectionModalComponent {

  @Input() updateObj = new EventEmitter();

  sel: ISelectionDecision | undefined;
  candidateName: string='';
  customerName: string='';
  categoryRefAndName = '';

  form: FormGroup = new FormGroup({});

  bsValueDate = new Date();
  
  bsValue = new Date();
  bsRangeValue = new Date();
  maxDate = new Date();
  minDate = new Date();

  statuses = [{status: 'selected'}, 
    {status: 'Rejected - Profile does not match'},
    {status: 'Rejected - High Salary expectation'}, 
    {status: 'Rejected - over-age'},
    {status: 'Rejected - Qualification does not match'},
    {status: 'Rejected - Low Exp'},
    {status: 'Rejected - Exp not relevant'},
    {status: 'Rejected - profile not relevant'}
  ]

  constructor(public bsModalRef: BsModalRef, private confirmService: ConfirmService,
    private toastr: ToastrService, private fb: FormBuilder, private service: SelectionService ) {}

  ngOnInit(): void {
    if(this.sel) {
      //this.sel.selectedOn = new Date(this.sel.selectedOn.getFullYear(), this.sel.selectedOn.getMonth(), this.sel.selectedOn.getDate());
      this.Initialize(this.sel);
    }
    console.log('recd in in modal ngOnINit:', this.sel);
  }

  Initialize(sel: ISelectionDecision) {    
    
    this.form = this.fb.group({
      id:[sel.id],
      candidateId: sel.candidateId ?? 0,
      candidateName:[sel.candidateName],
      applicationNo: [sel.applicationNo ?? 0],
      customerName: [sel.customerName],
      professionName: [sel.professionName],
      cvRefId: [sel.cvRefId],
      orderItemId: sel.orderItemId ?? 0,
      //orderNo: sel.orderNo,
      professionId: sel.professionId ?? 0,
      selectedOn: [sel.selectedOn],
      selectionStatus: [sel.selectionStatus],
      remarks: sel.remarks
    })
  }



  update() {
      
    var formdata = this.form.value;

    this.updateObj.emit(formdata);
    this.bsModalRef.hide();
  }

  deleteSel() {
    if(this.sel) 
    {
      //var ids:number[]=[];
      //ids.push(this.sel.id);

      const observableInner = this.service.deleteSelectionDecisions(this.sel.id);
        
        var messagePrompt = 'This will delete this selection record, along with all related records like ' +
          'Employments and all deployment records.  Or, depending upon settings, the deletion might fail ' +
          'if there are related records';
        const observableOuter = this.confirmService.confirm('Confirm Delete', messagePrompt);

        observableOuter.pipe(
            switchMap(confirmed => observableInner.pipe(
              catchError(err => {
                console.log('Error in deleting the order', err);
                return of();
              }),
              tap(res => this.toastr.success('deleted Selection record')),
            )),
            catchError(err => {
              this.toastr.error('Error in getting delete confirmation', err);
              return of();
            })
          ).subscribe(
              () => {
                console.log('delete succeeded');
                this.toastr.success('Selection record deleted');
              },
              (err: any) => {
                console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
            })
        
        this.bsModalRef.hide();
    }
  }
    
}

function formatDate( dt: Date) : string {
  const day = dt.getDate().toString();
  const month = dt.getMonth().toString();
  const year = dt.getFullYear().toString();

  return `${year}-${month}-${day}`;
}