import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Member } from './_models/member';
import { MemberService } from './_services/member.service';
import { DeployService } from './_services/deploy.service';
import { INextDepDataDto } from './_dtos/process/nextDepDataDto';
import { IDepItemToAddDto } from './_dtos/process/depItemToAddDto';
import { SuggestDeploymentModalComponent } from './deployments/suggest-deployment-modal/suggest-deployment-modal.component';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AboutComponent } from './about/about.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
  , encapsulation: ViewEncapsulation.None
})

export class AppComponent implements OnInit {
  title = 'client';
  model: any = {};
  user?:User;
  userIsAdmin=false;

  bsModalRef: BsModalRef | undefined;

  candidatePPNoForNextProcess: string = '';
  candidateNextProcess: INextDepDataDto | undefined;
  
  constructor(public accountService: AccountService, private toastr: ToastrService, private bsModalService: BsModalService,
    private router: Router, private depService: DeployService, private memberService: MemberService){}

  ngOnInit(): void {
    this.setCurrentUser();
  }

 
  setCurrentUser() {
    const userstring = localStorage.getItem('user');
    if (!userstring) return;
    this.user = JSON.parse(userstring);
    this.accountService.setCurrentUser(this.user!);
    this.userIsAdmin = this.user?.roles?.includes('Admin')!;
  }

  
  login(): void {

    var currentDate = new Date();
        var expiryDate = new Date('2024-04-27');
    
        console.log('currentDate', currentDate, 'expiryDate', expiryDate, currentDate > expiryDate);
    
        if(currentDate > expiryDate) {
          this.toastr.warning('Your trial period expired on 27-April-2025.  Please escalate the issue with your Vendor for resolution', 
              'Trial Period Expired', {closeButton: true, timeOut:15000});
          localStorage.setItem('user', JSON.stringify(null));
          this.router.navigateByUrl('/activation-error');
          return;
        }
        
      this.accountService.login(this.model).subscribe({
      next: (response: any) => {
        //this.router.navigateByUrl('/members');
        this.toastr.success('logged in successfully');
        this.model = {};
      }
    })

  }

  
  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  exportExcelProspectives() {
    this.router.navigateByUrl('/administration/excelConversion')
  }

  exportNaukriProspectives() {
    this.router.navigateByUrl('/administration/excelConversionOfNaukri')
  }

  exportExcelCustomers() {

  }

  exportExcelEmployees() {

  }

  editLoggedinMember() {
     var username = this.user?.userName;
     if(username===null || username === undefined) return;
    
     var member = this.memberService.getMember(username).subscribe({
        next: (response: Member) => {
          this.navigateByRoute(username!, '/members/edit', response);
        }
     })
  }

 
  navigateByRoute(id: string, routeString: string, object: Member) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            member: object,
            returnUrl: '/' 
          } }
      );
  }

  nextProcess() {
    
        const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
            passportNo: this.candidatePPNoForNextProcess
          }
        }
        this.bsModalRef = this.bsModalService.show(SuggestDeploymentModalComponent, config);
        this.bsModalRef.content.emittedDep.subscribe((response: boolean) => {
          if(response) {
            this.toastr.success('new deployment process added', 'Success')
          } else {
            this.toastr.warning('Failed to insert the deployment process', 'Failure')
          }
        })
  }

  displayAbout() {
    this.displayAboutModal(this.user!);
    
  }
  
  displayAboutModal(user: User) {
 
      const config = {
        class: 'modal-dialog-centered',
        initialState: {
          copyright: 'Ideal Solutions (' + new Date().getUTCFullYear(),
          email: 'idealsoln55@gmail.com',
          licensedTo: user.employer
        }
      }

      this.bsModalRef = this.bsModalService.show(AboutComponent, config);

      this.bsModalRef.onHide?.subscribe({
        next: () => {
         this.bsModalRef?.hide()
        }
      })      
  
    }

  showCustomAssessmentQuestions() {
    this.toastr.info('Custom Assessment Questions are avaiable in Category Listing.  Press the Assessment Question button to display assessment questions for that category', 
        'Open Categories', {extendedTimeOut: 0, closeButton: true })
  }

}
