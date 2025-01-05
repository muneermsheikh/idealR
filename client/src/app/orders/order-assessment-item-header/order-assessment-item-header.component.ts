import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IOrderAssessmentItemHeaderDto } from 'src/app/_dtos/admin/orderAssessmentItemHeaderDto';
import { OrderAssessmentItem } from 'src/app/_models/admin/orderAssessmentItem';
import { IOrderAssessmentItemQ } from 'src/app/_models/admin/orderAssessmentItemQ';
import { OrderAssessmentService } from 'src/app/_services/hr/orderAssessment.service';

@Component({
  selector: 'app-order-assessment-item-header',
  templateUrl: './order-assessment-item-header.component.html',
  styleUrls: ['./order-assessment-item-header.component.css']
})
export class OrderAssessmentItemHeaderComponent implements OnInit {

  @Input() profGroup: string = '';
  @Input() targetAssessmentItem: OrderAssessmentItem | undefined;
  @Output() sectionCloseEvent = new EventEmitter<boolean>();
  @Output() targetAssessmentItemQsEvent = new EventEmitter<IOrderAssessmentItemQ[]>();

  headers: IOrderAssessmentItemHeaderDto[]=[];
  assessmentItemProfessionGroupSelected: string='';
    
  constructor(private service: OrderAssessmentService){}

  ngOnInit(): void {
    this.service.getOrderAssessmentItemHeaderFromProfessionGroup(this.profGroup).subscribe({
      next: (response: IOrderAssessmentItemHeaderDto[]) => {
        this.headers = response;
        this.sectionCloseEvent.emit(true);
      }
    })
  }
  
  populateQs() {

    var id = this.headers.filter(x => x.checked===true).map(x => x.orderItemId);
    if(id===null || id===undefined) return;
    
    this.service.getOrderAssessmentItemQsFromOrderItemId(+id).subscribe({
      next: (response: IOrderAssessmentItemQ[]) => {
        console.log('response:', response, 'targetassessmentitem:', this.targetAssessmentItem);
        this.targetAssessmentItemQsEvent.emit(response); 
      }
    })

  }

  selectedClicked(id: number) {
    this.headers.forEach(x => {
      if(x.id !== id) x.checked=false;
    })
  }

  closeClicked() {
    this.sectionCloseEvent.emit(true);
  }
}
