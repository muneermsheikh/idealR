import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { Pagination } from '../_models/pagination';
import { IApplicationTaskInBrief } from '../_models/admin/applicationTaskInBrief';
import { IOrderAssignmentDto } from '../_dtos/admin/orderAssignmentDto';
import { IApplicationTask } from '../_models/admin/applicationTask';
import { getHttpParamsForTask, getPaginatedResult} from './paginationHelper';
import { ITaskItem } from '../_models/admin/taskItem';
import { TaskParams } from '../_models/params/Admin/taskParams';

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

  createOrderAssignmentTasks(assignmentTasks: IOrderAssignmentDto[]) {
    return this.http.post(this.apiUrl + 'orderassignment/orderitems', assignmentTasks);
  }

  createTaskFromAppTask(task: IApplicationTask)
  {
    return this.http.post<IApplicationTask>(this.apiUrl + 'task/create', task);
  }

  getPendingTasksOfLoggedInUser() {
    return this.http.get<IApplicationTaskInBrief[]>(this.apiUrl + 'task/pendingtasksofloggedinuser/' + 
      this.oParams.pageNumber + '/' + this.oParams.pageSize);
  }

  setOParams(params: TaskParams) {
    this.oParams = params;
  }
  
  getOParams() {
    return this.oParams;
  }
  

  getTasksPaged(oParams: TaskParams): any {     //returns IPaginationAppTask
    
      const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = getHttpParamsForTask(oParams);
        
      return getPaginatedResult<IApplicationTaskInBrief[]>(this.apiUrl + 'task/paginatedtasksOfloggedinuser', 
        params, this.http).pipe(map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
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
    tparams.taskType="Prospective";    //No. 27
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
    this.pagination!.totalItems--;
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

}
