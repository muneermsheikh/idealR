import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { delay, map, Observable, of } from 'rxjs';
import { IOrderItemBriefDto } from 'src/app/_dtos/admin/orderItemBriefDto';
import { IOrderItemForVisaAssignmentDto } from 'src/app/_dtos/admin/orderItemForVisaAssignmentDto';
import { IVisaBriefDto } from 'src/app/_dtos/admin/visaBriefDto';
import { IVisaAssignment } from 'src/app/_models/admin/visaAssignment';
import { IVisaItem } from 'src/app/_models/admin/visaItem';
import { VisaService } from 'src/app/_services/admin/visa.service';

@Component({
  selector: 'app-visa-assign-modal',
  templateUrl: './visa-assign-modal.component.html',
  styleUrls: ['./visa-assign-modal.component.css']
})
export class VisaAssignModalComponent implements OnInit{

  @Input() title: string = '';
  @Input() visaItem: IVisaBriefDto | undefined;
  
  @Input() orderItemsForTheCustomer: IOrderItemForVisaAssignmentDto[]=[];
  
  @Output() assignEvent = new EventEmitter<IVisaAssignment[]>();
  visaAssignments: IVisaAssignment[]=[];
  qntyAssigned=0;

  constructor(private toastr: ToastrService, public bsModalRef: BsModalRef, private service: VisaService){}

  ngOnInit(): void {
  
  }
  
  qntyChanged(oItem: IOrderItemForVisaAssignmentDto) {
    if(oItem.quantityAssigned > oItem.unassigned) {
      this.toastr.warning("Quantity assigned (" + this.qntyAssigned + 
          ") is more than visa avaialability (" + oItem.unassigned + ")", "Input error", {closeButton: true, extendedTimeOut: 0});
    }
  }

  canceled() {
    this.qntyAssigned = 0;
  } 
  
  close() {
    this.bsModalRef.hide()
  }
  update() {
    var orderitem = this.orderItemsForTheCustomer.find(x => x.checked===true );
    this.orderItemsForTheCustomer.find(x => x.orderItemId !== orderitem?.orderItemId);
    var visaAssignment: IVisaAssignment = {
      orderItemId: orderitem!.orderItemId, 
      visaItemId: this.visaItem!.id, 
      visaQntyAssigned: orderitem!.quantityAssigned,
      dateAssigned: new Date()
    }
    this.visaAssignments.push(visaAssignment);
    this.assignEvent.emit(this.visaAssignments);
    this.bsModalRef.hide();
  }
}


