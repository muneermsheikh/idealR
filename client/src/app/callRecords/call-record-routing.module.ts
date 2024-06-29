import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CallRecordsListComponent } from './call-records-list/call-records-list.component';
import { CallRecordMenuComponent } from './call-record-menu/call-record-menu.component';
import { RouterModule } from '@angular/router';

const routes = [
  {path: '', component: CallRecordMenuComponent},

  {path: 'callRecordList', component: CallRecordsListComponent},

  {path: 'callRecordListLoggedinUsername', component: CallRecordsListComponent},
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
export class CallRecordRoutingModule { }
