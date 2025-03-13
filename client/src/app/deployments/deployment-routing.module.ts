import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeployListingComponent } from './deploy-listing/deploy-listing.component';
import { RouterModule } from '@angular/router';
import { DeployStatusAndSeqResolver } from '../_resolvers/deploys/deployStatusAndSeqResolver';
import { DeployStatusListComponent } from './deploy-status-list/deploy-status-list.component';
import { DeploymentStatusResolver } from '../_resolvers/deploys/deploymentStatusResolver';
import { CandidateFlightHeaderComponent } from './candidate-flight-header/candidate-flight-header.component';
import { DeploymentsPagedResolver } from '../_resolvers/deploys/deploymentsPagedResolver';
import { CandidateFlightResolver } from '../_resolvers/deploys/candidateFlightResolver';
import { CandidateFlightsPagedResolver } from '../_resolvers/deploys/candidateFlightsPagedResolver';


const routes = [
  {path: '', component: DeployListingComponent,

    resolve: {
      statusNameAndSeq: DeployStatusAndSeqResolver,
      deps: DeploymentsPagedResolver,
      depStatuses: DeploymentStatusResolver
    }
  },

  /*{path: 'candidateFlight:/id', component: CandidateFlightsComponent, 
    resolve: {
      cFlight: CandidateFlightResolver
    }
  },
*/
  
  {path: 'candidateflights', component: CandidateFlightHeaderComponent,
    resolve: {
      cFlights: CandidateFlightsPagedResolver
    }
  },

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
