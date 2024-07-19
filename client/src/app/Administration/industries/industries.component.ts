import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IIndustryType } from 'src/app/_models/admin/industryType';
import { Pagination } from 'src/app/_models/pagination';
import { IndustryParams } from 'src/app/_models/params/masters/industryParams';
import { User } from 'src/app/_models/user';
import { IndustriesService } from 'src/app/_services/admin/industries.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CategoryEditModalComponent } from '../category-edit-modal/category-edit-modal.component';
import { catchError, filter, of, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-industries',
  templateUrl: './industries.component.html',
  styleUrls: ['./industries.component.css']
})
export class IndustriesComponent implements OnInit {
  
  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  title='';
  pParams = new IndustryParams();
  i=1;

  user?: User;
  
  industries: IIndustryType[]=[];
  pagination: Pagination | undefined;
  totalCount = 0;
  
  bsModalRef: BsModalRef | undefined;

  constructor(private service: IndustriesService, 
    private modalService: BsModalService, 
    private toastr: ToastrService,
    private confirm: ConfirmService){}


  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    var params = this.service.getParams();
    this.service.getIndustryPaged(params).subscribe({
      next: response => {
        if(response !== undefined && response !== null) {
          this.industries=  response.result;
          this.totalCount = response.count;
          this.pagination = response.pagination;
          console.log('industries response', response);
        } 
      }, error: error => this.toastr.error(error, 'Error encountered')
    })
  }
  

  setParameters() {
    var params = new IndustryParams();
    this.service.setParams(params);
    this.loadData();
  }
  
  addIndustry() {
    this.editIndustry(0, '');
  }

  editIndustry(catId: number,catName: string) {
    var prof={id: catId, professionName: catName};
    const config = {
      class: 'modal-dialog-centered modal-md',
      initialState: {
        category: prof,
        title: 'Industry'
      }
    }

    this.bsModalRef = this.modalService.show(CategoryEditModalComponent, config);

    const observableOuter =  this.bsModalRef.content.updateEvent;    //returns a sngle object, not collection

    observableOuter.pipe(
      filter((response: IIndustryType) => response !== undefined),
      
      switchMap((response: IIndustryType) => {
          return this.service.updateIndustry(response.id, response.industryName)
      })
    ).subscribe((response: IIndustryType) => {
        if(response !== null) {
          if(catId===0) {
            this.industries.push(response);
          } else {
            var index=this.industries.findIndex(x => x.id === catId);
            if(index !== -1) this.industries.splice(index, 1, response );
          }
        this.toastr.success('Success', catId===0 ? 'Inserted' : 'Updated' );

        }
    }) 
  
  }

  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.loadData();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.pParams = new IndustryParams();
    this.service.setParams(this.pParams);
    this.loadData();
  }

  onPageChanged(event: any){
    const params = this.service.getParams() ?? new IndustryParams();
    
    if (params.pageNumber !== event.page) {
      params.pageNumber = event.page;
      this.service.setParams(params);
      this.loadData();
    }
  }

  deleteIndustry (id: number){
    this.confirm.confirm('confirm delete this Category', 'confirm delete').pipe(
      switchMap(confirmed => this.service.deleteIndustry(id).pipe(
        catchError(err => {
          console.log('Error in deleting the Categoory', err);
          return of();
        }),
        tap(res => this.toastr.success('deleted Category')),
        //tap(res=>console.log('delete voucher succeeded')),
      )),
      catchError(err => {
        this.toastr.error('Error in getting delete confirmation', err);
        return of();
      })
    ).subscribe(
        () => {
          console.log('delete succeeded');
          this.toastr.success('order deleted');
        },
        (err: any) => {
          console.log('any error NOT handed in catchError() or if throwError() is returned instead of of() inside catcherror()', err);
      })
  }

}
