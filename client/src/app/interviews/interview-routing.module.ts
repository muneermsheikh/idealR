import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InterviewListComponent } from './interview-list/interview-list.component';
import { InterviewEditComponent } from './interview-edit/interview-edit.component';
import { RouterModule } from '@angular/router';
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



