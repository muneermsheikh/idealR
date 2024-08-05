import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IApplicationTask } from 'src/app/_models/admin/applicationTask';
import { IApplicationTaskInBrief } from 'src/app/_models/admin/applicationTaskInBrief';
import { TaskService } from 'src/app/_services/admin/task.service';

@Component({
  selector: 'app-task-line',
  templateUrl: './task-line.component.html',
  styleUrls: ['./task-line.component.css']
})
export class TaskLineComponent {

  @Input() task: IApplicationTaskInBrief|undefined;
  @Output() editEvent = new EventEmitter<IApplicationTask>();
  @Output() completedEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  
  constructor(private service: TaskService, private toastr: ToastrService){}

  completedClicked() {
      this.completedEvent.emit(this.task?.id)
  }

  deleteClicked(){
      this.deleteEvent.emit(this.task?.id)
  }

  editClicked() {
    
    if(this.task) {
      this.service.getTask(this.task?.id).subscribe({
        next: (response: IApplicationTask) => {
          if(response) {
            this.editEvent.emit(response)
          }
        }
      })
    }
    
  }


}
