import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuComponent } from './menu/menu.component';
import { ProspectiveListComponent } from './prospective-list/prospective-list.component';
import { RouterModule } from '@angular/router';
import { ProspectivePagedResolver } from '../_resolvers/admin/prospectivePagedResolver';
import { ProspectiveListResolver } from '../_resolvers/admin/prospectiveListResolver';
import { ProspectiveHeaderResolver } from '../_resolvers/admin/prospectiveHeaderResolver';
import { AudioComponent } from './audio/audio.component';
import { AutodialComponent } from './autodial/autodial.component';
import { AudioMessagesPendingPagedResolver } from '../_resolvers/admin/AudioMessagesPendingPagedResolver';


//parent route: prospectives
const routes = [
  {path: '', component: MenuComponent},
  
  {path: 'prospective', component: ProspectiveListComponent,
    resolve: {
      prospectives: ProspectivePagedResolver,
      headers: ProspectiveHeaderResolver
    }
  },

  {path: 'prospectiveReport/:orderno', component: ProspectiveListComponent,
    resolve: {
      reports: ProspectiveListResolver
    }
  },
  
  {path: 'audiomessages', component: AudioComponent },

  {path: 'autodial', component: AutodialComponent,
    resolve: {audioMessages: AudioMessagesPendingPagedResolver}
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
export class ProspectiveRoutingModule { }
