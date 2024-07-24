import { getMultipleValuesInSingleSelectionError } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IOrderBriefDto } from 'src/app/_dtos/admin/orderBriefDto';
import { OrderForwardToHR } from 'src/app/_models/orders/orderForwardToHR';
import { OrderForwardService } from 'src/app/_services/admin/order-forward.service';

@Component({
  selector: 'app-order-item',
  templateUrl: './order-item.component.html',
  styleUrls: ['./order-item.component.css']
})
export class OrderItemComponent implements OnInit{

  @Input() order: IOrderBriefDto | undefined;

  @Output() editEvent = new EventEmitter<number>();
  @Output() contractReviewEvent = new EventEmitter<number>();
  @Output() orderFwdToAssociatesEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() acknowledgeToClientEvent = new EventEmitter<number>();
  @Output() cvreferredEvent = new EventEmitter<number>();
  @Output() orderAssessmentItemEvent = new EventEmitter<number>();
  @Output() selectionEvent=new EventEmitter<number>();

  menuTopLeftPosition =  {x: 0, y: 0}

  constructor(private orderFwdService: OrderForwardService, private toastr: ToastrService) { }

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
          next: (response: boolean) => {
            if(response) {
              this.toastr.success('task created for HR Dept', 'success')
            } else {
              this.toastr.warning('failed to forward the Order to HR Dept', 'failure')
            }
          },
          error: (error: any) => this.toastr.error(error, 'error encountered')
        });
    }
    
  }
  
  
  dlForwardToAssociatesClicked() {
    this.orderFwdToAssociatesEvent.emit(this.order?.id);
  }

  AcknowledgeClicked() {
    this.acknowledgeToClientEvent.emit(this.order?.id);
  }

  cvReferredClicked() {
    this.cvreferredEvent.emit(this.order?.id);
  }
}
