import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IDeployStage } from 'src/app/_models/masters/deployStage';
import { IDeploymentStatus } from 'src/app/_models/masters/deployStatus';

@Component({
  selector: 'app-deploy-status-list',
  templateUrl: './deploy-status-list.component.html',
  styleUrls: ['./deploy-status-list.component.css']
})
export class DeployStatusListComponent implements OnInit {

  status: IDeploymentStatus[]=[];

  constructor(private activatedRoute: ActivatedRoute){}

  ngOnInit(): void {
      this.activatedRoute.data.subscribe({
        next: data => {
          this.status= data['statuslist']
        }
      })
  }

  getSeqNameFromSeq(seq: number): string {
    var index= this.status.findIndex(x => x.sequence == seq);
    return index===-1 ? 'Invalid Seq' : this.status[index].statusName;
  }
}
