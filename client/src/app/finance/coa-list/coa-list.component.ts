import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { coa, ICOA } from 'src/app/_models/finance/coa';
import { Pagination } from 'src/app/_models/pagination';
import { ParamsCOA } from 'src/app/_models/params/finance/paramsCOA';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { COAService } from 'src/app/_services/finance/coa.service';
import { CoaEditModalComponent } from '../coa-edit-modal/coa-edit-modal.component';
import { filter, switchMap } from 'rxjs';
import { InputModalComponent } from 'src/app/modals/input-modal/input-modal.component';
import { DateInputRangeModalComponent } from 'src/app/modals/date-input-range-modal/date-input-range-modal.component';

@Component({
  selector: 'app-coa-list',
  templateUrl: './coa-list.component.html',
  styleUrls: ['./coa-list.component.css']
})
export class CoaListComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;

  user?: User;
  sParams = new ParamsCOA();
  totalCount: number=0;
  coas: ICOA[]=[];
  pagination: Pagination | undefined;

  bolNavigationExtras:boolean=false;
  returnUrl: string='';
  bsModalRef?: BsModalRef;
  inputApplicationNo=0;

  lastExclude='';

  sortOptions = [
    {name:'By Account Name Asc', value:'name'},
    {name:'By Account Name Desc', value:'namedesc'},
    {name:'By Account Type Asc', value:'type'},
    {name:'By Account Type Desc', value:'typedesc'},
    {name:'By Division Asc', value:'divn'},
    {name:'By Division Desc', value:'divndesc'}
  ]

  constructor( 
      private service: COAService,
      //private accountService: AccountService, 
      private toastr: ToastrService,
      private router: Router,
      private confirmService: ConfirmService,
      private modalService: BsModalService
      ) {
        //this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);

        //read navigationExtras
      let nav: Navigation|null = this.router.getCurrentNavigation();

      if (nav?.extras && nav.extras.state) {
          this.bolNavigationExtras=true;
          if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

          if(nav.extras.state['userobject']) {
            this.user = nav.extras.state['userobject'] as User;
          }
      }
    
       }

  ngOnInit() {
    this.getCOAs();
  }

  getCOAs(useCache: boolean=true) {

    this.service.setParams(this.sParams);
    
    return this.service.getCoas(useCache).subscribe({
      next:  response => {
        if(response !==null && response !==undefined) {
          this.coas = response.result;
          this.totalCount = response.count;
          this.pagination = response.pagination;
        }
      },
      error: (err: any) => this.toastr.error(err.error.details, 'Error encountered')
    });
    
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new ParamsCOA();
    this.service.setParams(this.sParams);
    this.getCOAs();
  }
  
  onPageChanged(event: any){
    
    const params = this.service.getParams();

    if (params.pageNumber !== event.page) {
      params.pageNumber = event.page;
      this.service.setParams(params);
      this.getCOAs();
    }
  }
  
  onSearch() {
    var searchString=this.searchTerm!.nativeElement.value;
    if(this.sParams.search===searchString) return;

    this.sParams.search = this.searchTerm!.nativeElement.value;
    this.sParams.pageNumber = 1;
    this.service.setParams(this.sParams);
    this.getCOAs();
  }


  applyFilter() {

    if(this.sParams.divisionToExclude !== this.lastExclude) {
        this.getCOAs(false);
        this.lastExclude = this.sParams.divisionToExclude;
    }
    
  }

  addNewCOA() {

    this.editCOAModal(null);

    /*let route = '/finance/addaccount';
    this.router.navigate(
      [route], 
      { state: 
        { 
          coatoedit: undefined, 
          userobject: this.user,
          toedit: false, 
          returnUrl: '/finance/coalist' 
        } 
      }
    );
    */
  }

  editCOAModal(edit: ICOA | null) {
    if(edit===null) edit = new coa();

    const config = {
      class: 'modal-dialog-centered modal-md',
      initialState: {
        title: 'edit Chart of account',
        coa: edit
      }
    }
    this.bsModalRef = this.modalService.show(CoaEditModalComponent, config);
    
    this.bsModalRef.content.editCOAEvent.pipe(
      filter( response => response !== null && response !== undefined),
      switchMap((editedCOA: ICOA) => {
        return this.service.editCOA(editedCOA)
      })
    ).subscribe((edited: ICOA) => {
      console.log('returned from api:', edited);
      if(edited !== null) {     //succeeded in api
        if(edit==null) {        //object sent to api was null, hence it is add new
          this.coas.push(edited)
          this.toastr.success('COA was added', 'Success');
        } else {                //edited
          var index = this.coas.findIndex(x => x.id===edit?.id);
          if(index !==-1) this.coas[index] = edited;
          this.toastr.success('COA was edited', 'Success')
        }
      } else {                  //filed from api
        this.toastr.warning('Failed to update the COA', 'Failure')
      }
    })

  }
  
  updateCandidateAccountName(t: ICOA) 
  {
    const config = {
      class:'modal-dialog-centered modal-md',
      initialState: {
        title: 'get Application Number to include in the Account Name'
      }
    };
    
    this.bsModalRef = this.modalService.show(InputModalComponent, config);

    this.bsModalRef.content.outputEvent.subscribe((applicationno: any) => {
        this.inputApplicationNo = applicationno;
    });

    if(t.accountName.includes('Application')) {
      var pos = t.accountName.indexOf('Application');
      t.accountName = t.accountName.substring(1,pos +10) + this.inputApplicationNo;
    } else {
      t.accountName = t.accountName + '-Application ' + this.inputApplicationNo;
    }

      this.service.editCOA(t).subscribe(response => {
        var index=this.coas.findIndex(x => x.id===t.id);
        if ((index: number) => 0) {
          this.coas[index]=response;
        }
      })
    
  }

  statementOfAccount(accountid: number) {

    const config = {
      class:'modal-dialog-centered modal-md',
      initialState: {
        title: 'get Date Range Input for Statement of Account'
      }
    };
    
    this.bsModalRef = this.modalService.show(DateInputRangeModalComponent, config);

    this.bsModalRef.content.returnDateRangeEvent.subscribe((dateRange: any) => {
      if(dateRange.fromDate.getFullYear < 2000 || dateRange.uptoDate.getFullYear < 2000) {
        this.toastr.warning('failed to get the date range');
        return;
      }

      var dt1 = this.convertDateToDateOnly(dateRange.fromDate);
      var dt2 = this.convertDateToDateOnly(dateRange.uptoDate);

      let route = '/finance/soa/' + accountid + '/' + dt1 + '/' + dt2;
 
      this.router.navigate(
          [route], 
          { state: 
            { 
              userobject: this.user,
              dateRange: dateRange,
              returnUrl: '/finance/coalist' 
            } }
        );
    }, (error: any) => {
      console.log('error in invoking Date Range input modal', error);
    })

  }
  
  convertDateToDateOnly(datepart: Date) {

    const milliseconds: number = +datepart; // Replace with your milliseconds value
    const date: Date = new Date(milliseconds);

    // Extract the date part
    const year: number = date.getFullYear();
    const month: number = date.getMonth() + 1; // Months are zero-based
    const day: number = date.getDate();

    // Format the date as YYYY-MM-DD
    const formattedDate: string = `${year}-${month.toString().padStart(2, '0')}-${day.toString().padStart(2, '0')}`;

    return formattedDate;
  }


  deleteCoa(accountid: number) {

    if(this.confirmService
      .confirm('Do you want to delete the Chart of Account?  You may want to rename it, instead of deleting it!', 'confirm Delete Chart of account')
      .subscribe(response => {
        if(!response ) {
          this.toastr.info('deletion request canceled');
          return;
        }
      })
      )

    this.service.deleteCOA(accountid).subscribe(response  => {
      if (response) {
        this.removeFromCache(accountid);
        
      } else {
        this.toastr.error('failed to delete the Chart of Account');
      }
    }, error => {
      this.toastr.error('failed to delete the Chart of Account', error);
    })
 
  }

  removeFromCache(id: number) {
    this.service.deleteFromCache(id);
    var index=this.coas.findIndex(x => x.id==id);
    this.coas.splice(index,1);
    this.toastr.success('Chart of account deleted');
  }

  onSortSelected(sort: any) {
    if(this.sParams.sort===sort) return;

    this.sParams.sort = sort;
    this.service.setParams(this.sParams);
    this.getCOAs();
  }

  
  returnToCaller() {
    this.router.navigateByUrl(this.returnUrl || '/finance' );
  }
}
