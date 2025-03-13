import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IClientIdAndNameDto } from 'src/app/_dtos/admin/clientIdAndNameDto';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { IOrder } from 'src/app/_models/admin/order';
import { IOrderItem } from 'src/app/_models/admin/orderItem';
import { IProfession } from 'src/app/_models/masters/profession';
import { User } from 'src/app/_models/user';
import { ContractReviewService } from 'src/app/_services/admin/contract-review.service';
import { OrderService } from 'src/app/_services/admin/order.service';
import { TaskService } from 'src/app/_services/admin/task.service';
import { OrderItemReviewComponent } from '../../orders/order-item-review/order-item-review.component';
import { JdModalComponent } from '../../orders/jd-modal/jd-modal.component';
import { RemunerationModalComponent } from '../../orders/remuneration-modal/remuneration-modal.component';
import { filter, switchMap } from 'rxjs';
import { IAssessmentQBank } from 'src/app/_models/admin/assessmentQBank';
import { OrderAssessmentService } from 'src/app/_services/hr/orderAssessment.service';
import { IOrderAssessmentItem } from 'src/app/_models/admin/orderAssessmentItem';
import { OrderAssessmentItemModalComponent } from '../../orders/order-assessment-item-modal/order-assessment-item-modal.component';
import { IRemunerationDto } from 'src/app/_dtos/admin/remunerationDto';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-dl',
  templateUrl: './dl.component.html',
  styleUrls: ['./dl.component.css']
})
export class DLComponent implements OnInit{

  order: IOrder | undefined;
  associates: IClientIdAndNameDto[]=[];
  customers: IClientIdAndNameDto[]=[];
  employees: IEmployeeIdAndKnownAs[]=[];
  categories: IProfession[]=[];

  isAddMode: boolean=false;
  loading: boolean=false;

  form: FormGroup = new FormGroup({});
  bsModalRef: BsModalRef | undefined;

  bsValueDate = new Date();
  returnUrl: string='';
  routeId: string='';
  user?: User;

  assessmentQBank: IAssessmentQBank | undefined;
  errors: string[]=[];

  constructor(private activatedRoute: ActivatedRoute, 
    private fb: FormBuilder,
    private service: OrderService,
    private orderAssessService: OrderAssessmentService,
    private toastr: ToastrService,
    private router: Router,
    private taskService: TaskService,
    private contractRvwService: ContractReviewService,
    private modalService: BsModalService,
    private confirm: ConfirmService){

      this.routeId = this.activatedRoute.snapshot.params['id'];

      let nav: Navigation|null = this.router.getCurrentNavigation() ;

      if (nav?.extras && nav.extras.state) {
          if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

          if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
          
      }
    }

ngOnInit(): void {
  this.activatedRoute.data.subscribe(data => { 
    this.order = data['order'];
    //this.associates = data['associates'];
    this.customers = data['customers'];
    this.employees = data['employees'];
    this.categories = data['professions'];
    
  })

    this.isAddMode = this.order?.id ===0;

    this.createAndInitializeForm(this.order!);
  }
  
  createAndInitializeForm(ord: IOrder) {
    this.form = this.fb.group({
      id: [ord.id], 
      orderNo: [ord.orderNo, Validators.required],
      orderDate: [ord.orderDate, Validators.required],
      customerId: [ord.customerId, Validators.required], 
      orderRef: [ord.orderRef], 
      salesmanId: [ord.salesmanId], 
      projectManagerId: [ord.projectManagerId, Validators.required],
      cityOfWorking: [ord.cityOfWorking], 
      country: [ord.country, Validators.required],
      completeBy: [ord.completeBy, Validators.required],
      status: [ord.status], 
      forwardedToHRDeptOn: [ord.forwardedToHRDeptOn], 
      contractReviewStatus: [ord.contractReviewStatus],

      orderItems: this.fb.array(
        ord.orderItems.map(ph => (
          this.fb.group({
            selected: false, 
            id: ph.id, 
            orderId: ph.orderId, 
            srNo: ph.srNo, 
            professionId: [ph.professionId, Validators.required],
            ecnr: ph.ecnr,
            sourceFrom: ph.sourceFrom,
            quantity: [ph.quantity, [Validators.required, Validators.min(1)]],
            minCVs: ph.minCVs,
            maxCVs: ph.maxCVs,
            completeBefore: ph.completeBefore,
            status: ph.status,
            reviewItemStatus: [ph.reviewItemStatus],
            requireAssessment: [ph.requireAssessment]
          })
        ))
      ),

    } 
    );

  }

