import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeployListingComponent } from './deploy-listing/deploy-listing.component';
import { DeployEditModalComponent } from './deploy-edit-modal/deploy-edit-modal.component';
import { DeployAddModalComponent } from './deploy-add-modal/deploy-add-modal.component';
import { DeployLineComponent } from './deploy-line/deploy-line.component';
import { DepStatusPipe } from './dep-status.pipe';
import { DepNextSeqPipe } from './dep-next-seq.pipe';
import { SharedModule } from '../_modules/shared.module';
import { DeploymentRoutingModule } from './deployment-routing.module';
import { ChooseFlightModalComponent } from './choose-flight-modal/choose-flight-modal.component';
import { FlightDetailModalComponent } from './flight-detail-modal/flight-detail-modal.component';



@NgModule({
  declarations: [
    DeployListingComponent,
    DeployEditModalComponent,
    DeployAddModalComponent,
    DeployLineComponent,
    DepStatusPipe,
    DepNextSeqPipe,
    ChooseFlightModalComponent,
    FlightDetailModalComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    DeploymentRoutingModule
  ]
})
export class DeploymentModule { }
