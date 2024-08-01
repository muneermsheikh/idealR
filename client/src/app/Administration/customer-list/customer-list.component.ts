import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { ICustomerBriefDto } from 'src/app/_dtos/admin/customerBriefDto';
import { Pagination } from 'src/app/_models/pagination';
import { customerParams } from 'src/app/_models/params/Admin/customerParams';
import { User } from 'src/app/_models/user';
import { CustomersService } from 'src/app/_services/admin/customers.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { IFeedbackHistoryDto } from 'src/app/_dtos/admin/feedbackAndHistoryDto';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css']
})
export class CustomerListComponent implements OnInit{

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  user?: User;
  returnUrl = '';

  customerDtos: ICustomerBriefDto[]=[];
  customerSelected: ICustomerBriefDto|undefined;

  customerTypeSelected='';

  totalCount = 0;
  cParams = new customerParams();
  
  pagination: Pagination | undefined;
  bsModalRef: BsModalRef | undefined;

  display: boolean=false;
  customerIdSelected: number=0;
  history: IFeedbackHistoryDto[]=[];

  constructor(private service: CustomersService, 
      private router: Router,
      private confirm: ConfirmService, 
      private toastr: ToastrService, 
      private modalService: BsModalService) 
  {
        service.setParams(this.cParams);
        
        let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
        }
  }
  
  ngOnInit(): void {
    this.loadCustomers();
  }


    loadCustomers() {
        var params = this.service.getParams();
        
        this.service.getCustomers(params)?.subscribe({
        next: response => {
          if(response !== undefined && response !== null) {
            this.customerDtos = response.result;
            this.totalCount = response?.count;
            this.pagination = response.pagination;
          } 
        },
        error: error => console.log(error)
      })
      
    }

    onPageChanged(event: any){
      const params = this.service.getParams();
      if (params.pageNumber !== event) {
        params.pageNumber = event.page;
        this.service.setParams(params);

        this.loadCustomers();
      }
    }

    setParameters() {
      var params = new customerParams();
      this.service.setParams(params);
      this.loadCustomers();
    }

    applyCustomerType() {
      if(this.cParams.customerType===this.customerTypeSelected) {
        return
      } else {
        this.loadCustomers();
        this.customerTypeSelected=this.cParams.customerType;
      }
      
    }

    selectedClicked(item: any) {
      if(item.checked===true) {
        this.customerDtos.filter(x => x.id != item.id).forEach(x => x.checked=false);  
      } else {
        this.customerDtos.forEach(x => x.checked=false);
      }
      this.customerSelected = item.checked ? undefined : item;
    }

    customerEvaluationCicked(event: any) {

    }

    deleteClicked(event: any) {
      var id=event;
      var confirmMsg = 'confirm delete this Customer?. WARNING: this cannot be undone';

      const observableInner = this.service.deleteCustomer(id);
      const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

      observableOuter.pipe(
          filter((confirmed) => confirmed),
          switchMap((confirmed) => {
            return observableInner
          })
      ).subscribe(response => {
        if(response) {
          this.toastr.success('Customer deleted', 'deletion successful');
          var index = this.customerDtos.findIndex(x => x.id == id);
          if(index >= 0) this.customerDtos.splice(index,1);
        } else {
          this.toastr.error('Error in deleting the checklist', 'failed to delete')
        }
        
      });

    }


    /*editByModalClicked(event: any, item: ICustomerBriefDto)    //event:customer.id
    {
          if(event === null) {
            this.toastr.warning('No customer object returned from the modal form');
            return;
          }  

        const config = {
            class: 'modal-dialog-centered modal-lg',
            initialState: {
              customer: event,
            }
          }

          this.bsModalRef = this.modalService.show(CustomerEditModalComponent, config);

          const observableOuter =  this.bsModalRef.content.updateEvent;
          
          observableOuter.pipe(
            filter((response: ICustomer) => response !==null),
            switchMap((response: ICustomer) =>  {
              return this.service.updateCustomer(response)
            })
          ).subscribe((response: string) => {
      
            if(response==='') {
              this.toastr.success('Customer updated', 'Success');
      
            } else {
              this.toastr.warning(response, 'Failure');
            }
            
          })
              
    }
    */

    addClicked() {
      this.navigateByRoute(0, '/administration/customerEdit');
    }
    
    editClicked(event: any) {

      this.navigateByRoute(event, '/administration/customerEdit');
    }

    feedbackClicked(event: any) {
      var id=event;
 
      if(id === undefined)   return;
      //this.router.navigateByUrl('/administration/feedback/0/' + event);
      this.navigateByRoute(event, '/administration/feedback/0');
    }

    evaluationClicked(event: any) {   //customer line emits ICustomerReview

        this.navigateByRoute(event, '/administration/reviewEdit');
       
      }

    
      
    onSearch() {
      const params = this.service.getParams();
      params.search = this.searchTerm?.nativeElement.value;
      params.pageNumber = 1;
      this.service.setParams(params);
      this.loadCustomers();
    }

    onReset() {
      this.searchTerm!.nativeElement.value = '';
      //this.pParams = new CallRecordParams();
      this.cParams = new customerParams();
      this.service.setParams(this.cParams);
      this.loadCustomers();
    }

    
  navigateByRoute(id: number, routeString: string) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            returnUrl: '/administration/customers' 
          } }
      );
  }

  close() {
    this.router.navigateByUrl(this.returnUrl);
  }

  getHistories(customerId: number) {

  }

  displayFeedback(feedbackId: number) {

  }

  displayHistory(customerId: number) {
    this.display=!this.display;

    if(this.display) {
        if(this.customerIdSelected == customerId) return;
        this.service.getFeedbackHistory(customerId).subscribe({
          next: (response: IFeedbackHistoryDto[]) => {
            console.log('response', response);
            if(response === null) {
              this.toastr.warning('Failed to retrieve the history of the customer', 'No record count')
            } else {
              this.history = response
            }
          }, error: (err: any) => {
            console.log('error:', err);
            if(err.error.message) {
              this.toastr.error(err.error.message, 'Error encountered')
            }
          }
        })
        this.customerIdSelected = customerId;
    }
  }

}
