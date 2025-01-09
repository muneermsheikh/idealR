import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MessageComponent } from './message/messages.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../_modules/shared.module';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { MessageRoutingModule } from './message-routing.module';



@NgModule({
  declarations: [
    MessageComponent,
    //MessageMenuComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    AngularEditorModule,
    MessageRoutingModule
  ]
})
export class MessageModule { }
