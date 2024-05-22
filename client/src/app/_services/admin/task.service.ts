import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { Pagination } from 'src/app/_models/pagination';
import { TaskParams } from 'src/app/_models/params/Admin/taskParams';
import { IApplicationTaskInBrief } from 'src/app/_models/admin/applicationTaskInBrief';
import { IApplicationTask } from 'src/app/_models/admin/applicationTask';
import { getHttpParamsForTask, getPaginatedResult } from '../paginationHelper';
import { ITaskItem } from 'src/app/_models/admin/taskItem';
import { IOrderItemIdAndHRExecEmpNoDto } from 'src/app/_dtos/admin/orderItemIdAndHRExecEmpNoDto';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  oParams = new TaskParams();
  pagination: Pagination | undefined;
  tasks: IApplicationTaskInBrief[]=[];
  cache = new Map();
  cacheTasks = new Map();

  constructor(private http: HttpClient, private toastr:ToastrService) { }

  createOrderAssignmentTasks(assignmentTasks: IOrderItemIdAndHRExecEmpNoDto[]) {
    return this.http.post(this.apiUrl + 'Task/assignToHRExecs', assignmentTasks);
  }

  addTask(task: IApplicationTask)
  {
    return this.http.post<IApplicationTask>(this.apiUrl + 'Task', task);
  }

  getTasks(oParams: TaskParams): any {     //returns IPaginationAppTask
    
    const response = this.cache.get(Object.values(oParams).join('-'));
    if(response) return of(response);

    let params = getHttpParamsForTask(oParams);

    return getPaginatedResult<IApplicationTaskInBrief[]>(this.apiUrl + 
      'Task/pagedTasks', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(oParams).join('-'), response);
        return response;
      })
    )
    
  }
  
  getTask(id: number) {
    return this.http.get<IApplicationTask>(this.apiUrl + 'Task/taskbyid/' + id);
  }

  getTaskByOrderIdAndTaskType(orderid: number, tasktype: string) {
    return this.http.get<IApplicationTask>(this.apiUrl + 'Task/task/' + orderid + '/' + tasktype);
  }

  getTaskFromResumeId(resumeid: string) {

    return this.http.get<IApplicationTask>(this.apiUrl + 'task/getorcreatebyresumeid/' + resumeid);
  }

  getOrCreateTaskFromParams(resumeid: string, assignedToUsername: string) {
    var tparams = this.oParams;
    tparams.resumeId=resumeid;
    tparams.taskType= "Prosective";    //prospective candidate followup
    tparams.assignedToUsername = assignedToUsername;
    return this.http.post<IApplicationTask>(this.apiUrl + 'Task/prospectivetaskForResumeId', tparams);
  }

  createNewTask(model: IApplicationTask) {
    return this.http.post<IApplicationTask>(this.apiUrl + 'Task/task', model);  // also composes email msg to customer
  }
  
  UpdateTask(model: IApplicationTask) {
    return this.http.put<string>(this.apiUrl + 'Task/task', model)
  }

  deleteTaskFromCache(id: number) {
    this.cacheTasks.delete(id);
    this.pagination!!.totalItems--;
  }

  deleteTask(taskid: number) {
    return this.http.delete<string>(this.apiUrl + 'Task/task/' + taskid);
    }
  
    
  completeTask(taskid: number) {
    return this.http.put(this.apiUrl + 'Task/completeTask/' + taskid, {} );
  }

  //following not updated wrt url
  deleteTaskItem(taskitemid: number) {
    return this.http.delete(this.apiUrl + 'Task/taskitem/' + taskitemid)
      .subscribe(() => {
      }, error => {
        this.toastr.error(error);
        console.log('error in deleting task', error);
      })
  }



  getTaskFromAny(id: string) {
    return this.http.get<IApplicationTask>(this.apiUrl + 'task/fromany/' + id);
  }


  getPendingTasksOfLoggedInUser() {
    var taskParams = new TaskParams();
    taskParams.pageNumber=1;
    taskParams.pageSize=15;
    this.oParams = taskParams;

    return this.getTasks(taskParams);
  }

  setOParams(params: TaskParams) {
    this.oParams = params;
  }
  
  getOParams() {
    return this.oParams;
  }
}
