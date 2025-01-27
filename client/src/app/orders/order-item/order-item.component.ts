import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IOrderBriefDto } from 'src/app/_dtos/admin/orderBriefDto';
import { OrderForwardService } from 'src/app/_services/admin/order-forward.service';
import { InterviewService } from 'src/app/_services/hr/interview.service';

@Component({
  selector: 'app-order-item',
  templateUrl: './order-item.component.html',
  styleUrls: ['./order-item.component.css']
})
export class OrderItemComponent implements OnInit{

  @Input() order: IOrderBriefDto | undefined;
  @Input() isReportActive: boolean = false;

  @Output() editEvent = new EventEmitter<number>();
  @Output() contractReviewEvent = new EventEmitter<number>();
  @Output() orderFwdToAssociatesEvent = new EventEmitter<number>();
  @Output() DLForwardedEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() acknowledgeToClientEvent = new EventEmitter<number>();
  @Output() cvreferredEvent = new EventEmitter<number>();
  @Output() orderAssessmentItemEvent = new EventEmitter<number>();
  @Output() selectionEvent=new EventEmitter<number>();
  
  menuTopLeftPosition =  {x: 0, y: 0}

  constructor(private orderFwdService: OrderForwardService, private toastr: ToastrService, private intervwService: InterviewService) { }

  ngOnInit(): void {

  }

  contractReviewClicked() {
    this.contractReviewEvent.emit(this.order!.id);
  }

  orderAssessmentItemClicked() {
      if(this.order) this.orderAssessmentItemEvent.emit(this.order.id);
  }

  editClicked() {

    this.editEvent.emit(this.order!.id);
  }
  
  selectionClicked() {
    this.selectionEvent.emit(this.order!.id);
  }

  deleteClicked() {
    this.deleteEvent.emit(this.order?.id);
  }

 
  forwardOrderToHRDept() {

    if(this.order) {
        
        this.orderFwdService.forwarOrderToHR(this.order.id).subscribe({
          next: (response: string) => {
            if(response === '' || response === null) {
              this.toastr.success('task created for HR Dept and message created and saved in drafts', 'success')
            } else {
              this.toastr.warning(response, 'failure')
            }
          },
          error: (err: any) => this.toastr.error(err?.error?.details, 'error encountered')
        });
    }
    
  }
  

  dlForwardToAssociatesClicked() {
    console.log('orderid in modal', this.order?.id);
    this.orderFwdToAssociatesEvent.emit(this.order?.id);
  }

  DLForwardedClicked() {
    this.DLForwardedEvent.emit(this.order?.id);
  }

  AcknowledgeClicked() {
    this.acknowledgeToClientEvent.emit(this.order?.id);
  }

  cvReferredClicked() {
    this.cvreferredEvent.emit(this.order?.id);
  }

  
}
