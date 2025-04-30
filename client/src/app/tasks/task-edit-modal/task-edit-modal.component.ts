import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IApplicationTask } from 'src/app/_models/admin/applicationTask';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { User } from 'src/app/_models/user';
import { TaskService } from 'src/app/_services/admin/task.service';

@Component({
  selector: 'app-task-edit-modal',
  templateUrl: './task-edit-modal.component.html',
  styleUrls: ['./task-edit-modal.component.css']
})
export class TaskEditModalComponent implements OnInit{

  @Input() task: IApplicationTask | undefined;
  @Output() updateTaskEvent = new EventEmitter<IApplicationTask>();

  @Input() user: User | undefined;

  empIdAndNames: IEmployeeIdAndKnownAs[]=[];

  form: FormGroup = new FormGroup({});

  bsValueDate = new Date();
  taskStatuses = [{"status": "Pending", "Value": "pending"},
    {"status": "Canceled", "Value": "canceled"}, {"status": "All Users", "Value": "allusers"}, 
    {"status": "only Admin", "Value": "admin"}];
  
  
  constructor(private service: TaskService, private fb: FormBuilder, public bsModalRef: BsModalRef){}

  ngOnInit(): void {
      this.service.getEmployeeIdAndKnownAs().subscribe({
        next: (response:IEmployeeIdAndKnownAs[]) => {
          this.empIdAndNames = response
        }
      })

      this.Initialize(this.task!);
  }

  Initialize(t: IApplicationTask) {
    this.form = this.fb.group({          
        id: [t.id],
        taskType: [t.taskType, Validators.required],
        taskDate: [t.taskDate, Validators.required],
        assignedToUsername: [t.assignedToUsername, Validators.required],
        assignedByUsername: [t.assignedByUsername, Validators.required],
        taskDescription: [t.taskDescription, Validators.required],
        completeBy: [t.completeBy, Validators.required],
        taskStatus: [t.taskStatus, Validators.required],
        completedOn: [t.completedOn],

        taskItems: this.fb.array(
            t.taskItems.map(x => (
            this.fb.group({
                id: x.id,
                appTaskId: x.appTaskId,
                transactionDate: [x.transactionDate, Validators.required],
                userName: [x.userName],
                taskItemDescription: [x.taskItemDescription, Validators.required],
                nextFollowupOn: [x.nextFollowupOn],
                taskStatus: [x.taskStatus, Validators.required]
            })
          ))
        )
      })
    }

    newTaskItem() {
      return this.fb.group({
          id: 0,
          appTaskId: this.task?.id,
          transactionDate: [new Date(), Validators.required],
          userName: [this.user?.userName, Validators.required],
          taskItemDescription: ['', Validators.required],
          nextFollowupOn: [''],
          taskStatus: ['pending', Validators.required]
      })
    }

    get taskItems(): FormArray {
      return this.form.get('taskItems') as FormArray
    }

    AddNewTaskItem() {
      this.taskItems.push(this.newTaskItem());
    }

    removeItem(index: number) {
      this.taskItems.removeAt(index);
    }

    updateTask() {
      var formval=this.form.value;
      console.log('taskeditmodal', formval);
      this.updateTaskEvent.emit(formval);
      this.bsModalRef.hide();
    }

}
