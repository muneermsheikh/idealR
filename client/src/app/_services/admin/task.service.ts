import { Injectable } from '@angular/core';
import { ReplaySubject, map, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from 'src/app/_models/user';
import { Pagination } from 'src/app/_models/pagination';
import { TaskParams } from 'src/app/_models/params/Admin/taskParams';
import { IApplicationTaskInBrief } from 'src/app/_models/admin/applicationTaskInBrief';
import { IApplicationTask } from 'src/app/_models/admin/applicationTask';
import { getHttpParamsForTask, getPaginatedResult } from '../paginationHelper';
import { AccountService } from '../account.service';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { MedicalParams } from 'src/app/_models/admin/objectives/medicalParams';
import { IMedicalObjective } from 'src/app/_models/admin/objectives/medicalObjective';
import { IMedObj, MedObj } from 'src/app/_models/admin/objectives/medObj';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  apiUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  
  oParams = new TaskParams();
  pagination: Pagination | undefined;
  medPagination: Pagination|undefined;

  tasks: IApplicationTaskInBrief[]=[];
  cache = new Map();
  user?: User;
  cacheTasks = new Map();

  medParams = new MedicalParams();

  constructor(private http: HttpClient, private toastr:ToastrService, private accountService: AccountService) {
    accountService.currentUser$.subscribe({
      next: response => this.user = response!
    })
   }

 

  addTask(task: IApplicationTask)
  {
    return this.http.post<IApplicationTask>(this.apiUrl + 'Task', task);
  }

  
  setMedParams(params: MedicalParams) {
    this.medParams = params;
  }
  
  getMedParams() {
    return this.medParams;
  }

  getPaginatedTasks(): any {     //returns IPaginationAppTask
    
    var hparams=this.oParams;

    const response = this.cache.get(Object.values(hparams).join('-'));
    if(response) return of(response);

    let params = getHttpParamsForTask(hparams);

    return getPaginatedResult<IApplicationTaskInBrief[]>(this.apiUrl + 
      'Task/pagedTasks', params, this.http).pipe(
      map(response => {
        this.cache.set(Object.values(hparams).join('-'), response);
        return response;
      })
    )
    
  }
  
  getTaskWithItems(id: number) {
    return this.http.get<IApplicationTask>(this.apiUrl + 'task/taskbyid/' + id);
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
    return this.http.put<IApplicationTask>(this.apiUrl + 'Task/task', model)
  }

  deleteTaskFromCache(id: number) {
    this.cacheTasks.delete(id);
    this.pagination!!.totalItems--;
  }

  deleteTask(taskid: number) {
    return this.http.delete<string>(this.apiUrl + 'Task/task/' + taskid);
    }
  
    
  completeTask(taskid: number) {
    return this.http.put<string>(this.apiUrl + 'Task/completeTask/' + taskid, {} );
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
    taskParams.assignedByUsername = this.user?.userName!;
    taskParams.assignedToUsername = this.user?.userName!;
    this.oParams = taskParams;

    return this.getPaginatedTasks();
  }

  setParams(params: TaskParams) {
    this.oParams = params;
  }
  
  getParams() {
    return this.oParams;
  }

  getEmployeeIdAndKnownAs() {
    return this.http.get<IEmployeeIdAndKnownAs[]>(this.apiUrl + 'employees/idandknownas');
  }

  getMedicalObjectives(fromdate: any, uptodate: any) {

    return this.http.get<IMedicalObjective[]>(this.apiUrl + 'feedback/MedicalObjectives/' + fromdate + '/' + uptodate);
  }
}
