import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IApplicationTask } from 'src/app/_models/admin/applicationTask';
import { IEmployeeIdAndKnownAs } from 'src/app/_models/admin/employeeIdAndKnownAs';
import { User } from 'src/app/_models/user';
import { TaskService } from 'src/app/_services/admin/task.service';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-task-edit-modal',
  templateUrl: './task-edit-modal.component.html',
  styleUrls: ['./task-edit-modal.component.css']
})
export class TaskEditModalComponent implements OnInit {

  @Input() task: IApplicationTask | undefined;
  @Output() updateTaskEvent = new EventEmitter<IApplicationTask>();
  
  bsValue = new Date();
  bsRangeValue= new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();
  empIdAndNames: IEmployeeIdAndKnownAs[]=[];

  user?: User;

  form: FormGroup = new FormGroup({});

  taskStatuses= [
    {status: 'Not Started'},{status: 'In-process'}, {status: 'Completed'}, 
    {status: 'Canceled'}
  ]

  constructor(public bsModalRef: BsModalRef, private toastr: ToastrService,
    private confirm: ConfirmService, private fb: FormBuilder,
    private service: TaskService ){}
  
  ngOnInit(): void {
      this.service.getEmployeeIdAndKnownAs().subscribe({
        next: response => this.empIdAndNames = response 
      })

      if(this.task) this.InitializeForm(this.task)
  }

  InitializeForm(t: IApplicationTask) {

      this.form = this.fb.group({
        id:t.id,
        taskType: [t.taskType, Validators.required],
        taskDate: [t.taskDate, Validators.required],
        assignedToUsername: [t.assignedToUsername, Validators.required],
        assignedByUsername: [t.assignedByUsername, Validators.required],
        taskDescription: [t.taskDescription, Validators.required],
        completeBy: [t.completeBy, Validators.required],
        taskStatus: [t.taskStatus, Validators.required],
        completedOn: [t.completedOn],
        orderId: [t.orderId],
        orderNo: [t.orderNo],
        orderItemId: [t.orderItemId],
        candidateId: [t.candidateId],

        taskItems: this.fb.group(
          t.taskItems.map(x => (
            this.fb.group({
              id: x.id,
              appTaskId: x.appTaskId,
              transactionDate: [x.transactionDate, Validators.required],
              taskItemDescription: [x.taskItemDescription, Validators.required],
              userName: [x.userName, Validators.required],
              nextFollowupOn: [x.nextFollowupOn],
              nextFollowupByName: [x.nextFollowupByName]
            })
          ))
        )
      })
  }

  get taskItems(): FormArray {
    return this.form.get('taskItems') as FormArray
  }

  newTaskItem(): FormGroup {
    return this.fb.group({
      id: 0,
      appTaskId: this.task?.id,
      transactionDate: [new Date(), Validators.required],
      taskItemDescription: ['', Validators.required],
      userName: [this.user?.userName, Validators.required],
      nextFollowupOn: [new Date() ],
      nextFollowupByName: ['']
    })
  }

  addTaskItem() {
    this.taskItems.push(this.newTaskItem)
  }

  removeTaskItem(index: number) {

    var msg = "Confirm delete this task Item. " +
        " THE DELETION WILL TAKE EFFECT ONLY AFTER YOU UPDATE THIS FORM"
    
    this.confirm.confirm("Confirm Delete", msg).subscribe({next: confirmed => {
        if(confirmed) {
            this.taskItems.removeAt(index);
        }
      }})
  }

  updateTask() {
    
    var formdata = this.form.value;

    this.updateTaskEvent.emit(formdata);
    this.bsModalRef.hide();

  }

  
}
