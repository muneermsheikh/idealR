import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IJDDto } from 'src/app/_dtos/admin/jdDto';
import { IRemunerationDto } from 'src/app/_dtos/admin/remunerationDto';
import { IContractReview } from 'src/app/_models/admin/contractReview';
import { ICustomerOfficialDto } from 'src/app/_models/admin/customerOfficialDto';
import { ICustomerNameAndCity } from 'src/app/_models/admin/customernameandcity';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { IJobDescription } from 'src/app/_models/admin/jobDescription';
import { IOrder, Order } from 'src/app/_models/admin/order';
import { IOrderItem } from 'src/app/_models/admin/orderItem';
import { IRemuneration } from 'src/app/_models/admin/remuneration';
import { IProfession } from 'src/app/_models/masters/profession';
import { User } from 'src/app/_models/user';
import { OrderService } from 'src/app/_services/admin/order.service';
//import { ConfirmService } from 'src/app/_services/confirm.service';
import { ChooseAgentsModalComponent } from '../../choose-agents-modal/choose-agents-modal.component';
import { OrderForwardService } from 'src/app/_services/admin/order-forward.service';
import { IOfficialIdsAndOrderItemIdsDto } from 'src/app/_dtos/admin/officialIdsAndOrderItemIdsDto';
import { TaskService } from 'src/app/_services/admin/task.service';
import { JdModalComponent } from '../jd-modal/jd-modal.component';
import { RemunerationModalComponent } from '../remuneration-modal/remuneration-modal.component';
import { OrderItemReviewModalComponent } from '../order-item-review-modal/order-item-review-modal.component';
import { ContractReviewService } from 'src/app/_services/admin/contract-review.service';

@Component({
  selector: 'app-order-edit',
  templateUrl: './order-edit.component.html',
  styleUrls: ['./order-edit.component.css']
})
export class OrderEditComponent implements OnInit {

  @ViewChild('editForm') editForm: NgForm | undefined;

  routeId: string;

  member?: IOrder | undefined;
  user?: User;
  
  form: FormGroup = new FormGroup({});

  selectedCategoryIds: number[]=[];
  categories: IProfession[]=[];
  employees: IEmployeeIdAndKnownAs[]=[];
  customers: ICustomerNameAndCity[]=[];
  associates: ICustomerOfficialDto[]=[];
  
  fileToUpload: File | null = null;

  events: Event[] = [];

  isAddMode: boolean = false;
  loading = false;
  submitted = false;

  errors: string[]=[];

  bsValue = new Date();
  bsRangeValue = new Date();
  maxDate = new Date();
  minDate = new Date();

  defaultCompleteByTargetDays=7;
  defaultProjectManagerId=1;
  defaultOrderDate=new Date();
  
  dt=new Date();
  defaultCompleteby = new Date(this.dt.setDate(this.defaultOrderDate.getDate() + this.defaultCompleteByTargetDays)); 

  //file uploads
  uploadProgress = 0;
  selectedFiles: File[]=[];
  uploading = false;
  fileErrorMsg = '';

  bsValueDate = new Date();
  bsModalRef: BsModalRef | undefined;
  jd?: IJDDto;
  remun?: IRemunerationDto;
  cReview?: IContractReview;
  
  mySelect = 0;
  selectedCustomerName: any;
  selectedProjManagerName: any;

  hasEditRole=false;
  hasHRRole=false;

  bolNavigationExtras: boolean=false;
  returnUrl: string='/orders';

  orderReviewItemStatuses: string[] = ['under review', 'approved', 'rejected'];

  //modal choose agents
  existingOfficialIds: ICustomerOfficialDto[]=[]; // IChooseAgentDto[]=[];

  statuses= [
    {id: 1, status: 'Not Reviewed'},{id: 4, status: 'Accepted'}, {id: 2, status: 'Accepted with regrets'}, 
    {id: 3, status: 'Regretted'}
  ]

