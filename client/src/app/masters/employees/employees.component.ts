import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap } from 'rxjs';
import { IEmployeeBriefDto } from 'src/app/_dtos/admin/employeeBriefDto';
import { Pagination } from 'src/app/_models/pagination';
import { employeeParams } from 'src/app/_models/params/Admin/employeeParams';
import { User } from 'src/app/_models/user';
import { EmployeeService } from 'src/app/_services/admin/employee.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { EmpAttachmentComponent } from '../emp-attachment/emp-attachment.component';
import { IEmployeeAttachment } from 'src/app/_models/admin/employeeAttachment';

@Component({
  selector: 'app-employees',
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.css']
})
export class EmployeesComponent implements OnInit{

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  employees: IEmployeeBriefDto[]=[];
  pagination: Pagination | undefined;
  user?: User;

  iParams = new employeeParams();
  orderNoToGet = 0;

  totalCount: number=0;

  bsModalRef: BsModalRef | undefined;
  
  constructor(private service: EmployeeService, 
    private toastr: ToastrService, 
    private modalService: BsModalService, private router: Router,
    private confirm: ConfirmService) {}

  ngOnInit(): void {
    this.getEmployeesPaged();
  }

  getEmployeesPaged() {

    this.service.setParams(this.iParams);
    this.service.getEmployeesPaged().subscribe({
      next: response => {
        this.employees = response.result;
        this.pagination = response.pagination;
        this.totalCount = response.totalCount;
      }
    })
  }


  employeeEdit(employeeId: any) {    

    //this.router.navigateByUrl('/masters/editemployee/' + employeeId);
    this.router.navigateByUrl('/masters/testEmpEdit/' + employeeId);
  }

  employeeDelete(event: any) {
    var id=event!.id;
    var confirmMsg = 'confirm delete this Employee. ' +
      'WARNING: If there are more than one venues for this employee,  This cannot be undone';

    const observableInner = this.service.deleteEmployee(id);
    const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => {
          return observableInner
        })
    ).subscribe(response => {
      if(response) {
        this.toastr.success('Employee deleted', 'Success');
        this.bsModalRef!.hide();
      } else {
        this.toastr.error('Error in deleting the checklist', 'failed to delete')
      }
    });

  }

  
  onPageChanged(event: any){
    const params = this.service.getParams() ?? new employeeParams();
    
    if (params.pageNumber !== event.page) {
      params.pageNumber = event.page;
      this.service.setParams(params);
      this.getEmployeesPaged();
    }
  }

  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.getEmployeesPaged();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    const params = this.service.getParams();
    this.service.setParams(params);
    this.getEmployeesPaged();
  }

  attachments(empbrief: IEmployeeBriefDto) {
    
    const config = {
      class: 'modal-dialog-centered modal-lg',
      initialState: {
        empId: empbrief.id,
        empName: empbrief.firstName,
        position: empbrief.position
      }
    }

    this.bsModalRef = this.modalService.show(EmpAttachmentComponent, config);

    const observableOuter =  this.bsModalRef.content.updateEvent;
    
    observableOuter.pipe(
      filter((response: IEmployeeAttachment[]) => response.length !==0),
      switchMap((response: IEmployeeAttachment[]) =>  {
        return this.service.updateOrAddEmployeeAttachments(response)
      }),
      catchError((err: any) => {
        this.toastr.error(err.error.details, 'Error encountered in update service');
        return of(null);
      })
    ).subscribe((response: string) => {
      if(response !== '') {
        this.toastr.success('User Attachments updated', 'Success');
        //**TODO- Update DOM with new values */
      } else {
        this.toastr.warning(response, 'Failure');
      }
      
    }
  
  );

  }

}
