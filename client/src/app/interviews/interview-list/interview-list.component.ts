import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IInterviewBrief } from 'src/app/_models/hr/interviewBrief';
import { Pagination } from 'src/app/_models/pagination';
import { interviewParams } from 'src/app/_models/params/Admin/interviewParams';
import { User } from 'src/app/_models/user';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { InterviewService } from 'src/app/_services/hr/interview.service';
import { filter, switchMap } from 'rxjs';

@Component({
  selector: 'app-interview-list',
  templateUrl: './interview-list.component.html',
  styleUrls: ['./interview-list.component.css']
})
export class InterviewListComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  interviews: IInterviewBrief[]=[];
  pagination: Pagination | undefined;
  user?: User;

  iParams = new interviewParams();
  orderNoToGet = 0;

  totalCount: number=0;

  bsModalRef: BsModalRef | undefined;
  
  constructor(private service: InterviewService, private activatedRoute: ActivatedRoute,
    private toastr: ToastrService, private modalService: BsModalService, private router: Router,
    private confirm: ConfirmService) {}

  ngOnInit(): void {

    this.activatedRoute.data.subscribe(data => {
      this.interviews = data['interviews'].result;
        this.pagination = data['interviews'].pagination;
        this.totalCount = data['interviews'].totalCount;
    })
  }

  getInterviewsPaged(useCache: boolean) {

    this.service.setParams(this.iParams);
    this.service.getInterviewsPaged(useCache).subscribe({
      next: response => {
        this.interviews = response.result;
        this.pagination = response.pagination;
        this.totalCount = response.totalCount;
      }
    })
    //console.log('interviews:', this.interviews)
  }


  interviewEdit(orderno: number) {    //intervw emitted from line
    this.router.navigateByUrl('/interviews/editintervw/' + orderno);
    
  }

  attendance(orderid:number) {
    this.router.navigateByUrl('/interviews/attendance/' + orderid)
  }

  interviewDelete(event: any) {
    var id=event;

    console.log('interview id:', id);
    var confirmMsg = 'confirm delete this Interview. ' +
      'WARNING: If there are more than one venues for this interview, then only the interview for the venue selected will be deleted.  This cannot be undone';

    const observableInner = this.service.deleteInterview(id);
    const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap(() => {
          return observableInner
        })
    ).subscribe(response => {
      if(response) {
        this.toastr.success('Interview deleted', 'deletion successful');
        var index = this.interviews.findIndex(x => x.id === id);
        if(index !== -1) this.interviews.splice(index,1);
      } else {
        this.toastr.error('Error in deleting the checklist', 'failed to delete')
      }
      
    });

  }

  
  onPageChanged(event: any){
    const params = this.service.getParams() ?? new interviewParams();
    
    if (params.pageNumber !== event.page) {
      params.pageNumber = event.page;
      this.service.setParams(params);
      this.getInterviewsPaged(true);
    }
  }

  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.getInterviewsPaged(false);
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    const params = this.service.getParams();
    this.service.setParams(params);
    this.getInterviewsPaged(false);
  }

  getInterviewR() {
    if(this.orderNoToGet === 0) {
      this.toastr.warning('No Order No provided', 'Invalid Order No');
      return;
    }

    this.router.navigateByUrl('/interviews/editintervw/' + this.orderNoToGet);

  }
  
}
