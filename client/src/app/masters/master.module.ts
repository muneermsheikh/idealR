import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeesComponent } from './employees/employees.component';
import { EmployeeLineComponent } from './employee-line/employee-line.component';
import { SharedModule } from '../_modules/shared.module';
import { MasterRoutingModule } from './master-routing.module';
import { CategoriesComponent } from './categories/categories.component';
import { QualificationsComponent } from './qualifications/qualifications.component';
import { IndustriesComponent } from '../Administration/industries/industries.component';
import { EmpEditComponent } from './emp-edit/emp-edit.component';
import { EmpAttachmentComponent } from './emp-attachment/emp-attachment.component';


@NgModule({
  declarations: [
    EmployeesComponent,
    EmployeeLineComponent,
    CategoriesComponent,
    QualificationsComponent,
    IndustriesComponent,
    EmpEditComponent,
    EmpAttachmentComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    MasterRoutingModule
  ]
})
export class MasterModule { }
