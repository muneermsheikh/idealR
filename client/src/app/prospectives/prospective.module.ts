import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../_modules/shared.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MenuComponent } from './menu/menu.component';
import { ProspectiveLineComponent } from './prospective-line/prospective-line.component';
import { ProspectiveListComponent } from './prospective-list/prospective-list.component';
import { ProspectiveRoutingModule } from './prospective-routing.module';
import { NgxPrintModule } from 'ngx-print';


@NgModule({
  declarations: [
    MenuComponent,
    ProspectiveLineComponent,
    ProspectiveListComponent,
  
  ],

  imports: [
    CommonModule,
    SharedModule,
    MatButtonModule, 
    NgxPrintModule,
    MatIconModule,
    ProspectiveRoutingModule
  ]
})
export class ProspectiveModule { }
