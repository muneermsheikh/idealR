import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InterviewListComponent } from './interview-list/interview-list.component';
import { InterviewEditComponent } from './interview-edit/interview-edit.component';
import { RouterModule } from '@angular/router';
import { IntervwResolver } from '../_resolvers/hr/intervwResolver';
import { EditScheduleComponent } from './edit-schedule/edit-schedule.component';
import { InterviewAttendanceComponent } from './interview-attendance/interview-attendance.component';
import { InterviewsBriefPagedResolver } from '../_resolvers/hr/interviewsBriefPagedResolver';


const routes = [
  {path: '', component: InterviewListComponent,
    resolve: {
      interviews: InterviewsBriefPagedResolver
    }
  },
  
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

  {path: 'attendance/:orderid',  component: InterviewAttendanceComponent}
 
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



