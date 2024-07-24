import { Pipe, PipeTransform } from '@angular/core';
import { DeployService } from '../_services/deploy.service';
import { IDeployStage } from '../_models/masters/deployStage';
import { IDeploymentStatus } from '../_models/masters/deployStatus';

@Pipe({
  name: 'depNextSeq'
})
export class DepNextSeqPipe implements PipeTransform {

  statuses: IDeploymentStatus[]=[];

  constructor(private depService: DeployService){
    depService.getDeployStatuses().subscribe({
      next: (response: IDeploymentStatus[]) => this.statuses = response
    })
  }

  
  transform(value: number): number {
    var dto = this.statuses.filter(x => x.sequence === value).map(x => x.nextSequence)[0];
    if(dto===null || dto===undefined) return 0;
    return dto;
  }

}
