import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InterviewListComponent } from './interview-list/interview-list.component';
import { InterviewEditComponent } from './interview-edit/interview-edit.component';
import { InterviewResolver } from '../_resolvers/interviewResolver';
import { RouterModule } from '@angular/router';
import { EditComponent } from './edit/edit.component';
import { IntervwResolver } from '../_resolvers/hr/intervwResolver';
import { EditScheduleComponent } from './edit-schedule/edit-schedule.component';


const routes = [
  {path: '', component: InterviewListComponent},
  

  {path: 'editintervw/:orderno', component: InterviewEditComponent, // EditComponent,
    resolve: {
      interview: IntervwResolver
    }
  },
 
  {path: 'editschedule/:orderno', component: EditScheduleComponent,
    resolve: {
      interview: IntervwResolver
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
export class InterviewRoutingModule {


 }



