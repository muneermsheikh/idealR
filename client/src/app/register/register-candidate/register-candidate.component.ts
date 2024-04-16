import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-register-candidate',
  templateUrl: './register-candidate.component.html',
  styleUrls: ['./register-candidate.component.css']
})
export class RegisterCandidateComponent implements OnInit{

  //@Input() users: any;
  @Output() registerCanceled = new EventEmitter();
  
  model: any = {};


  constructor(private accountService: AccountService, private toastrService: ToastrService){}

  ngOnInit(): void {
    
  }

  register(){
    this.accountService.register(this.model).subscribe({
      next: () => {
           this.cancel();
      },
      error: error => {
        console.log('error from register-candidate.register: ', error.error);
        this.toastrService.error('register-candidate', error.error);
      }
    })
  }

  cancel() {
    this.registerCanceled.emit(false);
  }
}
