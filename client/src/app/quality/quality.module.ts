import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HrObjComponent } from './hr-obj/hr-obj.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../_modules/shared.module';
import { MedObjectivesComponent } from './med-objectives/med-objectives.component';
import { IndexComponent } from './index/index.component';
import { QualityRoutingModule } from './quality-routing.module';



@NgModule({
  declarations: [
    HrObjComponent,
    MedObjectivesComponent,
    IndexComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    QualityRoutingModule
  ]
})
export class QualityModule { }