  get orderItems() : FormArray {
    return this.form.get("orderItems") as FormArray
  }

  newItem(): FormGroup {
    //get max SrNo
    var completebefore = this.isAddMode ? this.form.get('completeBy')?.value : '';
  
    var maxSrNo = this.orderItems.length===0 ? 1 : Math.max(...this.orderItems.value.map((x: { srNo: number; }) => x.srNo))+1;
    var completebefore = this.isAddMode ? this.form.get('completeBy')?.value : '';
    return this.fb.group({
      selected: false, 
      id: 0, 
      orderId: this.order?.id, 
      srNo: maxSrNo, 
      professionId: [0,Validators.required], 
      ecnr: false,
      sourceFrom: 'India',
      quantity: [0, [Validators.required, Validators.min(1)]],
      minCVs : 0, 
      maxCVs: 0,
      completeBefore: [this.form.get('completeBy')?.value, Validators.required],
      status: 'Not Started',
      reviewItemStatus: 'Not Reviewed',
      requireAssessment: false
      })
  }

  addItem() {
    this.orderItems.push(this.newItem());
  }

  formChanged() {
    console.log('form changed event');
  }

  setMinCVs(index: number, newValue: number) {
    this.orderItems!.at(index).get('minCVs')!.setValue(newValue);
  }

  setMaxCVs(index: number, newValue: number) {
    this.orderItems!.at(index).get('maxCVs')!.setValue(newValue);
  }
  getMinCVs(index: number) {
    return this.getControls()[index].value.minCVs;
  }

  getMaxCVs(index: number) {
    return this.getControls()[index].value.maxCVs;
  }

  getQnty(index: number) {
    return this.getControls()[index].value.quantity;
  }

  getControls() {
    return (<FormArray>this.form.get('orderItems')).controls;
  }

  qntyChanged(index: number) {
    if(+this.getMinCVs(index) > 0  || +this.getMaxCVs(index) > 0) return;

    var q = +this.getQnty(index);
    this.setMinCVs(index, q*3);
    this.setMaxCVs(index, q*3);
  }

  
  onSubmit() {
    if(this.orderItems.length === 0) {
      this.toastr.warning('Require atleast one Order Item to save the Demand Letter', 'Invalid form data');
      return;
    }
    
    if (this.isAddMode) {
        this.CreateOrder();
    } else {
        this.toastr.info('updating order ...');
        this.UpdateOrder();
    }
  }

  
  removeItem(i:number) {

    var prompt = 'Press DELETE to confirm delete, cancel to cancel deletion';

    this.confirm.confirm("Confirm Delete", prompt, "Delete", "Cancel").subscribe({
      next: (succeeded: boolean) => {
        if(succeeded) {
          this.orderItems.removeAt(i);
          this.orderItems.markAsDirty();
          this.orderItems.markAsTouched();
          this.toastr.success('Demand Letter item deleted', 'Deletion successful');
        } else {
          this.toastr.warning('Deletion aborted by user', 'Aborted');
        }
      }
    })

  }

  close() {
    this.router.navigateByUrl(this.returnUrl);
  }

