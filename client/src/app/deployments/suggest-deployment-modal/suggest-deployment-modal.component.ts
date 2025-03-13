import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Duration } from 'ngx-bootstrap/chronos/duration/constructor';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { DepItemToAddDto, IDepItemToAddDto } from 'src/app/_dtos/process/depItemToAddDto';
import { IDeploymentPendingBriefDto } from 'src/app/_dtos/process/deploymentPendingBriefDto';
import { INextDepDataDto } from 'src/app/_dtos/process/nextDepDataDto';
import { DeployService } from 'src/app/_services/deploy.service';

@Component({
  selector: 'app-suggest-deployment-modal',
  templateUrl: './suggest-deployment-modal.component.html',
  styleUrls: ['./suggest-deployment-modal.component.css']
})
export class SuggestDeploymentModalComponent implements OnInit {

  @Output() emittedDep = new EventEmitter<boolean>();
  @Input() passportNo: string = '';
  candidateNextProcess: INextDepDataDto | undefined;
  
  constructor(private service: DeployService, 
    private toastr: ToastrService, public bsModalRef: BsModalRef){}

  ngOnInit(): void {
    this.service.getCandidateNextProcess(this.passportNo).subscribe({
      next: (response: INextDepDataDto) => {
        console.log('response', response);
        this.candidateNextProcess = response;
        if(response.errorString !== '') this.toastr.error(response.errorString, 'Error');
        
        console.log('candidateNextProcess', this.candidateNextProcess);
      }
    })
  }

  
    applyNextSuggestion(depid: number, sequence: number) {
        var depItemsToAddDto: IDepItemToAddDto = {depId: depid, 
          sequence: sequence, transactionDate: new Date() };
        var depitems: IDepItemToAddDto[]=[];
        depitems.push(depItemsToAddDto);

        this.service.InsertDepItems(depitems).subscribe({
          next: (response: IDeploymentPendingBriefDto[]) => {
            this.emittedDep.emit(true);
            this.bsModalRef.hide();
          },
          error: (err: any) => this.toastr.error(err.error?.details, 
            'Failed to insert the deployment record', {closeButton: true, timeOut:15000})
        })
    }
}
