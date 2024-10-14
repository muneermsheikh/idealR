import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MemberListComponent } from './member-list/member-list.component';
import { MemberEditComponent } from './member-edit/member-edit.component';
import { memberDetailedResolver } from '../_resolvers/member-detailed.resolver';
import { RouterModule } from '@angular/router';

const routes = [
  {path: '', component: MemberListComponent},
  
  {path: 'edit/:username', component: MemberEditComponent,
    resolve: {
      member: memberDetailedResolver
    }
  }
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
export class MemberRoutingModule { }
