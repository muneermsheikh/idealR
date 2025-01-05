import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SelectionMenuComponent } from './selection-menu/selection-menu.component';
import { SelectionPendingComponent } from './selection-pending/selection-pending.component';
import { RouterModule } from '@angular/router';
import { SelectionsPendingPagedResolver } from '../_resolvers/admin/selectionsPendingPagedResolver';
import { RejectionStatusResolver } from '../_resolvers/admin/rejectionStatusResolver';
import { SelectionsComponent } from './selections.component';
import { SelectionsPagedResolver } from '../_resolvers/admin/selectionsPagedResolver';

const routes = [
  {path: '', component: SelectionMenuComponent},
  
  {path: 'pendingSelections', component: SelectionPendingComponent,
    resolve: {
      selPending: SelectionsPendingPagedResolver,
      rejReasons: RejectionStatusResolver
    }
  },
  
  {path: 'selections', component: SelectionsComponent,
    resolve: {
      selections: SelectionsPagedResolver
    }
  },
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
export class SelectionRoutingModule { }
