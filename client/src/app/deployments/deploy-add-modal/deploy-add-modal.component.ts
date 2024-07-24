import { Component, EventEmitter, Input, ViewChild } from '@angular/core';
import { DepItem, IDepItem } from 'src/app/_models/process/depItem';
import { DeployService } from 'src/app/_services/deploy.service';
import { IDeployStage } from 'src/app/_models/masters/deployStage';
import { NgForm } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { IDeploymentStatus } from 'src/app/_models/masters/deployStatus';

@Component({
  selector: 'app-deploy-add-modal',
  templateUrl: './deploy-add-modal.component.html',
  styleUrls: ['./deploy-add-modal.component.css']
})
export class DeployAddModalComponent {

  @ViewChild('editForm') editForm: NgForm | undefined;
  
  @Input() cvRefId: number = 0;
  
  depItem = new DepItem();

  @Input() updateDepItem = new EventEmitter<IDepItem>();

  candidateName: string = '';
  categoryRef = '';
  companyName = '';
  ecnr = false;
  
  depStatuses: IDeploymentStatus[]=[];
  
  bsValue = new Date();
  bsRangeValue= new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();
  
  constructor(public bsModalRef: BsModalRef, private toastr:ToastrService, 
    private confirmService: ConfirmService, private service: DeployService) {
      this.service.getDeployStatuses().subscribe({
        next: (response:IDeploymentStatus[]) => this.depStatuses = response
      })

    }


  ngOnInit(): void {

      if(this.cvRefId !== 0) {
        this.service.getNextDepItemFromCVRefId(this.cvRefId).subscribe({
          next: (response: IDepItem) => {
            if(response !== null) {
              this.depItem = response
            }
          }
        })
      }
  }

  saveDepItem() {

    this.confirmService.confirm('cofirm Save', 'Press Ok to proceed').subscribe({
      next: (response: boolean) => {
        if(response) {
          this.updateDepItem.emit(this.editForm?.value);
          this.bsModalRef.hide();
        }
      }
    })

  }

  close() {
      this.bsModalRef.hide();
  }
    
}
