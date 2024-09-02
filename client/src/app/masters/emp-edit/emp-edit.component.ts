import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IEmployee } from 'src/app/_models/admin/employee';
import { IIndustryType } from 'src/app/_models/admin/industryType';
import { ISkillData } from 'src/app/_models/hr/skillData';
import { IProfession } from 'src/app/_models/masters/profession';
import { User } from 'src/app/_models/user';
import { EmployeeService } from 'src/app/_services/admin/employee.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { FileService } from 'src/app/_services/file.service';

@Component({
  selector: 'app-emp-edit',
  templateUrl: './emp-edit.component.html',
  styleUrls: ['./emp-edit.component.css']
})
export class EmpEditComponent {

  employee: IEmployee | undefined;
  professions: IProfession[]=[];
  industries: IIndustryType[]=[];
  skillDatas: ISkillData[]=[];
  
  bsValueDate = new Date();
  bsValue = new Date();
  user?: User;
  returnUrl = '';
  lastTimeCalled = 0;
  
  form: FormGroup = new FormGroup({});

  skillLevels = [
    {"skillLevel": "Proficient"}, {"skillLevel": "Good"}, {"skillLevel": "Poor"}, {"skillLevel": "Unskilled"}
  ]


  
  constructor(
      private toastr: ToastrService, 
      private service: EmployeeService, 
      private activatedRoute: ActivatedRoute,
      private fb: FormBuilder, 
      private confirm: ConfirmService, 
      private router: Router,
      private downloadService: FileService){
        let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
        }
    }

  ngOnInit(): void {

    this.activatedRoute.data.subscribe(data => {
      this.employee = data['employee'],
      this.skillDatas = data['skillDatas'],
      this.professions = data['professions'];
      //this.industries = data['industries'];
      
      if(this.employee !== undefined) this.InitializeForm(this.employee);
      }
    )
   
    
  }

  InitializeForm(emp: IEmployee) {
    console.log('employee:', emp);

    this.form = this.fb.group({
        id: [emp.id], gender: [emp.gender, Validators.required], firstName: [emp.firstName, Validators.required], 
        secondName: [emp.secondName, Validators.required], familyName: [emp.familyName, Validators.required], 
        knownAs: [emp.knownAs, Validators.required], displayName: [emp.displayName, Validators.required],
        address: [emp.address], address2: [emp.address2], city: [emp.city, Validators.required], 
        email: [emp.email, Validators.required], phoneNo: [emp.phoneNo, [Validators.required, Validators.minLength(10)]], 
        phone2: [emp.phone2], dateOfBirth: [emp.dateOfBirth], dateOfJoining: [emp.dateOfJoining, Validators.required],
        placeOfBirth: [emp.placeOfBirth], department: [emp.department, Validators.required], 
        aadharNo: [emp.aadharNo, [Validators.required, Validators.minLength(12), Validators.maxLength(12)]], 
        introduction: [emp.introduction], position: [emp.position, Validators.required], appUserId: [emp.appUserId], 
        qualification: [emp.qualification], userName: [emp.userName], 
        status: [emp.status],
        
        hrSkills: this.fb.array(
          emp.hrSkills.map(x =>  (
            this.fb.group({
              id: [x.id], employeeId: [x.employeeId, Validators.required], professionId: [x.professionId, Validators.required],
              professionName: [x.professionName, Validators.required], industryId: [x.industryId],
              skillLevelName: [x.skillLevelName, Validators.required]
            })
          ))
        ),

        employeeOtherSkills: this.fb.array(
            emp.employeeOtherSkills.map(m => (
              this.fb.group({
                id: [m.id], employeeId: [m.employeeId, Validators.required], 
                skillDataId: [m.skillDataId, Validators.required],
                skillLevel: [m.skillLevel, Validators.required], isMain: [m.isMain]
              })
            ))
        ),

      
    })
  }

  //HRSkills
    get hrSkills(): FormArray {
      return this.form.get("hrSkills") as FormArray
    }

    newHRSkill(): FormGroup {
      return this.fb.group({
        id: 0, employeeId: [this.employee?.id, Validators.required], 
        professionId: [0, Validators.required],
        professionName: ['', Validators.required], 
        industryId: [0],
        skillLevelName: ['', Validators.required]
      })
    }

    addHRSkill() {
        this.hrSkills.push(this.newHRSkill());
      }

    removeHRSkill(index: number) {
        this.confirm.confirm("Confirm Delete", "This will delete customer official.  To delete from the database as well, remember to SAVE this form before you close it")
          .subscribe({next: confirmed => {
            if(confirmed) {
              this.hrSkills.removeAt(index);
              this.hrSkills.markAsDirty();
            }
        }})
    }

//agency specialties

    get employeeOtherSkills(): FormArray {
      return this.form.get("employeeOtherSkills") as FormArray
    }

    newEmployeeOtherSkill(): FormGroup {
      return this.fb.group({
        id: 0, employeeId: [this.employee?.id, Validators.required], 
        skillDataId: [0, Validators.required],
        skillLevel: [0, Validators.required], isMain: [false]
      })
    }

    addEmployeeOtherSkill() {
      this.employeeOtherSkills.push(this.newEmployeeOtherSkill())
    }

    removeEmployeeOtherSkill(index: number) {
      this.employeeOtherSkills.removeAt(index);
      this.employeeOtherSkills.markAsDirty();
    }

    //attachments
   
    professionChanged(prof: any, index: number) {
      //var profn = this.hrSkills.at(index).value;
      this.hrSkills.at(index).get('professionName')?.setValue(prof.professionName);
      //profn.get('professionName').set(prof.professionName);
    }

  
   /* validateEmailNotTaken(): AsyncValidatorFn {x
      return control => {
        //return timer(500).pipe(
        return timer(10).pipe(
          switchMap(() => {
            if (!control.value) {
              return of(null);
            }
            return this.service.checkEmployeeEmailExists(control.value).pipe(
              map((res: string) => {
                return res==='' ? {emailExists: true} : null;
              })
            );
          })
        )
      }
    }*/

   
  
  update = () => {
       
    this.service.updateEmployee(this.form.value).subscribe({
        next: (response: boolean) => {
          if(!response) {
            this.toastr.error('failed to save the Employee data', 'Failed');
          } else {
            this.toastr.success('Employee created/updated', 'Employee successfully created');
            //this.registerForm.setValue({'applicationNo': response.returnInt});
          }},
        error: (err: any) => {
          this.toastr.error(err.error.details, 'failed to insert the employee');

        }
    })
  }


}
