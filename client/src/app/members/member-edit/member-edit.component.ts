import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  member: Member | undefined;

  user?: User;
  returnUrl = "/";

  bsValueDate = new Date();

  editForm?: NgForm;

  knownAs=''; city=''; country=''; phoneNumber=''; email='';gender='';

  constructor(private activatedRoute: ActivatedRoute, private toastr: ToastrService, 
    private router: Router, private service: MemberService){
    let nav: Navigation|null = this.router.getCurrentNavigation() ;

      if (nav?.extras && nav.extras.state) {
          if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

          if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
          if( nav.extras.state['member']) this.member = nav.extras.state['member'] as Member;
      }
  }


  ngOnInit(): void {
    
    /*this.activatedRoute.data.subscribe(data => {
      this.member = data['member']
    })*/
  }
  
  UpdateMember() {
    console.log('member', this.member);
    this.service.updateMember(this.member!).subscribe({
      next: succeeded => {
        if(succeeded) {
          this.toastr.success('Order Updated', 'Success')
        } else {
          this.toastr.warning('Failed to update the order', 'Failure')
        }
      },
      error: err => {
        console.log('updateOrder error:', err);
        if(err.error.error) {
          this.toastr.error(err.error.error, 'Error encountered')
        } else {
          this.toastr.error(err, 'Error encountered')
        }
      }
    })
  }

  close() {
    this.router.navigateByUrl('/members')
  }

}
