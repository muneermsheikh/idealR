import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CandidateListComponent } from './candidate-list/candidate-list.component';
import { CandidateEditComponent } from './candidate-edit/candidate-edit.component';
import { CandidateItemComponent } from './candidate-item/candidate-item.component';
import { ChecklistModalComponent } from './checklist-modal/checklist-modal.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../_modules/shared.module';
import { CandidateRoutingModule } from './candidate-routing.module';
import { CandidateHistoryComponent } from './candidate-history/candidate-history.component';
import { CandidateMenuComponent } from './candidate-menu/candidate-menu.component';


@NgModule({
  declarations: [
    CandidateListComponent,
    CandidateEditComponent,
    CandidateItemComponent,
    ChecklistModalComponent,
    CandidateHistoryComponent,
    CandidateMenuComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,
    CandidateRoutingModule
  ]
})
export class CandidateModule { }
