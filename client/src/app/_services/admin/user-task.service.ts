import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { environment } from 'src/app/environments/environment';
import { IUser } from '../../models/admin/user';
import { userTaskParams } from '../../params/admin/userTaskParams';
import { IPagination } from '../../models/pagination';
import { IApplicationTaskInBrief } from '../../models/admin/applicationTaskInBrief';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { IOrderAssignmentDto } from '../../dtos/admin/orderAssignmentDto';
import { IApplicationTask } from '../../models/admin/applicationTask';
import { ITaskType } from '../../models/admin/taskType';
import { ITaskItem } from '../../models/admin/taskItem';
import { IContactResult } from '../../models/admin/contactResult';

@Injectable({
  providedIn: 'root'
})
export class UserTaskService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  oParams = new userTaskParams();
  pagination: IPagination<IApplicationTaskInBrief[]> | undefined; // = new PaginationTaskInBrief();
  tasks: IApplicationTaskInBrief[]=[];
  cache = new Map();
  cacheTasks = new Map();

  constructor(private http: HttpClient, private toastr:ToastrService) { }

  createOrderAssignmentTasks(assignmentTasks: IOrderAssignmentDto[]) {
    return this.http.post(this.apiUrl + 'orderassignment/orderitems', assignmentTasks);
  }

  createTaskFromAppTask(task: IApplicationTask)
  {
    return this.http.post<IApplicationTask>(this.apiUrl + 'task/create', task);
  }

  getPendingTasksOfLoggedInUser() {
    var taskParams = new userTaskParams();
    taskParams.pageIndex=1;
    taskParams.pageSize=15;
    this.oParams = taskParams;
    //return this.http.get<IApplicationTaskInBrief[]>(this.apiUrl + 'task/paginatedtasksOfloggedinuser/' + this.oParams.pageIndex + '/' + this.oParams.pageSize);
    return this.getTasks(false);
  }

  setOParams(params: userTaskParams) {
    this.oParams = params;
  }
  
  getOParams() {
    return this.oParams;
  }
  
  getTaskTypes(){
    return this.http.get<ITaskType[]>(this.apiUrl + 'task/tasktypes');
  }

  getTasks(useCache: boolean): any {     //returns IPaginationAppTask
    
    if (useCache === false)  this.cache = new Map();

    if (this.cache.size > 0 && useCache === true) {
      if (this.cache.has(Object.values(this.oParams).join('-'))) {
        this.pagination = this.cache.get(Object.values(this.oParams).join('-'));
        return of(this.pagination);
      }
    }
    
    let params = new HttpParams();
    if (this.oParams.taskStatus !== "" && this.oParams.taskStatus !== undefined ) params = params.append('taskStatus', this.oParams.taskStatus); 
    if (this.oParams.orderId !== 0 && this.oParams.orderId !== undefined) params = params.append('orderId', this.oParams.orderId.toString()); 
    if (this.oParams.assignedToId !== 0 && this.oParams.assignedToId !== undefined) params = params.append('assignedToId', this.oParams.assignedToId?.toString()); 
    if (this.oParams.assignedToNameLike !== '' && this.oParams.assignedToNameLike !== undefined ) params = params.append('assignedToNameLike', this.oParams.assignedToNameLike); 
    if (new Date(this.oParams.taskDate).getFullYear() > 2000) params = params.append('taskDate', this.oParams.taskDate.toString()); 
    if(this.oParams.candidateId !==0 && this.oParams.candidateId !== undefined) {
      params = params.append('candidateId', this.oParams.candidateId.toString());
      params = params.append('personType', 'candidate');
    }
    
    if (this.oParams.search) {
      params = params.append('search', this.oParams.search);
    }
    
    params = params.append('sort', this.oParams.sort);
    params = params.append('pageIndex', this.oParams.pageIndex.toString());
    params = params.append('pageSize', this.oParams.pageSize.toString());

    return this.http.get<IPagination<IApplicationTaskInBrief[]>>
        (this.apiUrl + 'task/paginatedtasksOfloggedinuser', {params}).pipe(
        map(response => {
          this.cache.set(Object.values(this.oParams).join('-'), response)
          this.pagination = response;
          return response;
        })
      )
  }
  
  getTask(id: number) {
    return this.http.get<IApplicationTask>(this.apiUrl + 'task/byid/' + id);
  }

  getTaskByOrderIdAndTaskType(orderid: number, tasktypeid: number) {
    return this.http.get<IApplicationTask>(this.apiUrl + 'task/byorderidandtaktype/' + orderid + '/' + tasktypeid);
  }

  getTaskFromResumeId(resumeid: string) {

    return this.http.get<IApplicationTask>(this.apiUrl + 'task/getorcreatebyresumeid/' + resumeid);
  }

  getTaskItem(taskitemid: number) {
    return this.http.get<ITaskItem>(this.apiUrl + 'task/taskitem/' + taskitemid);
  }

  getOrCreateTaskFromParams(resumeid: string) {
    var tparams = this.oParams;
    tparams.resumeId=resumeid;
    tparams.taskTypeId=27;    //prospective candidate followup
    return this.http.post<IApplicationTask>(this.apiUrl + 'task/getorcreatetaskfromparams', tparams);
  }

  createNewTask(model: IApplicationTask) {
    return this.http.post(this.apiUrl + 'task/create', model);  // also composes email msg to customer
  }
  
  UpdateTask(model: any) {
    return this.http.put(this.apiUrl + 'task', model)
  }

  CreateNewTask(model: any) {
    return this.http.post(this.apiUrl + 'task', model)
  }

  deleteTaskItem(taskitemid: number) {
    return this.http.delete(this.apiUrl + '/task/taskitem/' + taskitemid)
      .subscribe(() => {
      }, error => {
        this.toastr.error(error);
        console.log('error in deleting task', error);
      })
  }

  deleteTaskFromCache(id: number) {
    this.cacheTasks.delete(id);
    this.pagination!.count--;
  }

  deleteTask(taskid: number) {
    return this.http.delete(this.apiUrl + 'task/task/' + taskid);
    }
  
  completeTask(taskid: number) {
    return this.http.put(this.apiUrl + 'task/applicationtask/' + taskid, {} );
  }

  getTaskFromAny(id: string) {
    return this.http.get<IApplicationTask>(this.apiUrl + 'task/fromany/' + id);
  }

  getContactResults() {
    return this.http.get<IContactResult[]>(this.apiUrl + 'UserHistory/contactresults');
  }
}
