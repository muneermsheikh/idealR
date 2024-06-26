import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/params/userParams';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

 // members$: Observable<Member[]> | undefined;
  members: Member[] | undefined;

  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  

  genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }]

  constructor(private memberService: MemberService, private toastr: ToastrService){
   
    this.userParams = memberService.getUserParams();
  }

  ngOnInit(): void {
    this.toastr.info('entered member list ngONInit');
    //this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }

  loadMembers() {
    if(this.userParams) {

      this.memberService.setUserParams(this.userParams);

      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          if(response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }

  }

  resetFilters() {
  
      this.userParams = this.memberService.resetUserParams();
      this.loadMembers();
    
  }

  pageChanged(event: any)
  {
    if(!this.userParams) return;

    if(this.userParams.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
