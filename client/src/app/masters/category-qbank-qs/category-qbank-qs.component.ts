import { Component, Input, OnInit } from '@angular/core';
import { IAssessmentBankQ } from 'src/app/_models/admin/assessmentBankQ';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-category-qbank-qs',
  templateUrl: './category-qbank-qs.component.html',
  styleUrls: ['./category-qbank-qs.component.css']
})
export class CategoryQBankQsComponent implements OnInit {
  
  @Input() assessmentQs: IAssessmentBankQ[] = [];
    
  user?: User;
  
  totalMarks = 0;
  show: boolean=false;

  ngOnInit(): void {
    this.calcualteTotals()
  }

  calcualteTotals() {
    this.totalMarks = this.assessmentQs.map((x: IAssessmentBankQ) => x.maxPoints).reduce((a: number, b: number) => +a + +b);
  }

}
