import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CallRecordsItemComponent } from './call-records-item/call-records-item.component';
import { CallRecordsEditModalComponent } from './call-records-edit-modal/call-records-edit-modal.component';
import { CallRecordsListComponent } from './call-records-list/call-records-list.component';
import { SharedModule } from '../_modules/shared.module';
import { CallRecordRoutingModule } from './call-record-routing.module';
import { CallRecordMenuComponent } from './call-record-menu/call-record-menu.component';
import { CallRecordAddModalComponent } from './call-record-add-modal/call-record-add-modal.component';


@NgModule({
  declarations: [
    CallRecordsItemComponent,
    CallRecordsEditModalComponent,
    CallRecordsListComponent,
    CallRecordMenuComponent,
    CallRecordAddModalComponent,
    
  ],
  imports: [
    CommonModule,
    SharedModule,
    CallRecordRoutingModule
  ]
})
export class CallRecordModule { }

/*export function contactResults: any[] = [{status: "Not Reachable"}, {status: "Wrong Number"}, 
  {status: "Declined-Family Issues"}, {status: "Declined-Low Remuneration"}, 
  {status: "Declined-Other Reasons"}, {status: "Will Call later"}, 
  {status: "Not interested for overseas"}, {status: "Declined-Service Charges"}
]
  */