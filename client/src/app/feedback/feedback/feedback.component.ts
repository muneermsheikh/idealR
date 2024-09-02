import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Navigation, Router } from '@angular/router';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { IFeedbackHistoryDto } from 'src/app/_dtos/admin/feedbackAndHistoryDto';
import { IFeedback } from 'src/app/_models/hr/feedback';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { FeedbackService } from 'src/app/_services/feedback.service';

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.css']
})
export class FeedbackComponent implements OnInit {

  feedback: IFeedback | undefined;
  
  bsValueDate = new Date();
  bsValue = new Date();
  user?: User;
  feedbackHistory?: IFeedbackHistoryDto[]=[];
  feedbackIdSelected: number=-1;
  lastFeedbackIdSelected: number=-1;

  form: FormGroup = new FormGroup({});
  returnUrl='';

  constructor(private fb: FormBuilder, private confirm: ConfirmService, private activatedRoute: ActivatedRoute,
    private service: FeedbackService, private toastr: ToastrService, private router: Router){
      let nav: Navigation|null = this.router.getCurrentNavigation() ;

          if (nav?.extras && nav.extras.state) {
            if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

            if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
          }
    }

  ngOnInit(): void {
    
    this.activatedRoute.data.subscribe({
      next: data => {
        this.feedback = data['feedback'],
        this.feedbackHistory = data['history'];
  
        if(this.feedback) this.InitializeForm(this.feedback);
      }, 
      error: (err:any) => {
        console.log('error:', err);
        this.toastr.error(err.error.error.details, 'Error encountered')
      }
    })
    

  }

  InitializeForm(feedbk: IFeedback) {

    this.form = this.fb.group({
      id: feedbk.id,
      customerId: feedbk.customerId,
      customerName: feedbk.customerName,
      city: feedbk.city,
      officialName: feedbk.officialName,
      designation: feedbk.designation,
      email: feedbk.email,
      phoneNo: feedbk.phoneNo,
      dateIssued: feedbk.dateIssued,
      dateReceived: feedbk.dateReceived,
      gradeAssessedByClient: feedbk.gradeAssessedByClient,
      customerSuggestion: feedbk.customerSuggestion,

      feedbackInputItems: this.fb.array(
        feedbk.feedbackItems.map(x => (
          this.fb.group({
            id: x.id, customerFeedbackId: x.customerFeedbackId,
            feedbackGroup: x.feedbackGroup, 
            questionNo: x.questionNo,
            question: [x.question, Validators.required],
            response: [x.response, Validators.required],
            prompt1: [x.prompt1, Validators.required],
            prompt2: [x.prompt2, Validators.required],
            prompt3: [x.prompt3],
            prompt4: x.prompt4,
            remarks: x.remarks,
          })
        ))
      )

    })
  }

  get feedbackInputItems(): FormArray {
    return this.form.get("feedbackInputItems") as FormArray
  }

  newFeedbackItem(): FormGroup {

    var lastItem = this.feedbackInputItems.at(this.feedbackInputItems.length-1);

    return this.fb.group({
        id: 0, customerFeedbackId: this.feedback?.id,
        questionNo: lastItem.get("questionNo")?.value + 1,
        question: '', 
        isMandatory: false,
        prompt1: '', prompt2: '', prompt3: '', prompt4: '',
        remarks: '', response:''
      
    })
  }

  addNewFeedbackItem() {
    this.feedbackInputItems.push(this.newFeedbackItem)
  }

  removeFeedbackInput(index: number) {
      this.confirm.confirm("Are you sure you want to delete this record?", "confirm Delete")
        .subscribe({
          next: confirmed => {
            if(confirmed) {
              this.feedbackInputItems.removeAt(index);
            }
          }
        })
  }
  
  updateFeedback() {
      
      var formdata = this.form.value;
      console.log('formdata:', formdata);

      const observableInner = formdata.id > 0 
        ? this.service.updateFeedback(formdata) : this.service.saveNewFeedback(formdata);
      
      var confirmMsg = 'Do you want to update changes';

      const observableOuter = this.confirm.confirm('confirm Update', confirmMsg);

      observableOuter.pipe(
          filter((confirmed) => confirmed),
          switchMap(() => {
            return observableInner
          })
      ).subscribe(response => {
        if(response) {
          this.toastr.success('Feedback successfully updated', 'Insertion successful');
        } else {
          this.toastr.error('Error in updating the Feedback Input', 'failed to update')
        }
      });

  }

  generatenew() {
    if(this.feedback!.id !== 0) {
      var msg = "This will replace the current feedback data with a new blank Feedback Object.  do you want to continue?";
      this.confirm.confirm("Confirm replace current feedback", msg).subscribe({
        next: (cnfm: boolean) => {
          if(!cnfm) return;
        }
      })
    }
    
    this.service.generatenewfeedback(this.feedback!.customerId).subscribe({
      next: (response: IFeedback) => this.feedback = response
    })
  }
  
  generateLinkForAddresses() {

  }

  close() {
      this.router.navigateByUrl(this.returnUrl);
  }

  gradeChanged(index: number) {
    var points =  this.feedbackInputItems.value.map((x:any) => +x.response).reduce((a:number, b: number) => a + b,0);
    var grade = points! * 100/ +(this.feedback!.feedbackItems.length * 20);
    var gradeString = grade > 90 ? "A+" : grade > 80 ? "A" : grade > 70 ? "B+" : grade > 60 ? "B" : grade > 50 ? "C" : "D";
    this.form.get("gradeAssessedByClient")?.setValue(gradeString + "(" + Math.round(grade) + "%)");
  }

  showFeedback() {
   
      if(this.feedbackIdSelected === this.lastFeedbackIdSelected && this.feedbackIdSelected ===-1) return;

      this.service.getFeedbackObject(this.feedbackIdSelected,0).subscribe({
        next: response => {
          this.feedback = response;
          this.InitializeForm(this.feedback);
        }
      })
      this.lastFeedbackIdSelected = this.feedbackIdSelected;
  }

  newFeedback() {
    if(this.feedback!.customerId === 0) return;

    this.service.getFeedbackObject(0, this.feedback!.customerId).subscribe({
      next: response => {
        this.feedback = response;
        this.InitializeForm(this.feedback);
      }
    })
  }

  sendEmail() {
    if(this.feedback!.id === 0) {
      this.toastr.warning('This feedback needs to be saved before its link can be forwarded to the customer', 'feedback not saved');
      return;
    }

    this.service.sendFeedbackMail(this.feedback!.id).subscribe({
      next: (response: string) => {
        if(response ==='' || response === null) {
          this.toastr.success('Feedback mail composed, and can be viewed in messages', 'Message composed')
        } else {
          this.toastr.info(response, 'Failed to compose message')
        }
      }, error : (response: any) => {
        this.toastr.error(response.error.details, 'Error in composing email message')
      }
    })
  }
  
}
