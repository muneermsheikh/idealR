import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IPendingDebitApprovalDto } from 'src/app/_dtos/finance/pendingDebitApprovalDto';
import { IUpdatePaymentConfirmationDto, UpdatePaymentConfirmationDto } from 'src/app/_dtos/finance/updatePaymentConfirmationDto';
import { Pagination } from 'src/app/_models/pagination';
import { ParamsCOA } from 'src/app/_models/params/finance/paramsCOA';
import { prospectiveSummaryParams } from 'src/app/_models/params/hr/prospectiveSummaryParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { ConfirmReceiptsService } from 'src/app/_services/finance/confirm-receipts.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-confirm-fund-receipt',
  templateUrl: './confirm-fund-receipt.component.html',
  styleUrls: ['./confirm-fund-receipt.component.css']
})
export class ConfirmFundReceiptComponent implements OnInit {

  pending: IPendingDebitApprovalDto[]=[];
  pendingSelected: IPendingDebitApprovalDto[]=[];
  pagination: Pagination | undefined;
  totalCount:number=0;

  user?: User;
  bolNavigationExtras:boolean=false;
  returnUrl: string='';
  pParams: ParamsCOA=new ParamsCOA();

  drEntryReviewedOn: Date=new Date();
  drEntryApproved: boolean=false;

  form: FormGroup= new FormGroup({});
  bsValueDate = new Date();

  MESSAGE_TYPEID_REQUEST_PAYMENT_CONFIRMATION=24;
  MESSAGE_TYPEID_CONFIRM_PAYMENT_RECEIVED=25;
  MESSAGE_TYPEID_CONFIRM_PAYMENT_NOT_RECEIVED=26;

  MESSAGE_POST_ACTION=5;  //compose and send Msg;  then send push notification

  constructor(private service: ConfirmReceiptsService,
    private toastr: ToastrService,
    private router: Router, private accountService: AccountService,
    private confirmService: ConfirmService,
    private activatedRoute: ActivatedRoute,
    private fb: FormBuilder,
    //private pushNotice: PushNotificationService,
    private msgService: MessageService) {
      let nav: Navigation | null= this.router.getCurrentNavigation();

        if (nav?.extras && nav.extras.state) {
            this.bolNavigationExtras=true;
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            var usr = this.accountService.currentUser$.subscribe({
              next: response => this.user=response!
            })
        }
     }

  ngOnInit(): void {

    this.pParams = new ParamsCOA();
    this.getPendings();

  }

  getPendings() {
    this.service.setParams(this.pParams);

      this.service.getDebitApprovalsPending().subscribe({
        next: response => {
          this.pending = response.result;
          this.pagination = response.pagination;
          this.totalCount = response.count;
        }
      })
  }

  update() {
    var selected = this.pending.filter(x => x.selected===true);
    console.log('all selected', selected);

    if(this.pendingSelected.length > 0) {
      this.service.updatePaymentReceipts(this.pendingSelected).subscribe(response => {
        if(response) {
          this.toastr.success('selected payments confirmed as received');
        } else {
          this.toastr.warning('failed to register the payment confirmations');
          return;
        }
      }, error => {
        this.toastr.error('failed to register the payment confirmations');
        return;
      })
    }
  }
    
  selectedClicked(item: IPendingDebitApprovalDto) {
    
    var index = this.pending.findIndex(x => x.id === item.id);
    console.log('item:', item, 'index:', index);
    
    if(item.drEntryApproved===true) {
      if(index ===-1) {
        item.drEntryApprovedByUsername=this.user!.userName;
        item.drEntryApprovedOn=new Date();
        this.pendingSelected.push(item);
      }
    } else {
      this.pendingSelected.splice(index,1);
    }

  }

  PageChanged(event: any){
      const params = this.service.getParams();
      if (params.pageNumber !== event.page) {
        params.pageNumber = event.page;
        this.service.setParams(params);

        this.getPendings();
      }
    }


  close() {
    
  }
}
