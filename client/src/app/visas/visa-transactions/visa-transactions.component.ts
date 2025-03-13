import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IVisaTransaction } from 'src/app/_models/admin/visaTransaction';
import { Pagination } from 'src/app/_models/pagination';
import { visaParams } from 'src/app/_models/params/Admin/visaParams';
import { User } from 'src/app/_models/user';
import { VisaService } from 'src/app/_services/admin/visa.service';

@Component({
  selector: 'app-visa-transactions',
  templateUrl: './visa-transactions.component.html',
  styleUrls: ['./visa-transactions.component.css']
})
export class VisaTransactionsComponent implements OnInit{

  visas: IVisaTransaction[]=[];
  pagination: Pagination | undefined;
  totalCount = 0;
  totalBal = 0;

  user?: User;
  vParams = new visaParams();

  printVisas: IVisaTransaction[]=[];
  isPrintPDF: boolean = false;

  constructor(private service: VisaService, private toastr: ToastrService,
    private activatedRoute: ActivatedRoute ) {}


  ngOnInit(): void {
    
    this.activatedRoute.data.subscribe(data => {
      this.visas = data['visas'].result;
      this.pagination = data['visas'].pagination;
      this.totalCount = data['visas'].totalCount;
    })

    
    if(this.visas) this.totalBal = +this.visas.map((x: any) => x.visaBalance)
        .reduce((a:number,b:number) => +a+(+b),0);
  }

  loadVisas() {
    var params = this.service.getParams();
    this.service.getPagedVisaTransactions(params)?.subscribe({
    next: response => {
      if(response !== undefined && response !== null) {
        this.visas = response.result;
        this.totalCount = response?.count;
        this.pagination = response.pagination;

      } else {
        console.log('response is undefined');
      }
    },
    error: (err: any) => console.log(err.error?.details, 'Error encountered')
  })
}

  onPageChanged(event: any){
    const params = this.service.getParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event.page;
      this.service.setParams(params);

      this.loadVisas();
    }
  }

  
  generatePDF() {
    this.printVisas = this.visas;

    if(this.printVisas.length) this.isPrintPDF = true;
  }


}
