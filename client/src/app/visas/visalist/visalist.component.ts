import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { distinct, filter, map, switchMap, take } from 'rxjs';
import { IOrderItemForVisaAssignmentDto } from 'src/app/_dtos/admin/orderItemForVisaAssignmentDto';
import { IVisaBriefDto } from 'src/app/_dtos/admin/visaBriefDto';
import { IVisaAssignment } from 'src/app/_models/admin/visaAssignment';
import { Pagination } from 'src/app/_models/pagination';
import { visaParams } from 'src/app/_models/params/Admin/visaParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { VisaService } from 'src/app/_services/admin/visa.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { VisaAssignModalComponent } from '../visa-assign-modal/visa-assign-modal.component';

@Component({
  selector: 'app-visalist',
  templateUrl: './visalist.component.html',
  styleUrls: ['./visalist.component.css']
})
export class VisalistComponent implements OnInit{

    @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
    orderItemsForTheCustomer: IOrderItemForVisaAssignmentDto[] = [];

    visas: IVisaBriefDto[]=[];
    pagination: Pagination | undefined;
    totalCount = 0;
    totalBal = 0;

    user?: User;
    vParams = new visaParams();

    isReportActive: boolean = false;
    printVisas: IVisaBriefDto[]=[];

    isPrintPDF: boolean = false;
    printtitle = '';
    visaCustomers: string[]=[];
    visaCustomerSelected: string = '';
    assignments: IVisaAssignment[]=[];
    

    bsModalRef: BsModalRef|undefined;
  
    constructor(private service: VisaService, 
      //private mastersService: MastersService,
      private accountsService: AccountService,
      private activatedRoute: ActivatedRoute,
      private toastr: ToastrService, 
      private modalService: BsModalService,
      private confirm: ConfirmService,
      private router: Router) {
        this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
        }
    
    ngOnInit(): void {
      
      this.activatedRoute.data.subscribe(data => {
        this.visas = data['visas'].result;
        this.pagination = data['visas'].pagination;
        this.totalCount = data['visas'].totalCount;
      })
      this.visaCustomers = this.visas.map(x => x.customerKnownAs),distinct();
      
      this.totalBal = +this.visas.map((x: any) => x.visaBalance)
          .reduce((a:number,b:number) => +a+(+b),0);

    }

    loadVisas() {
      var params = this.service.getParams();
      this.service.getPagedVisasBrief(params)?.subscribe({
      next: response => {
        if(response !== undefined && response !== null) {
          this.visas = response.result;
          this.totalCount = response?.count;
          this.pagination = response.pagination;

        } else {
          console.log('response is undefined');
        }
      },
      error: (err: any) => console.log(err.error?.details, 'Error encountered')
    })
  }

    onPageChanged(event: any){
      const params = this.service.getParams();
      if (params.pageNumber !== event) {
        params.pageNumber = event.page;
        this.service.setParams(params);
  
        this.loadVisas();
      }
    }

    deleteVisa(event: any) {
        var id=event;
        var confirmMsg = 'confirm delete this Visa?. WARNING: this cannot be undone';
    
        const observableInner = this.service.deleteVisa(id);
        const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);
    
        observableOuter.pipe(
            filter((confirmed) => confirmed),
            switchMap((confirmed) => {
              return observableInner
            })
        ).subscribe(response => {
          if(response) {
            this.toastr.success('Visa deleted', 'deletion successful');
            var index = this.visas.findIndex(x => x.id == id);
            if(index >= 0) this.visas.splice(index,1);
          } else {
            this.toastr.error('Error in deleting the Visa', 'failed to delete')
          }
          
        });
    }

    editVisa(event: any) {

      {if(event === null) {
          this.toastr.warning('No Visa Id available');
          return;
        }  
      }

      this.router.navigateByUrl('/visas/visaEdit/' + event)
    }

    generatePDF() {
    
        this.printtitle =  "Visas for " + this.visaCustomerSelected;
          
        this.printVisas = this.visas.filter(x => x.customerKnownAs === this.visaCustomerSelected);

        if(this.printVisas.length) this.isPrintPDF = true;
    }

    saveAssignments() {
      this.service.assignVisaItemToOrderItem(this.assignments).subscribe({
        next: (response: IVisaAssignment[]) =>  {
          if(response.length > 0) {
            this.toastr.success('visa assignments updated')
          } else {
            this.toastr.warning('failed to update the visa assignments')
          }
        }, error: (err: any) => this.toastr.error(err.error?.details, 'Error')
      })
    }

    deleteAssignment(assignment: IVisaAssignment) {

    }
        //this.router.navigateByUrl("/prospectives/pdf/001012/'active'")
    getOrderItemsForCustomer(visaItem: IVisaBriefDto) {
     //Having selected candidates, refer them to internal reviews or directly to client

        const config = {
          class: 'modal-dialog-centered',
          initialState: {
            title: 'Assign Visas to Order Categories',
            visaItem: visaItem
          }
        }
        this.bsModalRef = this.modalService.show(VisaAssignModalComponent, config);
        this.bsModalRef.content.assignEvent.subscribe((values: IVisaAssignment[]) => {
          if (values.length > 0) {
            this.service.assignVisaItemToOrderItem(values).subscribe(() => {
              
            })
        }
      })
    }

    DisplayOrderItemsOfCustomer(visaItem: IVisaBriefDto) {
      
        var observableOuter = this.service.getOrderItemsForCustomer(visaItem.customerId);
      
        observableOuter.pipe(
          filter((response: IOrderItemForVisaAssignmentDto[]) => response.length > 0),
          switchMap((response: any) => {
              console.log('order items in outer loop', response);
              const config = {
                class: 'modal-dialog-centered modal-lg',
                initialState: {
                  title: 'Assign Visas to Order Categories',
                  visaItem: visaItem,
                  orderItemsForTheCustomer: response
                }
              }
              this.bsModalRef = this.modalService.show(VisaAssignModalComponent, config);
              const observableInner = this.bsModalRef.content.assignEvent;
              return observableInner
            })
          ).subscribe((response: any) => {
            if(response)   {
              if(response) {
                this.service.assignVisaItemToOrderItem(response).subscribe({
                  next: (response: IVisaAssignment[]) => this.toastr.success('Visa assigned to ')
                })
              }
              this.toastr.success('Viewed/updated the Custom Assessment Parameters for the category', 'Success');
            } else {
              this.toastr.warning('Failed to view/update the Custom Assessment Parameters', 'Failed')
            }
          })
    }

    displayVisaConsumed(visaId: number) {
      this.navigateByRoute(visaId, '/visas/visaTransactions')
    }

    navigateByRoute(id: number, routeString: string) {
    
      let route = id===0 ? routeString :  routeString + '/' + id;
  
      this.router.navigate(
          [route], 
          { state: 
            { 
              user: this.user, 
              returnUrl: '/visas' 
            } }
        );
    }
}