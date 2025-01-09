import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MessageComponent } from './message/messages.component';
import { RouterModule } from '@angular/router';


const routes = [
  /* {path: '', component: MessageMenuComponent}, */
  
  {path: 'messages', component: MessageComponent}
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
export class MessageRoutingModule { }
