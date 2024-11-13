import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { IAssessmentBank } from 'src/app/_models/admin/assessmentBank';
import { User } from 'src/app/_models/user';
import { QbankService } from 'src/app/_services/hr/qbank.service';
import { Pagination } from 'src/app/_models/pagination';
import { assessmentQBankParams } from 'src/app/_models/admin/assessmentQBankParams';

@Component({
  selector: 'app-category-qbank',
  templateUrl: './category-qbank.component.html',
  styleUrls: ['./category-qbank.component.css']
})
export class CategoryQBankComponent implements OnInit {

  form: FormGroup = new FormGroup({});
  id: number=0;
  assessments: IAssessmentBank[]=[];
  qParams= new assessmentQBankParams();
  
  @Output() cancelRegister = new EventEmitter();
  @Output() updateEvent = new EventEmitter<boolean>();

  show: boolean=false;
  pagination: Pagination | undefined;
  totalCount=0;
  
  user?: User;
  bsModalRef: BsModalRef | undefined;

  constructor(private service: QbankService){}
  
  ngOnInit(): void {
    this.service.setCustomParams(this.qParams);

    this.service.getQBankPaginated().subscribe({
      next: (response) => {
        console.log('getting response', response);
        if(response.result && response.pagination) {
          this.assessments = response.result;
          this.pagination = response.pagination;
          this.totalCount = response.count;
        }
      }
    })
  }

  onPageChanged(event: any) {

  }
}