  private CreateOrder() {
    this.service.register(this.form.value).subscribe({
      next: (response: IOrder) => {
        if(response == null) {
          this.toastr.info('Failed to creat the order');
        } else {
          this.form.get('orderNo')?.value.setValue(response.orderNo);
          this.form.get('id')?.value.setValue(response.id);
          this.toastr.success("Created the Order, with Order No. " + response.orderNo);
          
        }
      },
      error: error => this.toastr.error('Error in creating the Order', error)
    })

  }

  
  displayOrderAssessmentItem(index: number){
    var orderitemid = this.orderItems.at(index).get('id')?.value;
    if(!orderitemid) {
      this.toastr.warning('Failed to get Order Item Id', 'logic error');
      return;
    }

    var observableOuter = this.orderAssessService.getOrderAssessmentItem(orderitemid);

    observableOuter.pipe(
      filter((response: IOrderAssessmentItem) => response !==null),
      switchMap((response: any) => {

        const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
            orderAssessmentItem: response,
            user: this.user
          }
        }
        this.bsModalRef = this.modalService.show(OrderAssessmentItemModalComponent, config);
        const observableInner = this.bsModalRef.content.updateEvent;
        return observableInner
      })
    ).subscribe((response: any) => {
      if(response)   {
        this.toastr.success('Updated the contract review item - you must update this Demand Letter for the changes to take permanent effect', 'Success');
      } else {
        this.toastr.warning('Failed to update the contract review', 'Failed')
      }
    })

  }


  assignTasksToHRExecs() {

    if(this.isAddMode) return;
   
    if(this.order?.projectManagerId===0) {
      this.toastr.warning('Project Manager needs to be defined before tasks can be assiged');
      return;
    }
 
    if (this.isAddMode) {
      this.toastr.error('tasks cannot be assigned while in add mode.  Save the data first and then comeback to this form in edit mode to assign HR Executive tasks');
      return;
    }

    let f = this.form.value;
    var itemids = f.orderItems.filter((x:any) => x.selected===true).map((x:IOrderItem) => x.id);   
    if (itemids.length === 0) {
      this.toastr.error('No Order Items selected');
      return;
    }

    var confirmMsg = "This will create tasks in the name of HR Executives defined in the contract review.  " 
      + "This will also compose messages which you can view/edit in the Messages section.";

    const observableInner = this.service.createOrderAssignmentTasks(itemids);
    const observableOuter = this.confirm.confirm('confirm assign Tasks to HR Executives', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => {
          return observableInner
        })
    ).subscribe({
      next: (response) => {
        if(response === '' || response === null) {
          this.toastr.success('tasks created for the chosen order items. Messages also composed, which you can view/edit in the Messages section', "Success");
        } else  {
          this.toastr.warning(response, 'Failed to process Some or all Order Item Ids');
        }
      },
      error: (err: any) => this.toastr.error(err.error.text, 'Error encountered')
    })
  }

  assignTasksToHRExecsdelete() {
    if(this.isAddMode) return;
   
    if(this.order?.projectManagerId===0) {
      this.toastr.warning('Project Manager needs to be defined before tasks can be assiged');
      return;
    }
 
    if (this.isAddMode) {
      this.toastr.error('tasks cannot be assigned while in add mode.  Save the data first and then comeback to this form in edit mode to assign HR Executive tasks');
      return;
    }

    let f = this.form.value;
    var itemids = f.orderItems.filter((x:any) => x.selected===true).map((x:IOrderItem) => x.id);   
    if (itemids.length === 0) {
      this.toastr.error('No Order Items selected');
      return;
    }
 
    return this.service.createOrderAssignmentTasks(itemids).subscribe((response: string) => {

      if(response === '' || response === null) {
        this.toastr.success('tasks created for the chosen order items', "Success");
      } else {
        this.toastr.warning(response, 'Failure');
      }
    }, error => {
      this.toastr.error(error.error.details, 'failed to create tasks for the chosen order items');
    })

    
  }      

  updateContractReviewStatus() {
    var orderid = this.form.get('id')?.value;
    if(orderid === '' || orderid === null) return  null;

    return this.service.updateContractReviewStatus(+orderid).subscribe({
      next: (response: string) => {
        console.log('response:', response);
        if(response === '' || response === null) {
          this.toastr.info('Failed to update the Contract Review', 'Failure');
        } else {
          //this.form.get('contractReviewStatus')?.setValue(response);
          this.order!.contractReviewStatus = response;
          this.toastr.success('Contract Review value updated', 'Success');
        }
      },
      error: (err: any) => {
        console.log('error in updatecontractreviewstatus:',err);
        this.toastr.error(err.error?.details, 'Error');
      }
    })
  }
  
  openContractReviewItemModal(index: any) 
  {
      var orderitemid = this.orderItems.at(index).get('id')?.value;
      
      var observableOuter = this.contractRvwService.getContractReviewItem(orderitemid);

      observableOuter.pipe(
        filter((response) => response !==undefined && response !== null),
        switchMap((response) => {
          const config = {
            class: 'modal-dialog-centered modal-lg',
            initialState: {
              reviewItem: response}
          }
          
          this.bsModalRef = this.modalService.show(OrderItemReviewComponent, config);
          const observableInner = this.bsModalRef.content.updateModalReview;
          return observableInner
        })
      ).subscribe((response: any) => {       //the modal form updates the content
          if(response !== null)   {
            this.toastr.success('Updated the contract review item - you must update this Demand Letter for the changes to take permanent effect', 'Success');
            this.orderItems.at(index).get('reviewItemStatus')?.setValue(response.reviewItemStatus);
            this.form.markAsDirty();
          } else {
            this.toastr.warning('Failed to update the contract review', 'Failed')
          }
        
      })
  
  }

  navigateByRoute(routeString: string, obj: any,  editable: boolean) {
    let route =  routeString;
    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            object: obj,
            toedit: editable, 
            returnUrl: '/orders/orders/edit/' + this.routeId
          } }
      );
  }

  
  openJDModal(index: number) {
      if(this.isAddMode) return;

      var orderitembrief = this.getControls()[index].value;
      var orderitemid = orderitembrief.id;

      var observableOuter = this.service.getJD(orderitemid);

      observableOuter.pipe(
        filter((response) => response !==undefined && response !== null),
        switchMap((response) => {
          //console.log('contractrvwitemmodal response:', response);
          const config = {
            class: 'modal-dialog-centered modal-md',
            initialState: {
              title: 'Job Description',
              jd: response
            }
          }
          
          this.bsModalRef = this.modalService.show(JdModalComponent, config);
          const observableInner = this.bsModalRef.content.updateSelectedJD;
          return observableInner
        })
      ).subscribe((response) => {       //the modal form updates the content
        if(response) {
          this.toastr.success('Updated the contract review item', 'Success')
        } else {
          this.toastr.success('Failed to update the contract review item', 'Failure')
        }
        
      })
      
  }

  openRemunerationModal(index: number) {
    if(this.isAddMode) return;
    var orderitemid = this.getName(index);

    var observableOuter = this.service.getRemuneration(orderitemid);

    observableOuter.pipe(
      filter((response: IRemunerationDto) => response !==null),
      switchMap((response: IRemunerationDto) => {
        const config = {
          class: 'modal-dialog-centered modal-md',
          initialState: {
            remun: response,
            editable: true
          }
        }

        this.bsModalRef = this.modalService.show(RemunerationModalComponent, config);
        const observableInner = this.bsModalRef.content.updateSelectedRemuneration;
        return observableInner
      })
    ).subscribe((response: any) => {
      if(response !== null)   {    //response: IRemunerationDto
       
        this.toastr.success('Updated the Remuneration details', 'Success');
      } else {
        this.toastr.warning('Failed to update the Remuneration details', 'Failed')
      }
    })//, error: (err: any) => this.toastr.error(err.error?.details, 'Error encountered')
  }

  getName(i: number) {
    return this.getControls()[i].value.id;
}

  private UpdateOrder() {

    this.service.UpdateOrder(this.form.value).subscribe({
      next: succeeded => {
        if(succeeded) {
          this.toastr.success('Order Updated', 'Success')
        } else {
          this.toastr.warning('Failed to update the order', 'Failure')
        }
      },
      error: err => {
        console.log('updateOrder error:', err);
        if(err.error.error) {
          this.toastr.error(err.error.error, 'Error encountered')
        } else {
          this.toastr.error(err, 'Error encountered')
        }
      }
    })
  }

  forwardOrderToHRDept() {

  }

  OrderForwardedTo(){
    
  }

}

