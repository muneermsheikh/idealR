import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { LoDashExplicitNumberArrayWrapper } from 'lodash';
import { getDate } from 'ngx-bootstrap/chronos/utils/date-getters';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { CvsMatchingProfAvailableDto } from 'src/app/_dtos/hr/cvsMatchingProfAvailableDto';
import { IIntervwItemCandidate, IntervwItemCandidate } from 'src/app/_models/hr/intervwItemCandidate';
import { IIntervw } from 'src/app/_models/hr/intervw';
import { IIntervwItem } from 'src/app/_models/hr/intervwItem';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { InterviewService } from 'src/app/_services/hr/interview.service';
import { CandidatesAvailableModalComponent } from 'src/app/modals/candidates-available-modal/candidates-available-modal.component';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit{

  interview: IIntervw | undefined;
  //form: FormGroup = new FormGroup({});
  
  constructor(private activatedRoute: ActivatedRoute, 
      private toastr: ToastrService, private bsModalService: BsModalService, 
      private confirm: ConfirmService, private service: InterviewService) {}
  
      bsValueDate = new Date();
      bsModalRef: BsModalRef|undefined;

    ngOnInit(): void {
        this.activatedRoute.data.subscribe(data => {
          this.interview = data['interview'];
          //console.log('interview: ', this.interview);
          //this.Initialize(this.interview!);
          //if(this.interview) this.patchValue();
        })

    }

      updateReportedAt(candidate: IIntervwItemCandidate) {

        var dt = new Date().toLocaleDateString();
        console.log('dt', dt);
        candidate.reportedAt = new Date(dt);
      }

      updateInterviewedAt(candidate: IIntervwItemCandidate) {
        candidate.interviewedAt = new Date(getFormattedDate());
      }

      getMatchingCVs(item:IIntervwItem) {

        var profid=item.professionId;

        if(profid === 0) {
          this.toastr.warning('No Task object returned from Task line');
          return;
        }  
      
        const config = {
            class: 'modal-dialog-centered modal-lg',
            initialState: {
              professionid: profid
            }
          }
      
          this.bsModalRef = this.bsModalService.show(CandidatesAvailableModalComponent, config);
      
          this.bsModalRef.content.emittedEvent.subscribe({
            next: (response: CvsMatchingProfAvailableDto[]) => {
              if(response !== null) {
                  response.forEach(x => {
                    
                    let cv = new IntervwItemCandidate();

                    cv.candidateId=x.candidateId;
                    cv.intervwItemId=item.id; cv.applicationNo=x.applicationNo;
                    cv.candidateName=x.fullName; cv.scheduledFrom=new Date();
                    
                    item.interviewItemCandidates.push(cv);
                  })
                  this.toastr.success(response.length + ' candidates assigned.  Pl note the form needs to be saved for changes to take effect', 'Success');
              } else {
                this.toastr.warning('No candidates found - either registered or in prospective list - matching the profession ' + item.professionName, 'Not Found')
              }
            },
            error: (err: any) => this.toastr.error(err.error.details, 'error encountered')
          
          })
        }
      
      deleteItemCandidate(orderitemId: number, candidateid: number) {
        var item = this.interview?.interviewItems.find(x => x.orderItemId===orderitemId);
        if(item !==undefined) {
          var index = item.interviewItemCandidates.findIndex(x => x.candidateId===candidateid);
          if(index !== -1) item.interviewItemCandidates.splice(index, 1);
        } 
      }
      

      addItemCandidate(itemId: number) 
      {

      }

      update() {
        var clone = this.interview;
        console.log('cloned:', clone);
        var ct=0;
        var disallowed: IIntervwItemCandidate[]=[];
        var strErrs='';
        clone?.interviewItems.forEach(x => {
          x.interviewItemCandidates.forEach(y => {
            if(new Date(y.scheduledFrom).getHours() === 0) {
              disallowed.push(y);
               strErrs +=++ct + '. (' + y.id + ')' + x.professionName + ", candidate:" + y.candidateName;
            }
          })
        });

        if(disallowed.length) {
          this.confirm.confirm('confirm exclude non-compliant candidate assignments', 
              'Following candidates have no time slots assigned.  These will be excluded from the updates.  ' + 
              strErrs + '. Press OK to proceed, NO to abort and correct the data')
              .subscribe(response => {
                if(!response) {
                    this.toastr.warning('aborted by user', 'Abort');
                    return;
                } else {
                  disallowed.forEach(d => {
                    clone?.interviewItems.forEach(x => {
                      var index = x.interviewItemCandidates.findIndex(x => x.id === d.id);
                      if(index !== -1) {
                        x.interviewItemCandidates.splice(index,1);
                        //return true;
                      }
                    }
                  )
                  });
                }
              })
        }

        if(clone?.id ===0) {
          this.service.saveNewInterview(clone).subscribe({
            next: (response: IIntervw) => {
              if(response !== null) {
                this.interview = response;
                this.toastr.success('Interview updated', 'Success')
              } else {
                this.toastr.warning('Failed to update the interview', 'Failure')
              }
            },
            error: (err: any) => this.toastr.error(err.error.details, 'Error encountered in insert')
          })
        } else {
          this.service.updateInterview(clone!).subscribe({
            next: (response: IIntervw) => {
              if(response !== null) {
                this.interview = response;    //this will take  care of updating the DOM
              } else {
                this.toastr.warning('Failed to update the interview', 'Failure')
              }
            },
            error: (err: any) => this.toastr.error(err.error.details, 'Error encountered in update')
          })
        }
      }

      onSubmit(form: any) {
        var clone = form.value;
        console.log('cloned:', clone);
        var ct=0;
        var disallowed: IIntervwItemCandidate[]=[];
        var strErrs='';
        clone?.interviewItems.forEach((x: { interviewItemCandidates: any[]; professionName: string; }) => {
          x.interviewItemCandidates.forEach(y => {
            if(new Date(y.scheduledFrom).getHours() === 0) {
              disallowed.push(y);
               strErrs +=++ct + '. (' + y.id + ')' + x.professionName + ", candidate:" + y.candidateName;
            }
          })
        });

        if(disallowed.length) {
          this.confirm.confirm('confirm exclude non-compliant candidate assignments', 
              'Following candidates have no time slots assigned.  These will be excluded from the updates.  ' + 
              strErrs + '. Press OK to proceed, NO to abort and correct the data')
              .subscribe(response => {
                if(!response) {
                    this.toastr.warning('aborted by user', 'Abort');
                    return;
                } else {
                  disallowed.forEach(d => {
                    clone?.interviewItems.forEach((x:any) => {
                      var index = x.interviewItemCandidates.findIndex((x: { id: number; }) => x.id === d.id);
                      if(index !== -1) {
                        x.interviewItemCandidates.splice(index,1);
                        //return true;
                      }
                    }
                  )
                  });
                }
              })
        }

        if(clone?.id ===0) {
          this.service.saveNewInterview(clone).subscribe({
            next: (response: IIntervw) => {
              if(response !== null) {
                this.interview = response;
                this.toastr.success('Interview updated', 'Success')
              } else {
                this.toastr.warning('Failed to update the interview', 'Failure')
              }
            },
            error: (err: any) => this.toastr.error(err.error.details, 'Error encountered in insert')
          })
        } else {
          this.service.updateInterview(clone!).subscribe({
            next: (response: IIntervw) => {
              if(response !== null) {
                this.interview = response;    //this will take  care of updating the DOM
              } else {
                this.toastr.warning('Failed to update the interview', 'Failure')
              }
            },
            error: (err: any) => this.toastr.error(err.error.details, 'Error encountered in update')
          })
        }
      }
     
  } 

    function padTo2Digits(num: number): string {
      return num.toString().padStart(2, '0');
    }

    function getFormattedDate(): string {
      const date = new Date();
      const year = date.getFullYear();
      const month = padTo2Digits(date.getMonth() + 1); // Months are zero-based
      const day = padTo2Digits(date.getDate());
      const hours = padTo2Digits(date.getHours());
      const minutes = padTo2Digits(date.getMinutes());

      var dt = `${year}-${month}-${day}T${hours}:${minutes}`;
      console.log('getfofrmattedate:', dt);
      return dt;
    }
  
