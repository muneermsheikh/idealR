import { Pipe, PipeTransform } from '@angular/core';
import { DeployService } from '../_services/deploy.service';
import { IDeployStage } from '../_models/masters/deployStage';

@Pipe({
  name: 'depNextSeq'
})
export class DepNextSeqPipe implements PipeTransform {

  statuses: IDeployStage[]=[];

  constructor(private depService: DeployService){
    depService.getDeployStatuses().subscribe({
      next: (response: IDeployStage[]) => this.statuses = response
    })
  }

  
  transform(value: number): number {
    var dto = this.statuses.filter(x => x.sequence === value).map(x => x.nextSequence)[0];
    if(dto===null || dto===undefined) return 0;
    return dto;
  }

}
