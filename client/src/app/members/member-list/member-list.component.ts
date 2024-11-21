import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/params/userParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent {

  mParams = new UserParams();
  members: Member[]=[];
  pagination: Pagination | undefined;
  totalCount = 0;

  user?: User;
  userparams = new UserParams();
  
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}]

  constructor(private toastr: ToastrService, private accountService: AccountService, 
    private service: MemberService, private confirm: ConfirmService, private router: Router){}

  ngOnInit(): void {
    this,this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user!);
    
    this.loadMembers(false);
  }

  loadMembers(usecache: boolean=true) {
    this.service.setUserParams(this.userparams);
    
    this.service.getMembers(usecache).subscribe({
      next: (response) => {
        this.members = response.result;
        this.pagination = response.pagination;
        this.totalCount = response.count;
      }
    })
  }

  resetFilters() {
    this.service.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.userparams!.pageNumber != event.page) {
      console.log('new page:', event.page)
      this.userparams.pageNumber = event.page;
      this.loadMembers(true);
    }
  }

  editMember(evt: any) {
    //evt = member
    this.navigateByRoute(evt, '/members/edit', evt)
  }

 deleteMember(evt: any) {
  var id=evt;
  var confirmMsg = 'confirm delete this Member. WARNING: this cannot be undone';

  const observableInner = this.service.deleteMember(id);
  const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

  observableOuter.pipe(
      filter((confirmed) => confirmed),
      switchMap(() => {
        return observableInner
      })
  ).subscribe(response => {
    if(response) {
      this.toastr.success('Checklist deleted', 'deletion successful');
    } else {
      this.toastr.error('Error in deleting the Member', 'failed to delete')
    }
    
  });
  }

     
  navigateByRoute(id: string, routeString: string, object: Member) {
    let route =  routeString + '/' + id;

    this.router.navigate(
        [route], 
        { state: 
          { 
            user: this.user, 
            member: object,
            returnUrl: '/members' 
          } }
      );
  }
  
}
