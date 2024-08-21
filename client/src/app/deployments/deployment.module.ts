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
import { DeployStatusListComponent } from './deploy-status-list/deploy-status-list.component';
import { DepNextSeqNamePipe } from './dep-next-seq-name.pipe';
import { DepAttachmentModalComponent } from './dep-attachment-modal/dep-attachment-modal.component';
import { MatIconModule } from '@angular/material/icon';



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
    DeployStatusListComponent,
    DepNextSeqNamePipe,
    DepAttachmentModalComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    DeploymentRoutingModule,
    MatIconModule
  ]
})
export class DeploymentModule { }
