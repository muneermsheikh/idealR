import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap, take } from 'rxjs';
import { IApplicationTask } from 'src/app/_models/admin/applicationTask';
import { IApplicationTaskInBrief } from 'src/app/_models/admin/applicationTaskInBrief';
import { Pagination } from 'src/app/_models/pagination';
import { TaskParams } from 'src/app/_models/params/Admin/taskParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { TaskService } from 'src/app/_services/admin/task.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { EditModalComponent } from '../edit-modal/edit-modal.component';
import { values } from 'lodash-es';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-user-tasks',
  templateUrl: './user-tasks.component.html',
  styleUrls: ['./user-tasks.component.css']
})
export class UserTasksComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  user?: User;
  lastStatus="";

  constructor(private activatedRoute: ActivatedRoute, 
      private accountService: AccountService, 
      private service: TaskService, 
      private toastr: ToastrService,
      private bsModalService: BsModalService, private confirm: ConfirmService){

        this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
  }

  pagination: Pagination | undefined;

  tasks: IApplicationTaskInBrief[]=[];
  
  sParams = new TaskParams();

  totalCount = 0;

  bsModalRef: BsModalRef | undefined;

  parameterCriteria='';

  taskStatuses = [{"status": "All Status", "Value": "allstatus"}, {"status": "Pending", "Value": "pending"},
    {"status": "Canceled", "Value": "canceled"}, {"status": "All Users", "Value": "allusers"}, {"status": "only Admin", "Value": "admin"}];
  
  taskTypes = [{"type": "Customer Feedback", "Value": "CustomerFeedbackResponse"},
    {"type": "CV Forward", "Value": "CVFwdTask"},
    {"type": "Employment Acceptance", "Value": "EmploymentAcceptance"},
    {"type": "Selection Followup", "Value": "SelectionFollowupWithClient"},
    {"type": "Prospective Candidates", "Value": "Prospective"}, {"type": "Portal Task", "Value": "PortalTask"},
    {"type": "Order Forward To HR", "Value":"OrderFwdToHR"}, {"type": "Assign Task To HR Exec", "Value": "AssignTaskToHRExec"},
    
  ]

  ngOnInit(): void {
  
    console.log('sParams', this.sParams);
    this.sParams.taskStatus.toLowerCase() === "pending";

    this.getTasksPaged();
  }

  getTasksPaged() {
    this.service.setParams(this.sParams);
    this.service.getPaginatedTasks().subscribe({
      next: (response: any) => {
        if(response.result && response.pagination) {
          this.tasks = response.result;
          this.pagination = response.pagination;
          this.totalCount = response.count;
        }
      },
      error: (err: any) => this.toastr.error(err.error.details, 'Error')
    });
  }

  markAsCompleted(taskId: number) {
    this.service.completeTask(taskId).subscribe({
      next: (response: string) => {
        if(response==='' || response === null) {
          this.toastr.success('Task Marked as Completed', 'Task Completed');
          var index=this.tasks.findIndex(x => x.id === taskId);
          if(index !== -1) {
            if(this.sParams.taskStatus==='Pending') {
              this.tasks.splice(index, 1)
            } else {
              this.tasks[index].taskStatus='Completed';
            }
          } 
        } else {
          this.toastr.warning(response, 'Failed to mark the task as completed')
        }
      },
      error: (err: any) => {
        this.toastr.error(err.error.details, 'Error in marking the task as completed')
      }
    })
  }

  removeTask(taskId: number) {

    var id= taskId;
    var confirmMsg = 'confirm delete this Task. WARNING: this cannot be undone';

    const observableInner = this.service.deleteTask(id);
    const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => {
          return observableInner
        })
    ).subscribe(response => {
      console.log('response post delete', response);
      if(response === '' || response === null) {
        this.toastr.success('Task deleted', 'deletion successful');
        var index = this.tasks.findIndex(x => x.id == taskId);
        if(index !== -1) this.tasks.splice(index, 1);
      } else {
        this.toastr.error(response, 'failed to delete')
      } error: (err: any) => this.toastr.error(err.error.details, 'Error enccountered')
    });

  }

  onPageChanged(event: any){
    const params = this.service.getParams();
    if (this.sParams.pageNumber !== event.page) {
      this.sParams.pageNumber = event.page;
      this.service.setParams(this.sParams);
      this.getTasksPaged();
    }
  }

  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.getTasksPaged();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new TaskParams();
    this.sParams.assignedByUsername = this.user?.userName!;
    this.sParams.assignedToUsername = this.user?.userName!;
    this.getTasksPaged();
  }

  applyStatus() {
    console.log('sParams.Status', this.sParams.taskStatus, 'laststatus:', this.lastStatus);
    if(this.sParams.taskStatus===null || (this.lastStatus !== this.sParams.taskStatus)) {
      this.lastStatus = this.sParams.taskStatus || "pending";
      this.sParams.pageNumber=1;
      this.getTasksPaged();
    }
  }

  
 editDeploymentModal(task: IApplicationTask){

  

  if(task === null) {
    this.toastr.warning('No Task object returned from Task line');
    return;
  }  

  const config = {
      class: 'modal-dialog-centered modal-lg',
      initialState: {
        task,
        user: this.user
      }
    }

    this.bsModalRef = this.bsModalService.show(EditModalComponent, config);

    const observableOuter =  this.bsModalRef.content.updateEvent;
    
    observableOuter.pipe(
      filter((response: IApplicationTask) => response !==null),
      switchMap((response: IApplicationTask) =>  {
        return this.service.UpdateTask(response)
      })
    ).subscribe((response: IApplicationTask) => {

      if(response !== null) {
        this.toastr.success('Deployment updated', 'Success');
        //**TODO- Update DOM with new values */
      } else {
        this.toastr.warning(response, 'Failure');
      }
      
    })
      
}



}
