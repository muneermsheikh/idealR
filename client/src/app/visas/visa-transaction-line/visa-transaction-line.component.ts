import { Component, Input, OnInit, Output } from '@angular/core';
import { IVisaTransaction } from 'src/app/_models/admin/visaTransaction';
import { VisaService } from 'src/app/_services/admin/visa.service';

@Component({
  selector: 'app-visa-transaction-line',
  templateUrl: './visa-transaction-line.component.html',
  styleUrls: ['./visa-transaction-line.component.css']
})
export class VisaTransactionLineComponent implements OnInit {

    @Input() visa: IVisaTransaction | undefined;
    @Input() isReport: boolean = false;
    
    constructor(private service: VisaService){}

    ngOnInit(): void {
      
    }

    
}
