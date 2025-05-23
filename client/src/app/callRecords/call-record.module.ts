import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CallRecordsItemComponent } from './call-records-item/call-records-item.component';
import { CallRecordsEditModalComponent } from './call-records-edit-modal/call-records-edit-modal.component';
import { CallRecordsListComponent } from './call-records-list/call-records-list.component';
import { SharedModule } from '../_modules/shared.module';
import { CallRecordRoutingModule } from './call-record-routing.module';
import { CallRecordMenuComponent } from './call-record-menu/call-record-menu.component';
import { MatIconModule } from '@angular/material/icon';


@NgModule({
  declarations: [
    CallRecordsItemComponent,
    CallRecordsEditModalComponent,
    CallRecordsListComponent,
    CallRecordMenuComponent,
   
  ],
  imports: [
    CommonModule,
    SharedModule,
    CallRecordRoutingModule,
    MatIconModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class CallRecordModule { }

/*export function contactResults: any[] = [{status: "Not Reachable"}, {status: "Wrong Number"}, 
  {status: "Declined-Family Issues"}, {status: "Declined-Low Remuneration"}, 
  {status: "Declined-Other Reasons"}, {status: "Will Call later"}, 
  {status: "Not interested for overseas"}, {status: "Declined-Service Charges"}
]
  */