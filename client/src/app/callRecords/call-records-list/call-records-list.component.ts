import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap, take, tap } from 'rxjs';
import { IUserHistoryBriefDto } from 'src/app/_dtos/admin/useHistoryBriefDto';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserHistoryService } from 'src/app/_services/admin/user-history.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CallRecordsEditModalComponent } from '../call-records-edit-modal/call-records-edit-modal.component';
import { IContactResult } from 'src/app/_models/admin/contactResult';
import { CallRecordParams, ICallRecordParams } from 'src/app/_models/params/callRecordParams';
import { CallRecordStatusReturnDto } from 'src/app/_dtos/admin/callRecordStatusReturnDto';
import { AccountService } from 'src/app/_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-call-records-list',
  templateUrl: './call-records-list.component.html',
  styleUrls: ['./call-records-list.component.css']
})

export class CallRecordsListComponent implements OnInit{
  
  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  callRecords: IUserHistoryBriefDto[]=[];

  oParams = new CallRecordParams();
  totalCount: number=0;

  contactResults: IContactResult[]=[];
  
  pagination: Pagination | undefined;
  userHistoryParams = new CallRecordParams();

  user?: User;
    
  constructor(private service: UserHistoryService, private confirm: ConfirmService,
      private toastr: ToastrService, private bsModalRef: BsModalRef, private accountService: AccountService,
      private bsModalService: BsModalService, private router: Router){
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
      }
  
  ngOnInit(): void {
    this.getCallRecords(this.userHistoryParams);
    this.contactResults = this.service.getContactResults();
  }

  getCallRecords(userHistParams: ICallRecordParams) {
    
    this.service.getUserHistoryPaged(userHistParams).subscribe({
      next: response => {
        if(response.result && response.pagination) {
          this.callRecords = response.result;
          this.pagination = response.pagination;
          this.totalCount = response.count;
        }
      }
    })
  }

  onPageChanged(event: any){
    const params = this.service.getParams() ?? new CallRecordParams();
    
    if (params.pageNumber !== event) {
      params.pageNumber = event.page;
      this.service.setParams(params);
      this.getCallRecords(params);
    }
  }

  deleteCallRecord (callRecordId: number){
    const observableOuter= this.confirm.confirm('confirm delete this Order', 'confirm delete order');
    observableOuter.pipe(
      filter(confirmed => confirmed ),
      switchMap(deleted => this.service.deleteUserHistoryById(callRecordId).pipe(
        catchError(err => {
          console.log('Error in deleting the Call Record', err);
          return of();
        }),
        tap(res => this.toastr.success('deleted the Call Recorrd')),
      )),
      catchError(err => {
        this.toastr.error('Error in getting delete confirmation', err);
        return of();
      })
    ).subscribe(
        () => {
          console.log('delete succeeded');
          this.toastr.success('Call Record deleted');
        },
        (err: any) => {
          console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
      })
  }

  /*addNewCallRecord(event: any, item: IUserHistoryBriefDto) {    //emitted from modal: IUserHistoryItem

    var callRecordItem = event;

    if(callRecordItem === null || callRecordItem === undefined) {
      this.toastr.warning('No Call Record Item returned by modal form');
      return;
    }  

    callRecordItem.personType=item.personType;
    
    const config = {
        class: 'modal-dialog-centered modal-md',
        initialState: {
          callRecord: callRecordItem,
          phoneNo: item.phoneNo,
          email: item.email,
          status: "Not Started",
          candidateName: item.personName,
          categoryRef: item.categoryName,
          contactResults: this.contactResults
        }
      }

      this.bsModalRef = this.bsModalService.show(CallRecordAddModalComponent, config);
      const observableOuter =  this.bsModalRef.content.callRecordAddEvent;
      
      observableOuter.pipe(
      filter((response: any) => response),

      switchMap((response: ICallRecordItem) => {
          var params: ICallRecordParams = {personType:item.personType, subject:'',
            personId:item.personId, incomingOutgoing: response.incomingOutgoing, id:response.id,
            mobileNo:response.phoneNo, emailId:item.email, dateOfContact:response.dateOfContact,
            username:this.user?.userName!, gistOfDiscussions:response.gistOfDiscussions,
            advisoryBy:response.advisoryBy, categoryRef: item.categoryRef, search:'',
            sort: '', status: response.contactResult, pageNumber:1, pageSize:10, statusClass: "",
            nextAction:'', nextActionOn: new Date()};

            return this.service.getOrAddCallRecord(params).pipe(
              catchError(err => {
                return of();
              })
            )
      })
    
    ).subscribe(
        () => {
          this.toastr.success('Call Record Item added', 'success')
        },
        (error: string | undefined) => {
          return this.toastr.error(error, 'Error encountered');
        }
    )
  }
*/
  editCallRecord(callRecord: any, item: IUserHistoryBriefDto) {

    if(callRecord === null) {
      this.toastr.warning('No Call Record object returned by call record item');
      return;
    }  

    const config = {
        class: 'modal-dialog-centered modal-lg',
        initialState: {
          callRecord,
          status: item.status,
          candidateName: item.personName,
          categoryRef: item.categoryName
        }
      }

      this.bsModalRef = this.bsModalService.show(CallRecordsEditModalComponent, config);
      const observableOuter =  this.bsModalRef.content.updateCallRecordEvent;
      
      observableOuter.subscribe({
        next: (callRecReturn: CallRecordStatusReturnDto) => {
          if(callRecReturn.strError === '') {
              item.status=callRecReturn.status;
              var index = this.callRecords.findIndex(x => x.id == item.id);
              if(index >= 0) this.callRecords[index]=item;
              this.toastr.success('The Call Record was updated', 'success')
          } else {
            this.toastr.warning('Failed to update the call Record', 'Failed')
          }
        }, 
        error: (err: any) => this.toastr.error(err, 'Error encountered')
      })
        
   }

   onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.getCallRecords(params);
   }

   onReset() {

   }

  
}
