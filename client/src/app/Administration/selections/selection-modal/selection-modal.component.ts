import { Component, EventEmitter, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap, tap } from 'rxjs';
import { ISelDecisionDto } from 'src/app/_dtos/admin/selDecisionDto';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { SelectionService } from 'src/app/_services/hr/selection.service';

@Component({
  selector: 'app-selection-modal',
  templateUrl: './selection-modal.component.html',
  styleUrls: ['./selection-modal.component.css']
})
export class SelectionModalComponent {

  @Input() updateObj = new EventEmitter();

  sel: ISelDecisionDto | undefined;
  
  form: FormGroup = new FormGroup({});

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
    private toastr: ToastrService, private fb: FormBuilder, private service: SelectionService) {}

  ngOnInit(): void {
    if(this.sel) this.Initialize(this.sel);
  }

  Initialize(sel: ISelDecisionDto) {
    
    this.form = this.fb.group({
      selDecisionId:[sel.selDecisionId],
      candidateName:[sel.candidateName],
      applicationNo: [sel.applicationNo],
      customerName: [sel.customerName],
      categoryRef: [sel.categoryRef],
      referredOn: [sel.referredOn],
      selectedOn: [sel.selectedOn],
      selectionStatus: [sel.selectionStatus]
      //, rejectionReason: [sel.rejectionReason]
    })
  }

  update() {
      this.updateObj.emit(this.sel);
      this.bsModalRef.hide();
  }

  deleteSel() {
    if(this.sel) 
    {
        const observableInner = this.service.deleteSelectionDecision(this.sel.selDecisionId);
        
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
