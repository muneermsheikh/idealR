import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Navigation, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { IFeedbackDto } from 'src/app/_dtos/hr/feedbackDto';
import { Pagination } from 'src/app/_models/pagination';
import { FeedbackParams } from 'src/app/_models/params/hr/feedbackParams';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { FeedbackService } from 'src/app/_services/feedback.service';

@Component({
  selector: 'app-feedback-list',
  templateUrl: './feedback-list.component.html',
  styleUrls: ['./feedback-list.component.css']
})
export class FeedbackListComponent implements OnInit {

  
  @ViewChild('search', {static: false}) searchTerm?: ElementRef;
  
  feedbacks: IFeedbackDto[]=[];
  pagination: Pagination | undefined;

  fParams = new FeedbackParams();

  totalCount=0;

  user?: User;

  returnUrl='';

  constructor(private router: Router, private service: FeedbackService,
    private confirm:ConfirmService, private toastr: ToastrService ) {
    let nav: Navigation|null = this.router.getCurrentNavigation() ;

        if (nav?.extras && nav.extras.state) {
        if(nav.extras.state['returnUrl']) this.returnUrl=nav.extras.state['returnUrl'] as string;

        if( nav.extras.state['user']) this.user = nav.extras.state['user'] as User;
      }
  }

  ngOnInit(): void {
    this.loadFeedbacks()
  }

  loadFeedbacks() {
    
      this.service.setParams(this.fParams ?? new FeedbackParams());

      this.service.getFeedbacks(this.fParams).subscribe({
        next: response => {
          if(response.result && response.pagination) {
            this.feedbacks = response.result;
            this.totalCount = response.count;
            this.pagination = response.pagination;
          }
        }
      })
    
  }

  navigateByUrl(route: string) {
    this.router.navigate(
      [route],
      { state: 
        { 
          user: this.user,
          returnUrl: '/feedback/list' 
        } }
    )
  }

  
  onSearch() {
    const params = this.service.getParams() ?? new FeedbackParams();
    
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.loadFeedbacks();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.fParams = new FeedbackParams();
    this.service.setParams(this.fParams);
    this.loadFeedbacks();
  }

  
  onPageChanged(event: any){

    const params = this.service.getParams() ?? new FeedbackParams();
    if (params.pageNumber !== event.page) {
      params.pageNumber = event.page;
      this.service.setParams(params);
      this.loadFeedbacks();
    }
  }

    editClicked(event: any) {   //emitted: IFeedback
        this.navigateByUrl('/feedback/edit/' + event );
    }

    deleteClicked(event: any) {
      var id=event;
      var confirmMsg = 'confirm delete this Feedback record?. WARNING: this cannot be undone';

      const observableInner = this.service.deleteFeedback(id);
      const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

      observableOuter.pipe(
          filter((confirmed) => confirmed),
          switchMap((confirmed) => {
            return observableInner
          })
      ).subscribe(response => {
        if(response) {
          this.toastr.success('Customer deleted', 'deletion successful');
          var index = this.feedbacks.findIndex(x => x.id == id);
          if(index >= 0) this.feedbacks.splice(index,1);
        } else {
          this.toastr.error('Error in deleting the Feedback', 'failed to delete')
        }
        
      });
    }

    emailClicked(event: any) {    //emits feedback.Id
    
      this.service.sendFeedbackEmailToClient(event)
    }
  
}
