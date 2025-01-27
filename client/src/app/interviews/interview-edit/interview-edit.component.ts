import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { IIntervw } from 'src/app/_models/hr/intervw';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { InterviewService } from 'src/app/_services/hr/interview.service';

@Component({
  selector: 'app-interview-edit',
  templateUrl: './interview-edit.component.html',
  styleUrls: ['./interview-edit.component.css']
})
export class InterviewEditComponent implements OnInit{

  interview: IIntervw | undefined;

  constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute, private router: Router,
    private service: InterviewService, private toastr: ToastrService, private confirm: ConfirmService) {}
  
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

  deleteInterview() {

      var confirmMsg = 'confirm delete this Interview?. WARNING: this cannot be undone';

      const observableInner = this.service.deleteInterview(this.interview!.id);
      const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

      observableOuter.pipe(
          filter((confirmed) => confirmed),
          switchMap(() => {
            return observableInner
          })
      ).subscribe(response => {
        if(response) {
          this.toastr.success('Interview deleted', 'deletion successful');
          this.router.navigateByUrl('/interviews');
        } else {
          this.toastr.error('Error in deleting the interview', 'failed to delete')
        }
        
      });

  }
}