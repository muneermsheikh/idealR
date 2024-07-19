import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IVoucherToAddNewPaymentDto, VoucherToAddNewPaymentDto } from 'src/app/_dtos/finance/voucherToAddNewPaymentDto';
import { ICOA } from 'src/app/_models/finance/coa';
import { User } from 'src/app/_models/user';
import { COAService } from 'src/app/_services/finance/coa.service';

@Component({
  selector: 'app-add-payment-modal',
  templateUrl: './add-payment-modal.component.html',
  styleUrls: ['./add-payment-modal.component.css']
})
export class AddPaymentModalComponent implements OnInit {

  @Output() paymentVoucherEvent = new EventEmitter<IVoucherToAddNewPaymentDto|null>();
  
  title: string='';
  accountName: string='';
  
  currentBalance: number=0;

  paymentAmount: number=0;
  bankAndCashAccounts: ICOA[]=[];
  creditAccount: number=0;
  paymentDated: Date = new Date();
  drEntryRequiresApproval: boolean=false;
  narration: string='';

  drAccountName: string='';
  crAccountName: string='';

  debitAccountId: number=0;

  bsValue = new Date();
  bsValueDate = new Date();
  user?: User;
  returnUrl = '';
  
  coas: ICOA[]=[];

  newVoucher = new VoucherToAddNewPaymentDto()
  
  constructor(public bsModalRef: BsModalRef, private toastr: ToastrService, private service: COAService) { }

  ngOnInit(): void {
    this.service.getGroupOfCOAs("banks").subscribe({
      next: responses => this.bankAndCashAccounts = responses,
      error: err => this.toastr.error('error in getting cashandbank', err)
    });
  }

  addNewVoucher() {
    if(this.paymentAmount <=0 || this.creditAccount===0 ) {
      this.toastr.warning('invalid inputs');
      return;
    }
    this.newVoucher = {
      partyName:'',
      debitCOAId:this.debitAccountId, creditCOAId:this.creditAccount,
      amount:this.paymentAmount, paymentDate: this.paymentDated, narration: this.narration,
      drEntryRequiresApproval:this.drEntryRequiresApproval, 
      debitAccountName:this.drAccountName, creditAccountName: this.crAccountName};

    this.paymentVoucherEvent.emit(this.newVoucher);
    this.bsModalRef.hide();
  }

  onBankCashAccountChanged() {
    this.drEntryRequiresApproval = this.debitAccountId===2;
  }

  updateDrAccountName(event: any) {
    this.newVoucher.debitAccountName=event.accountName;
  }

  updateCrAccountName(event: any) {
    this.newVoucher.creditAccountName=event.accountName;
  }

  close() {
    this.toastr.warning('aborted');
    console.log('aborted');
    this.paymentVoucherEvent.emit(null);
    this.bsModalRef.hide();
  }

}
