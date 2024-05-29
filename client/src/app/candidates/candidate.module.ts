import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CandidateItemComponent } from './candidate-item/candidate-item.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../_modules/shared.module';
import { CandidateRoutingModule } from './candidate-routing.module';
import { CandidateHistoryComponent } from './candidate-history/candidate-history.component';
import { CandidateMenuComponent } from './candidate-menu/candidate-menu.component';
import { CandidateListComponent } from './candidate-list/candidate-list.component';


@NgModule({
  declarations: [
    CandidateItemComponent,
    CandidateHistoryComponent,
    CandidateMenuComponent,
    CandidateListComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,
    CandidateRoutingModule
  ]
})
export class CandidateModule { }
