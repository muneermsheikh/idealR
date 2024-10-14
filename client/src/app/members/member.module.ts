import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MemberListComponent } from './member-list/member-list.component';
import { MemberItemComponent } from './member-item/member-item.component';
import { SharedModule } from '../_modules/shared.module';
import { MemberEditComponent } from './member-edit/member-edit.component';
import { MemberRoutingModule } from './member-routing.module';



@NgModule({
  declarations: [
    MemberListComponent,
    MemberItemComponent,
    MemberEditComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    MemberRoutingModule
  ]
})
export class MemberModule { }