  constructor(private service: OrderService, 
      //private bcService: BreadcrumbService, 
      private orderFwdService: OrderForwardService,
      private modalService: BsModalService,
      private activatedRoute: ActivatedRoute, 
      private router: Router, 
      //private rvwService: ContractReviewService,
      //private confirmService: ConfirmService,
      private toastr: ToastrService, 
      private taskService: TaskService,
      private contractRvwService: ContractReviewService,
      //private accountsService: AccountService,
      private fb: FormBuilder) {
          this.routeId = this.activatedRoute.snapshot.params['id'];
          this.router.routeReuseStrategy.shouldReuseRoute = () => false;

          //this.accountsService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);

          //navigationExtras
          let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
              this.bolNavigationExtras=true;
              if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

              if( nav.extras.state['user']) {
                this.user = nav.extras.state['user'] as User;
                //this.hasEditRole = this.user.roles.includes('AdminManager');
                //this.hasHRRole =this.user.roles.includes('HRSupervisor');
              }
              //if(nav.extras.state.object) this.orderitem=nav.extras.state.object;
          }
          //this.bcService.set('@editOrder',' ');
   }

    ngOnInit(): void {

      this.activatedRoute.data.subscribe(data => { 
        this.member = data['order'];
        this.associates = data['associates'];
        this.customers = data['customers'];
        this.employees = data['employees'];
        this.categories = data['professions'];

      })

        this.isAddMode = this.member?.id ===0;
        if(this.isAddMode)  this.member=new Order();
        if(this.member === undefined || this.member===null) {
          this.toastr.error('failed to retrieve order data', 'no data available');
        } 

        if(this.member) this.createAndInitializeForm(this.member);
      
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
          cityOfWorking: [ord.cityOfWorking, Validators.required],
          country: [ord.country, Validators.required],
          completeBy: [ord.completeBy, Validators.required],
          status: [ord.status], 
          forwardedToHRDeptOn: [ord.forwardedToHRDeptOn], 
          contractReviewStatus: [ord.contractReviewStatus],
          orderItems: this.fb.array(
            ord.orderItems.map(ph => (
              this.fb.group({
                selected: false, 
                id: ph.id, orderId: ph.orderId, srNo: ph.srNo, 
                professionId: [ph.professionId, Validators.required],
                ecnr: ph.ecnr, sourceFrom: ph.sourceFrom, quantity: ph.quantity, 
                minCVs: ph.minCVs, maxCVs: ph.maxCVs,
                completeBefore: ph.completeBefore, 
                status: ph.status,
                reviewItemStatus: ph.reviewItemStatus
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
      var maxSrNo = this.orderItems.length===0 ? 1 : Math.max(...this.orderItems.value.map((x:any) => x.srNo))+1;
      var completebefore = this.isAddMode ? this.form.get('completeBy')?.value : '';
      return this.fb.group({
        selected: [false],  
        id: 0, orderId: 0, srNo: maxSrNo, 
        professionId: [0,[Validators.required, Validators.min(1)]], 
        ecnr: false, sourceFrom: 'India', quantity: [0, Validators.min(1)], 
        completeBefore: [completebefore, Validators.required], 
        status: 'Not Started', reviewItemStatus: 'Not Reviewed'})
    }

    addItem() {
      this.orderItems.push(this.newItem());
    }

    removeItem(i:number) {
      this.orderItems.removeAt(i);
      this.orderItems.markAsDirty();
      this.orderItems.markAsTouched();
    }

     onSubmit() {
      if (this.isAddMode) {
          this.CreateOrder();
      } else {
          this.toastr.warning('updating order ...');
          this.UpdateOrder();
      }
    }

    acknowledgeToClient() {
      
    }

    private CreateOrder() {
      this.service.register(this.form.value).subscribe({
        next: (response: IOrder) => {
          if(response == null) {
            this.toastr.info('Failed to creat the order');
          } else {
            this.toastr.success("Created the Order, with Order No. " + response.orderNo)
          }
        },
        error: error => this.toastr.error('Error in creating the Order', error)
      })

    }

    private UpdateOrder() {
      this.service.UpdateOrder(this.form.value).subscribe(response => {
        this.toastr.success('order updated');
        this.router.navigateByUrl('/orders');
      }, error => {
        console.log(error);
      })
    }

    handleFileInput(files: FileList) {
      this.fileToUpload = files.item(0);
    }

    getJD(orderitemid: number) {
      return this.service.getJD(orderitemid).subscribe(response => {
        this.jd=response;
      }, error => {
        this.toastr.error('failed to retrieve job description');
      })
    }

    getName(i: number) {
        return this.getControls()[i].value.id;
    }

    getSrNo(i: number) {
      return this.getControls()[i].value.srNo;
    }

    getQnty(index: number) {
      return this.getControls()[index].value.quantity;
    }

    getMinCVs(index: number) {
      return this.getControls()[index].value.minCVs;
    }

    getMaxCVs(index: number) {
      return this.getControls()[index].value.maxCVs;
    }

    getReviewItemStatusId(index: number) {
      return this.getControls()[index].value.reviewItemStatusId;
    }

    setMinCVs(index: number, newValue: number) {
      this.orderItems!.at(index).get('minCVs')!.setValue(newValue);
    }

    setMaxCVs(index: number, newValue: number) {
      this.orderItems!.at(index).get('maxCVs')!.setValue(newValue);
    }
  
    getControls() {
      return (<FormArray>this.form.get('orderItems')).controls;
    }

    openJDModal(index: number) {
      if(this.isAddMode) return;
      var orderitemid = this.getName(index);
        this.service.getJD(orderitemid).subscribe(response => {
            var jd = response;
 
            const initialState = {
              class: 'modal-dialog-centered modal-lg',
               title: 'job description',
               jd
            };
            this.bsModalRef = this.modalService.show(JdModalComponent, {initialState});
            //**TODO** IMPLEMENT SWITCHMAP HERE, TO AVOID SUBSCRIPTION NESTING - CHECK implementation in referral edit */
            this.bsModalRef.content.updateSelectedJD.subscribe((values: IJobDescription) => {
            this.service.updateJD(values).subscribe(() => {
              this.toastr.success("job description updated");
            }, error => {
              this.toastr.error("failed to update the job description");
            })
          }
          )
          
        }, error => {
          this.toastr.warning('failed to retrieve job description');
        })
        
    }

    openRemunerationModal(index: number) {
      if(this.isAddMode) return;
      var orderitemid = this.getName(index);

        this.service.getRemuneration(orderitemid).subscribe(response => {
            //this.remun=response;
            var remun = response;
       
            const initialState = {
              class: 'modal-dialog-centered modal-lg',
              remun
            };
            this.bsModalRef = this.modalService.show(RemunerationModalComponent, {initialState});
            
            this.bsModalRef.content.updateSelectedRemuneration.subscribe((values: IRemuneration) => {
            this.service.updateRemuneration(values).subscribe(() => {
              this.toastr.success("Remuneration updated");
            }, error => {
              this.toastr.error("failed to update the Remuneration");
            })
          }
          )
          
        }, error => {
          this.toastr.warning('failed to retrieve Remuneration data');
        })
        
    }

    
    forwardOrderToAgents() {

      if(this.isAddMode) {
        this.toastr.warning('Only an order that is contract reviewed can be sent to ASsociates');
        return;
      }
      
      if(this.member === null || this.member === undefined) return;

      var selectedOrderItems = this.orderItems.value.filter((x:any) => x.selected===true && (x.reviewItemStatusId===1 || x.reviewItemStatusId===2 ));
      if (selectedOrderItems.length===0) {
        selectedOrderItems = this.orderItems.value.filter((x:any) => x.reviewItemStatusId===7 ); //select all that are accepted
        if(selectedOrderItems.length===0) {
          this.toastr.error('only items that are reviewed and accepted can be forwarded to agents');
          return;
        }
      }

      let agents = this.associates;
      var title="Choose Associates";
      const config = {
        class: 'modal-dialog-centered modal-lg windowlarge',
        //windowClass: 'large-Modal',
        initialState: {
          title,
          agents
        }
      }

      this.bsModalRef = this.modalService.show(ChooseAgentsModalComponent, config);
      
      this.bsModalRef.content.updateSelectedOfficialIds.subscribe((values: any) => {
            agents: [...values.filter((el: any) => el.checked === true)];
            for( var i = 0; i < agents.length; i++){ 
              if ( agents[i].checked) agents.splice(i,1);
            }
        });
        
        if(agents.length===0) {
          this.toastr.warning('no associates selected', 'abort');
          return;
        }
        
        var orderitemids: number[]=[];
        var associateids: number[]=[];
        selectedOrderItems.forEach((x: IOrderItem) => {orderitemids.push(x.id);})
        agents.forEach(x => {associateids.push(x.officialId);})
        
        let ids: IOfficialIdsAndOrderItemIdsDto | undefined;  
        ids!.officialIds=associateids;
        ids!.orderItemIds=orderitemids;
        
        if(ids) {
            //check for unique constraints - OrderItemId, Dateforwarded.Date, OfficialId ** TODO **
            this.orderFwdService.forwardDLtoSelectedAgents(ids).subscribe((response) => {
              if(response ==='' || response===null) {
                this.toastr.success('Selected Order Categories forwarded to selected Associates', 'Success!');
              } else {
                this.toastr.error('Error forwarding the DLs to Associates' + response, 'Failure');
                console.log(response);
              }
            }, error => {
              this.toastr.error(error);
            })
        }
          
      }
  
    forwardOrderToHRDept() {

    }
  
    assignTasksToHRExecs() {
      if(this.isAddMode) return;
     
      if(this.member?.projectManagerId===0) {
        this.toastr.warning('Project Manager needs to be defined before tasks can be assiged');
        return;
      }
   
      if (this.isAddMode) {
        this.toastr.error('tasks cannot be assigned while in add mode.  Save the data first and then comeback to this form in edit mode to assign HR Executive tasks');
        return;
      }

      let f = this.form.value;
      
      var itemids = f.orderItems.filter((x:any) => x.checked===true).map((x:IOrderItem) => x.id);
      
      return this.taskService.createOrderAssignmentTasks(itemids).subscribe((response: string) => {
        if(response === '') {
          this.toastr.success('tasks created for the chosen order items', "Success");
        } else {
          this.toastr.warning('failed to create the tasks', 'Failure');
        }
      }, error => {
        this.toastr.error(error, 'failed to create tasks for the chosen order items');
      })

      
    }      

    customerChange(event: any) {
      if(event ===null) return;
      this.form.get('cityOfWorking')!.setValue(event.city);
      this.form.get('country')!.setValue(event.country);
      //this.selectedCustomerName = this.sharedService.getDropDownText(this.mySelect, this.customers[0].customerName);

    }

    openContractReviewItemModal(index: any) 
    {
        var orderitembrief = this.getControls()[index].value;
        var orderitemid = orderitembrief.id;
       
        this.contractRvwService.getContractReviewItem(orderitemid).subscribe(
          response => {
            if(response) 
              {
                const config = {
                  class: 'modal-dialog-centered modal-lg',
                  initialState: {
                  reviewItem: response
                }}
      
                this.bsModalRef = this.modalService.show(OrderItemReviewModalComponent, config);
      
                this.bsModalRef.content.updateModalReview.subscribe((response: string) => {
                    if(response !== '') {
                      this.toastr.warning("Failed to update the Checklist object -" + response);
                    } else {
                      this.toastr.success('Updated the checklist object');
                    }
                })
            } else {
              this.toastr.warning('Failed to get the contract review item object from api')
            }
          }
        )
    }
  
    openAssessmentModal(orderitemid: number) {
      if(orderitemid===undefined || orderitemid===0) return;
      this.router.navigateByUrl('/orders/itemassess/' + orderitemid);
    }

    showProspectives(srno: number) {
      this.router.navigateByUrl('/prospectives/prospectivelist/' + this.member?.orderNo + '-' + srno );
    }

    close() {
      this.router.navigateByUrl(this.returnUrl);
    }

    assessItem(index: number){
      var orderitemid = this.orderItems.at(index).get('id')?.value;

      //var orderitembrief = this.getControls()[index].value;
      //var orderitemid = orderitembrief.id;
      console.log('orderitembrief', orderitemid);
      this.navigateByRoute('/administration/orderassessmentitem/' + orderitemid, null, true);
    }

    formChanged() {
      console.log('form changed event');
    }

    qntyChanged(index: number) {
      if(+this.getMinCVs(index) > 0  || +this.getMaxCVs(index) > 0) return;

      var q = +this.getQnty(index);
      this.setMinCVs(index, q*3);
      this.setMaxCVs(index, q*3);
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
              returnUrl: '/orders/edit/' + this.routeId
            } }
        );
    }
}
