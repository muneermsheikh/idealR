import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap, tap } from 'rxjs';
import { IVoucherDto } from 'src/app/_dtos/finance/voucherDto';
import { Pagination } from 'src/app/_models/pagination';
import { transactionParams } from 'src/app/_models/params/finance/transactionParams';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { VouchersService } from 'src/app/_services/finance/vouchers.service';

@Component({
  selector: 'app-voucher-list',
  templateUrl: './voucher-list.component.html',
  styleUrls: ['./voucher-list.component.css']
})
export class VoucherListComponent implements OnInit{

  @ViewChild('search', {static: false}) searchTerm?: ElementRef;

  sParams = new transactionParams();
  totalCount= 0;
  vouchers: IVoucherDto[]=[];
  voucher?: IVoucherDto;

  pagination: Pagination | undefined;
  
  user?: User;
  bolNavigationExtras:boolean=false;
  returnUrl: string='';

  constructor( private service: VouchersService,
      private toastr: ToastrService,
      private router: Router,
      private confirmService: ConfirmService) {

        let nav: Navigation | null = this.router.getCurrentNavigation();

        if (nav?.extras && nav.extras.state) {
            this.bolNavigationExtras=true;
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            if(nav.extras.state['userobject']) {
              this.user = nav.extras.state['userobject'] as User;
            }
        }
       }

  ngOnInit(): void {
    this.getVouchers();
  }

  getVouchers() {

    this.service.setParams(this.sParams);

    this.service.getVouchers().subscribe({
      next: response => {
        this.vouchers = response.result;
        this.pagination = response.pagination;
        this.totalCount = response.count;
      }
    })
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new transactionParams();
    this.getVouchers();
  }
  
  onPageChanged(event: any){

    if (this.sParams.pageNumber !== event.page) {
      this.sParams.pageNumber=event.page;
     this.getVouchers();
    }
  }

  
  onSearch() {
    var searchString=this.searchTerm!.nativeElement.value;
    if(this.sParams.search===searchString) return;

    this.sParams.search = this.searchTerm!.nativeElement.value;
    this.sParams.pageNumber = 1;
    this.service.setParams(this.sParams);
    this.getVouchers(); 
  }


  addNewFinanceTransaction() {
    let route = '/finance/voucherEdit/0';
    this.router.navigate([route], { state: { toedit: true, returnUrl: '/finance/voucherlist' } });
  }

  editTransaction(id: any, toedit: boolean) {
    let route = '/finance/voucherEdit/' + id;
    this.router.navigate([route], { state: { toedit: toedit, returnUrl: '/finance/voucherlist', userObject: this.user } });
  }

  
  viewTransaction(id: any, readonly: boolean) {
    let route = '/finance/voucherEdit/' + id;
    this.router.navigate([route], { state: { toedit: false, readonly: readonly, returnUrl: '/finance/voucherlist', userObject: this.user } });

  }

  deleteVoucher(voucherid: any) {

    this.confirmService.confirm('confirm delete this voucher', 'confirm delete voucher').pipe(
      filter(result => result),
      switchMap(confirmed => this.service.deleteVoucher(voucherid).pipe(
        catchError(err => {
          console.log('Error in deleting the voucher', err);
          return of();
        }),
        tap(res => this.toastr.success('deleted voucher')),
        //tap(res=>console.log('delete voucher succeeded')),
      )),
      catchError(err => {
        this.toastr.error('Error in getting delete confirmation', err);
        return of();
      })
    ).subscribe(
      deleteReponse => {
        console.log('deete succeeded');
        this.toastr.success('voucher deleted');
      },
      err => {
        console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
      }
    )

  }


  removeTaskFromCache(id: number) {
    this.service.deleteVoucherFromCache(id);
    var index=this.vouchers.findIndex(x => x.id==id);
    this.vouchers.splice(index,1);
    this.toastr.success('task deleted');
  }

  
  returnToCaller() {
    //console.log('return to caller:', this.returnUrl);
    this.router.navigateByUrl(this.returnUrl || '/' );
  }

}
