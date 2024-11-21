import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MedObjectivesComponent } from './med-objectives/med-objectives.component';
import { HrObjComponent } from './hr-obj/hr-obj.component';
import { RouterModule } from '@angular/router';
import { IndexComponent } from './index/index.component';


const routes = [
  {path: '', component: IndexComponent},
  
  {path: 'medobjective', component: MedObjectivesComponent },
  {path: 'hrobjective/:subroute', component: HrObjComponent}
]

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
  
})
export class QualityRoutingModule { }
