import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/parameters/userParams';
import { User } from 'src/app/_models/user';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members$: Observable<Member[]> | undefined;
  userParams: UserParams|undefined;
  user?: User;
  pagination: Pagination | undefined;
  genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }]

  constructor(private memberService: MemberService){
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void {
    this.members$ = this.memberService.getMembers();
  }

}
