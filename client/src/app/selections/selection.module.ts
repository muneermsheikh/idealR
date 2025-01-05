import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SelectionMenuComponent } from './selection-menu/selection-menu.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../_modules/shared.module';
import { SelectionRoutingModule } from './selection-routing.module';
import { EmploymentModalComponent } from './employment-modal/employment-modal.component';
import { SelectionLineComponent } from './selection-line/selection-line.component';
import { SelectionModalComponent } from './selection-modal/selection-modal.component';
import { SelectionPendingComponent } from './selection-pending/selection-pending.component';
import { SelectionPendingLineComponent } from './selection-pending-line/selection-pending-line.component';
import { SelectionsComponent } from './selections.component';


@NgModule({
  declarations: [
    EmploymentModalComponent,
    SelectionLineComponent,
    SelectionMenuComponent,
    SelectionModalComponent,
    SelectionPendingComponent,
    SelectionPendingLineComponent,
    SelectionsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    SelectionRoutingModule
  ]
})
export class SelectionModule { }
