import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap, take, tap } from 'rxjs';
import { IOfficialAndCustomerNameDto } from 'src/app/_dtos/admin/client/oficialAndCustomerNameDto';
import { IOrderBriefDto } from 'src/app/_dtos/admin/orderBriefDto';
import { IApplicationTask } from 'src/app/_models/admin/applicationTask';
import { ICustomerNameAndCity } from 'src/app/_models/admin/customernameandcity';
import { Pagination } from 'src/app/_models/pagination';
import { orderParams } from 'src/app/_models/params/Admin/orderParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { CustomersService } from 'src/app/_services/admin/customers.service';
import { OrderForwardService } from 'src/app/_services/admin/order-forward.service';
import { OrderService } from 'src/app/_services/admin/order.service';
import { TaskService } from 'src/app/_services/admin/task.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CvrefService } from 'src/app/_services/hr/cvref.service';
import { SelectionService } from 'src/app/_services/hr/selection.service';
import { SelectAssociatesModalComponent } from 'src/app/select-associates-modal/select-associates-modal.component';

@Component({
  selector: 'app-orders-listing',
  templateUrl: './orders-listing.component.html',
  styleUrls: ['./orders-listing.component.css']
})
export class OrdersListingComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  orders: IOrderBriefDto[]=[];
  oParams = new orderParams();
  totalCount: number=0;
  
  customers: ICustomerNameAndCity[]=[];
  pagination: Pagination | undefined;

  task?: IApplicationTask;   //to show in task.edit component

  user?: User;

  isReportActive: boolean = false;
  printOrders: IOrderBriefDto[]=[];
  distinctStatus: string = '';
      
  //right click context menu
  title = 'context-menu';

  sortOptions = [
    {name:'By Order No Asc', value:'orderno'},
    {name:'By Order No Desc', value:'ordernodesc'},
    {name:'By City Asc', value:'city'},
    {name:'By City Desc', value:'citydesc'},
    {name:'By Profession Asc', value:'prof'},
    {name:'By Profession Desc', value:'profdesc'},
    {name:'By Contract Reviewed Asc', value:'reviewed'},
    {name:'By Contract Rvwd Desc', value:'reveiweddesc'},
  ]

  orderStatus = [
    {name: 'Not Reviewed', value: 'Not Reviewed'},
    {name: 'Reviewed and Approved', value: 'ReviewedAndApproved'},
    {name: 'Reviewed and declined', value: 'ReviewedAndDeclined'}
  ]

  bsModalRef: BsModalRef|undefined;

  constructor(private service: OrderService, 
    //private mastersService: MastersService,
    private accountsService: AccountService, private selService: SelectionService,
    private taskService: TaskService, private orderFwdService: OrderForwardService,
    private router: Router, private activatedRoute: ActivatedRoute,
    private toastr: ToastrService, private modalService: BsModalService,
    private customerService: CustomersService, private dlFwdService: OrderForwardService,
    private confirmService: ConfirmService, private cvrefService: CvrefService) {
      this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
     }

  ngOnInit(): void {
    //this.service.setOParams(this.oParams);
    this.getOrders(this.oParams);
    
    this.activatedRoute.data.subscribe({
      next: response => {
        //this.orders = response['orders'];
        //this.professions = response['professions']; 
        this.customers = response['customers']
      }
    })
  }

  getOrders(oParams: orderParams) {
    this.service.getOrdersBrief(oParams).subscribe({
      next: response => {
        if(response.result && response.pagination) {
          this.orders = response.result;
          this.pagination = response.pagination;
          this.totalCount = response.count;
          
        }
      },
      error: error => console.log(this.orders)
    });

  }


  onSearch() {
    const params = this.service.getOParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setOParams(params);
    this.getOrders(params);
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.oParams = new orderParams();
    this.service.setOParams(this.oParams);
    this.getOrders(this.oParams);
  }

  onSortSelected(event: any) {
    var sort = event?.target.value;
    this.oParams.pageNumber=1;
    this.oParams.sort = sort;
    this.getOrders(this.oParams);
  }
  
  onPageChanged(event: any){
    const params = this.service.getOParams() ?? new orderParams();
    
    if (params.pageNumber !== event.page) {
      params.pageNumber = event.page;
      this.service.setOParams(params);
      this.getOrders(params);
    }
  }

  deleteOrder (id: number){
    this.confirmService.confirm('confirm delete this Order', 'confirm delete order').pipe(
      switchMap(confirmed => this.service.deleteOrder(id).pipe(
        catchError(err => {
          return of();
        }),
        
        tap(res => this.toastr.success('deleted Order')),

      )),
      catchError(err => {
        this.toastr.error('Error in getting delete confirmation', err);
        return of();
      })
    ).subscribe(
        () => {
          var index = this.orders.findIndex(x => x.id === id);
          if(index !== -1) this.orders.splice(index,1);
          this.toastr.success('order deleted');
        },
        (err: any) => {
          console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
      })
  }

  editOrder(id: any) {
    this.navigateByRoute(id, '/orders/edit', true);
  }

  contractReviewOrder(id: number) {
    this.navigateByRoute(id, '/orders/orderReview', true);
  }
  
  orderAssessmentItem(event: any) {
    var id = event;
    this.navigateByRoute(id, 'orders/orderitemreview', true);
  }

  orderForwardToAssociates(event: any) {

        const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
          }
        }

        this.bsModalRef = this.modalService.show(SelectAssociatesModalComponent, config);

        const observableOuter =  this.bsModalRef.content?.selectedOfficialsEvent;    //returns a sngle object, not collection

        observableOuter.pipe(
          filter((response: IOfficialAndCustomerNameDto[]) => response.length > 0),
          switchMap((response: IOfficialAndCustomerNameDto[]) => {
            var orderid = event;
            //console.log('event from SelectAssociatesModal', event);
            return this.orderFwdService.forwardOrderToSelectedAgents(response, orderid)
          })
      ).subscribe({
        next: () => this.toastr.success('Requirement forwarded', 'Success')
        , error: (err: any) => {
          console.log(err);
          this.toastr.error(err.error.details, 'Error encountered');
        }
      })
  }

  
  DLForwarded(event: any) {
      var id = event;
      this.navigateByRoute(id, 'orders/DLForwarded', true);
  }

  acknowledgeToClient(event: any) {
    this.service.acknowledgeOrderToClient(event).subscribe({
      next: response => {
        if(response) {
          this.toastr.success('message of acknowledgement composed and saved in Messages Draft');
        } else {
          this.toastr.warning('failed to compose message of acknowledgement');
        }
      },
      error: err => this.toastr.error('Error in composing acknowldgement message to client', err)
    });
  }

  cvsReferred(event: any)   //event: order.id
  {
    this.navigateByRoute(event, '/orders/cvsreferred', false)
  }

  selectionRejection(event: any) {    //event.order.id
    this.navigateByRoute(event, "/orders/selections", false);
  }

  navigateByRoute(id: number, routeString: string, editable: boolean) {
    
    let route = id===0 ? routeString :  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            toedit: editable, 
            returnUrl: '/orders/orders' 
          } }
      );
  }
  
  @HostListener('document:click')
  documentClick(): void {
    //this.isDisplayContextMenu = false;
  }

  closePrintSection() {
    this.isReportActive = false;
    this.distinctStatus = '';
  }

}
