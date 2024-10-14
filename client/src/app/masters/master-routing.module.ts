import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeesComponent } from './employees/employees.component';
import { EmployeeFromIdResolver } from '../_resolvers/employeeFromIdResolver';
import { MastersMenuComponent } from './masters-menu/masters-menu.component';
import { QualificationsComponent } from './qualifications/qualifications.component';
import { CategoriesComponent } from './categories/categories.component';
import { IndustriesComponent } from '../Administration/industries/industries.component';
import { RouterModule } from '@angular/router';
import { CategoryResolver } from '../_resolvers/admin/categoryResolver';
import { SkillDataResolver } from '../_resolvers/hr/skillDataResolver';
import { CustomerResolver } from '../_resolvers/admin/customerResolver';
import { EmpEditComponent } from './emp-edit/emp-edit.component';
import { CategoryListResolver } from '../_resolvers/admin/categoryListResolver';


const routes = [
  {path: '', component: MastersMenuComponent},
  

  {path: 'employees', component: EmployeesComponent },
 
  {path: 'testEmpEdit/:id', component: EmpEditComponent,
    resolve: {
      employee: EmployeeFromIdResolver,
      professions: CategoryListResolver,
      skillDatas: SkillDataResolver,
      //industries: IndustryListResolver
    }
  },
  
  {path: 'qualifications', component: QualificationsComponent },
  {path: 'categories', component: CategoriesComponent},
  {path: 'industries', component: IndustriesComponent},

 
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
export class MasterRoutingModule { }
