import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-liked-list',
  templateUrl: './member-liked-list.component.html',
  styleUrls: ['./member-liked-list.component.css']
})
export class MemberLikedListComponent implements OnInit{

  members: Member[] |undefined;
  predicate = 'liked';

  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination | undefined;
  user: User | undefined;

  constructor(private memberService: MemberService){}

  ngOnInit(): void {
    this.loadLikes();
  }
  
  loadLikes() {
    this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe({
      next: response => {
        this.members = response.result;
        this.pagination = response.pagination;
      }
    })
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }

}
