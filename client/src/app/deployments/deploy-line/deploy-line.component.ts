import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IDeploymentPendingBriefDto } from 'src/app/_dtos/process/deploymentPendingBriefDto';
import { IDeploymentPendingDto } from 'src/app/_dtos/process/deploymentPendingDto';
import { IDep } from 'src/app/_models/process/dep';
import { DeployService } from 'src/app/_services/deploy.service';

@Component({
  selector: 'app-deploy-line',
  templateUrl: './deploy-line.component.html',
  styleUrls: ['./deploy-line.component.css']
})
export class DeployLineComponent {

  @Input() dep: IDeploymentPendingBriefDto | undefined;
  @Output() editDepEvent = new EventEmitter<IDep>();
  @Output() deleteDepEvent = new EventEmitter<number>();
  @Output() selectedEvent = new EventEmitter<IDeploymentPendingBriefDto>();
  @Output() showTicket = new EventEmitter<number>();    //emit the CVRefId, to allow calling program to display the ticket
  @Output() editAttachmentEvent = new EventEmitter<IDep>();

  constructor(private service: DeployService, private toastr: ToastrService) { }

  ngOnInit(): void {
    //if(this.dep?.deploySequence) this.sequence = of(this.dep?.deploySequence);
  }

  editDep() {

    if(this.dep) {
      this.service.getDeploymentWithItems(this.dep.depId).subscribe({
        next: (response: IDep) => {
          if(response) {
            this.editDepEvent.emit(response);
            //console.log('emitted from dep line:', response);
          } 
        }
      })
    }
  
 }

 showFlight() {
  if(this.dep!.deploySequence !== 1300) {
    this.toastr.warning('This can be called only when there is a ticket booked');
    return;
  }

  this.showTicket.emit(this.dep?.cvRefId);
 }
 
  deleteDep () {
    this.deleteDepEvent.emit(this.dep!.depId);
  }

  selectedClicked() {
      this.selectedEvent.emit(this.dep)
  }

  editAttachment() {
    if(this.dep) {
      this.service.getDeploymentWithItems(this.dep.depId).subscribe({
        next: (response: IDep) => {
          if(response) {
            this.editAttachmentEvent.emit(response);
          } 
        }
      })
    }
  }
  
}
