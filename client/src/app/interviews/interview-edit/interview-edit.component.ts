import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrComponentlessModule, ToastrService } from 'ngx-toastr';
import { IIntervw } from 'src/app/_models/hr/intervw';
import { InterviewService } from 'src/app/_services/hr/interview.service';

@Component({
  selector: 'app-interview-edit',
  templateUrl: './interview-edit.component.html',
  styleUrls: ['./interview-edit.component.css']
})
export class InterviewEditComponent implements OnInit{

  interview: IIntervw | undefined;

  constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute, 
    private service: InterviewService, private toastr: ToastrService) {}
  
  bsValueDate = new Date();

  ngOnInit(): void {
      this.activatedRoute.data.subscribe(data => {
        this.interview = data['interview'];
      })

  }

  updateInterview() {
    this.service.updateInterviewHeader(this.interview!).subscribe({
      next: (response: IIntervw) => {
        if(response === null) {
          this.toastr.warning('Failed to update the interview', 'failure')
        } else {
          this.toastr.success('Updated the interview', 'Success')
        }
      }, 
      error: (err: any) => this.toastr.error(err.error.details, 'Error encountered')
    })
  }

}