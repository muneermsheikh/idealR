import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeesComponent } from './employees/employees.component';
import { EmployeeLineComponent } from './employee-line/employee-line.component';
import { SharedModule } from '../_modules/shared.module';
import { MasterRoutingModule } from './master-routing.module';
import { CategoriesComponent } from './categories/categories.component';
import { QualificationsComponent } from './qualifications/qualifications.component';
import { IndustriesComponent } from './industries/industries.component';
import { EmpEditComponent } from './emp-edit/emp-edit.component';
import { EmpAttachmentComponent } from './emp-attachment/emp-attachment.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CategoryEditModalComponent } from './category-edit-modal/category-edit-modal.component';
import { CategoryQBankModalComponent } from './category-qbank-modal/category-qbank-modal.component';
import { CategoryQBankQsComponent } from './category-qbank-qs/category-qbank-qs.component';
import { IndustryEditModalComponent } from './industry-edit-modal/industry-edit-modal.component';


@NgModule({
  declarations: [
    EmployeesComponent,
    EmployeeLineComponent,
    CategoriesComponent,
    QualificationsComponent,
    IndustriesComponent,
    EmpEditComponent,
    EmpAttachmentComponent,
    CategoryEditModalComponent,
    CategoryQBankModalComponent,
    CategoryQBankQsComponent,
    IndustryEditModalComponent
  ],

  imports: [
    CommonModule,
    SharedModule,
    MasterRoutingModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class MasterModule { }
