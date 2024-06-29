import { NgModule, resolveForwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeployListingComponent } from './deploy-listing/deploy-listing.component';
import { RouterModule } from '@angular/router';
import { DeployStatusAndSeqResolver } from '../_resolvers/deploys/deployStatusAndSeqResolver';


const routes = [
  {path: '', component: DeployListingComponent,

    resolve: {
      statusNameAndSeq: DeployStatusAndSeqResolver
    }
  }
]
   
@NgModule({
  declarations: [],

  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],

  exports: [
    RouterModule
  ]
})
export class DeploymentRoutingModule { }
