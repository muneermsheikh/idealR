import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrComponentlessModule, ToastrService } from 'ngx-toastr';
import { filter, switchMap, take } from 'rxjs';
import { IApplicationTask } from 'src/app/_models/admin/applicationTask';
import { IApplicationTaskInBrief } from 'src/app/_models/admin/applicationTaskInBrief';
import { Pagination } from 'src/app/_models/pagination';
import { TaskParams } from 'src/app/_models/params/Admin/taskParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { TaskService } from 'src/app/_services/admin/task.service';
import { TaskEditModalComponent } from '../task-edit-modal/task-edit-modal.component';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-user-tasks',
  templateUrl: './user-tasks.component.html',
  styleUrls: ['./user-tasks.component.css']
})
export class UserTasksComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  user?: User;

  constructor(private activatedRoute: ActivatedRoute, 
      private accountService: AccountService, 
      private service: TaskService, 
      private toastr: ToastrService,
      private bsModalService: BsModalService){

        this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
  }

  pagination: Pagination | undefined;

  tasks: IApplicationTaskInBrief[]=[];
  
  sParams = new TaskParams();
  totalCount = 0;

  bsModalRef: BsModalRef | undefined;

  parameterCriteria='';

  ngOnInit(): void {
    
    /*this.activatedRoute.data.subscribe(data => { 
      this.tasks = data['tasks'];
    }) */

    if(this.user) {
        this.sParams.assignedByUsername = this.user?.userName!;
        this.sParams.assignedToUsername = this.user?.userName!;
        this.getTasksPaged();
    
    }
  }

  getTasksPaged() {
    this.service.setParams(this.sParams);
    this.service.getPaginatedTasks().subscribe({
      next: (response: any) => {
        console.log('response:', response);
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
      next: response => {
        if(response) {
          this.toastr.success('Task Marked as Completed', 'Task Completed')
        } else {
          this.toastr.warning('Failed to mark the task as completed', 'Failure')
        }
      },
      error: (err: any) => {
        this.toastr.error(err.error.details, 'Error in marking the task as completed')
      }
    })
  }

  removeTask(taskId: number) {
      this.service.deleteTask(taskId).subscribe({
        next: response => {
          if(response) {
            this.toastr.success('Task Deleted', 'Success')
          } else {
            this.toastr.warning('Faileed to delete the task', 'Failure')
          }
        },
        error: (err: any) => this.toastr.error(err.error.details, 'Error encountered')
      })
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
    this.service.setParams(params);
    this.getTasksPaged();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new TaskParams();
    this.sParams.assignedByUsername = this.user?.userName!;
    this.sParams.assignedToUsername = this.user?.userName!;
    this.service.setParams(this.sParams);
    this.getTasksPaged();
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
      }
    }

    this.bsModalRef = this.bsModalService.show(TaskEditModalComponent, config);

    const observableOuter =  this.bsModalRef.content.updateTaskEvent;
    
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
