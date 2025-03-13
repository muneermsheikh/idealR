import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IProfession, Profession } from 'src/app/_models/masters/profession';
import { Pagination } from 'src/app/_models/pagination';
import { professionParams } from 'src/app/_models/params/masters/ProfessionParams';
import { User } from 'src/app/_models/user';
import { CategoryService } from 'src/app/_services/category.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CategoryEditModalComponent } from '../category-edit-modal/category-edit-modal.component';
import { catchError, filter, of, switchMap, tap } from 'rxjs';
import { CategoryQBankModalComponent } from '../category-qbank-modal/category-qbank-modal.component';
import { QbankService } from 'src/app/_services/hr/qbank.service';
import { IAssessmentBank } from 'src/app/_models/admin/assessmentBank';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  title='';
  pParams = new professionParams();
  i=1;

  user?: User;
  
  categories: IProfession[]=[];
  pagination: Pagination | undefined;
  totalCount:number = 0;
  
  bsModalRef: BsModalRef | undefined;

  constructor(private service: CategoryService, 
    private qBkService: QbankService,
    private modalService: BsModalService, 
    private toastr: ToastrService,
    private confirm: ConfirmService){}


  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    //var params = this.service.getParams();
    this.service.setParams(this.pParams);
    this.service.getCategoriesPaged(this.pParams).subscribe({
      next: response => {
        if(response !== undefined && response !== null) {
          this.categories=  response.result;
          this.totalCount = response.count;
          this.pagination = response.pagination;
        } 
        console.log('categories response:', response);
      }, error: error => this.toastr.error(error, 'Error encountered')
    })
  }
  

  setParameters() {
    var params = new professionParams();
    this.service.setParams(params);
    this.loadData();
  }
  
  addCategory() {
    this.editCategory(new Profession());
  }

  editCategory(cat: IProfession) {
    var category=cat;
    
    const config = {
      class: 'modal-dialog-centered modal-md',
      initialState: {
        Category: category,
        title: 'Category'
      }
    }

    this.bsModalRef = this.modalService.show(CategoryEditModalComponent, config);

    const observableOuter =  this.bsModalRef.content.updateEvent;    //returns a sngle object, not collection

    observableOuter.pipe(
      filter((response: IProfession) => response !== undefined),
      
      switchMap((response: IProfession) => {
          return this.service.updateCategory(response)
      })
    ).subscribe((response: IProfession) => {
        if(response !== null || response === true) {
          if(cat.id===0) {
            this.categories.push(response);   //added
          } else {                            //updated
            var index=this.categories.findIndex(x => x.id === cat.id);
            if(index !== -1) this.categories[index]=cat; //.splice(index, 1, response );
          }
        this.toastr.success('Success', cat.id===0 ? 'Inserted' : 'Updated' );

        }
    }) 
  
  }

  onSearch() {
    const params = new professionParams();  // this.service.getParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.pParams = params;
    this.service.setParams(params);
    this.loadData();
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.pParams = new professionParams();
    this.service.setParams(this.pParams);
    this.loadData();
  }

  onPageChanged(event: any){
    const params = this.service.getParams() ?? new professionParams();
    
    if (params.pageNumber !== event.page) {
      params.pageNumber = event.page;
      this.service.setParams(params);
      this.loadData();
    }
  }

  deleteCategory (id: number){
    this.confirm.confirm('confirm delete this Category', 'confirm delete').pipe(
      switchMap(confirmed => this.service.deleteCategory(id).pipe(
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
  
  customAssessmentQs(categoryId: number){
    
    var observableOuter = this.qBkService.getQBankOfCategoryId(categoryId);

    observableOuter.pipe(
      filter((response: IAssessmentBank) => response !==null),
      switchMap((response: any) => {

        const config = {
          class: 'modal-dialog-centered modal-lg',
          initialState: {
            assessment: response,
            user: this.user
          }
        }
        this.bsModalRef = this.modalService.show(CategoryQBankModalComponent, config);
        const observableInner = this.bsModalRef.content.updateEvent;
        return observableInner
      })
    ).subscribe((response: any) => {
      if(response)   {
        this.toastr.success('Viewed/updated the Custom Assessment Parameters for the category', 'Success');
      } else {
        this.toastr.warning('Failed to view/update the Custom Assessment Parameters', 'Failed')
      }
    })

  }

}
