import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IQualification } from 'src/app/_models/hr/qualification';
import { professionParams } from 'src/app/_models/params/masters/ProfessionParams';
import { User } from 'src/app/_models/user';
import { QualificationService } from 'src/app/_services/admin/qualification.service';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { CategoryEditModalComponent } from '../category-edit-modal/category-edit-modal.component';
import { IProfession } from 'src/app/_models/masters/profession';
import { catchError, of, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-qualifications',
  templateUrl: './qualifications.component.html',
  styleUrls: ['./qualifications.component.css']
})
export class QualificationsComponent implements OnInit{

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  title='';
  pParams = new professionParams();
  i=1;

  user?: User;
  
  qualifications: IQualification[]=[];
  bsModalRef: BsModalRef | undefined;

  constructor(private service: QualificationService, 
    private modalService: BsModalService, 
    private toastr: ToastrService,
    private confirm: ConfirmService){}


  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
   this.service.getQualificationList().subscribe({
      next: response => {
        if(response !== undefined && response !== null) {
          this.qualifications=  response;
        } 
      }, error: error => this.toastr.error(error, 'Error encountered')
    })
  }
  

  addQualification() {
    this.editQualification(0,'');
  }

  editQualification(id: number, st: string) {
    var qual = {id: id, professionName: st};

    const config = {
      class: 'modal-dialog-centered modal-md',
      initialState: {
        category: qual,
        title: 'Qualification'
      }
    }

    this.bsModalRef = this.modalService.show(CategoryEditModalComponent, config);

    this.bsModalRef.content.updateEvent.subscribe({
      next: (response: IProfession) => {
        if(response) {
          this.service.updateQualification(response.id, response.professionName).subscribe({
            next: succeeded => {
              if(succeeded) {
                this.toastr.success('Qualification updated', 'success')
              }
            }, error: err => this.toastr.error(err, 'Error')
          })
        }
      }
    })
  
  }

  onSearch() {
    var search = this.searchTerm?.nativeElement.value;
    this.qualifications.filter(x => x.qualificationName==search);
  }

  onReset() {
    this.qualifications.filter
  }

  deleteQualification (id: number){
    this.confirm.confirm('confirm delete this Qualification', 'confirm delete').pipe(
      switchMap(confirmed => this.service.deleteQualification(id).pipe(
        catchError(err => {
          console.log('Error in deleting the Qualification', err);
          return of();
        }),
        tap(res => this.toastr.success('deleted Qualification')),
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
