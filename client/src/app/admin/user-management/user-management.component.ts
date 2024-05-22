import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit{

  users: User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  clBtnName='';
  pagination: Pagination | undefined;
  
  availableRoles = [
    'Candidate', 'Employee', 'Client', 'Admin', 'HR Manager', 'HR Supervisor', 'HR Executive',
    'Asst HR Executive', 'Accounts Manager', 'Finance Manager', 'Cashier', 'Accountant',
    'Document Controller-Admin', 'Document Controller-Processing', 'Processing Manager',
    'AdminManager', 'Receptionist', 'Marketing Manager', 'Design Assessment Questions',
    'Register Selections and Rejections', 'Approve release of documents'
  ]

  selectedRoles: string[]=[];


  constructor(private adminService: AdminService, private modalService: BsModalService) {};

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    
    this.adminService.getUsersWithRolesPaginated(1,10).subscribe({
      next: response => {
        if(response.result && response.pagination) {
          this.users = response.result;
          this.pagination = response.pagination;
        }
      }
    })
  }

  

  openRolesModal(user: User) {

    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        username: user.userName,
        availableRoles: this.availableRoles,
        selectedRoles: [...user.roles]
      }
    }

    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        const selectedRoles = this.bsModalRef.content?.selectedRoles;
        if (!this.arrayEqual(selectedRoles, user.roles)) {
          this.adminService.updateUserRoles(user.userName, selectedRoles!).subscribe({
            next: () => console.log('succeeded updatng user roles')
          })
        }
      }
    })

  }

  private arrayEqual(arr1: any, arr2: any) {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort())
  }
}
