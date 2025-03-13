import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SelectAssociatesModalComponent } from '../select-associates-modal/select-associates-modal.component';
import { RolesModalComponent } from './roles-modal/roles-modal.component';
import { InputModalComponent } from './input-modal/input-modal.component';
import { DisplayTextModalComponent } from './display-text-modal/display-text-modal.component';
import { DateInputRangeModalComponent } from './date-input-range-modal/date-input-range-modal.component';
import { CandidatesAvailableModalComponent } from './candidates-available-modal/candidates-available-modal.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NavComponent } from './nav/nav.component';
import { SharedModule } from '../_modules/shared.module';
import { SuggestDeploymentModalComponent } from '../deployments/suggest-deployment-modal/suggest-deployment-modal.component';



@NgModule({
  declarations: [
    NavComponent
  ],
  
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ]
})
export class ModalModule { }
