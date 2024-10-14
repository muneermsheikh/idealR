import { NgModule, resolveForwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeployListingComponent } from './deploy-listing/deploy-listing.component';
import { RouterModule } from '@angular/router';
import { DeployStatusAndSeqResolver } from '../_resolvers/deploys/deployStatusAndSeqResolver';
import { DeployStatusListComponent } from './deploy-status-list/deploy-status-list.component';
import { DeploymentStatusResolver } from '../_resolvers/deploys/deploymentStatusResolver';
import { CandidateFlightsComponent } from './candidate-flights/candidate-flights.component';
import { CandidateFlightResolver } from '../_resolvers/deploys/candidateFlightResolver';
import { CandidateFlightHeaderComponent } from './candidate-flight-header/candidate-flight-header.component';


const routes = [
  {path: '', component: DeployListingComponent,

    resolve: {
      statusNameAndSeq: DeployStatusAndSeqResolver
    }
  },

  /*{path: 'candidateFlight:/id', component: CandidateFlightsComponent, 
    resolve: {
      cFlight: CandidateFlightResolver
    }
  },
*/
  
  {path: 'candidateflights', component: CandidateFlightHeaderComponent},

  {path: 'deployStatus', component: DeployStatusListComponent,
      resolve: {
        statuslist: DeploymentStatusResolver
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
