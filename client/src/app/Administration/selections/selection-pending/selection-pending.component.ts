import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { createSelDecisionDto } from 'src/app/_dtos/admin/createSelDecisionDto';
import { ISelPendingDto } from 'src/app/_dtos/admin/selPendingDto';
import { Pagination } from 'src/app/_models/pagination';
import { SelDecisionParams } from 'src/app/_models/params/Admin/selDecisionParams';
import { User } from 'src/app/_models/user';
import { SelectionService } from 'src/app/_services/hr/selection.service';


@Component({
  selector: 'app-selection-pending',
  templateUrl: './selection-pending.component.html',
  styleUrls: ['./selection-pending.component.css']
})
export class SelectionPendingComponent implements OnInit{

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  selectionsPending: ISelPendingDto[]=[];
  pendingSelectionsSelected: ISelPendingDto[]=[];

  selectionStatus='';

  pagination: Pagination | undefined;
  user?: User;
  
  cvsSelected: ISelPendingDto[]=[];
  sParams = new SelDecisionParams();

  pageIndex=1;
  totalCount=0;

  todayDate = new Date(Date.now());

  rejectionStatuses =[
    { name: 'Selected'}, {name: 'Rejected-Low Salary'}, {name: 'Rejected-Low Exp'}, {name: 'Rejected-irrelevant Exp'},
    {name: 'Rejected-other reasons'}
  ]


  constructor(private service: SelectionService,
      private toastr: ToastrService){}

  ngOnInit(): void {
    
    this.getPendingSelectionsPaged(false);
  }

  getPendingSelectionsPaged(useCache: boolean=true)
  {
    //this.sParams=sParams;
    this.service.setParams(this.sParams);

    this.service.getPendingSelections(useCache).subscribe({
      next: response => {

        if(response.result && response.pagination) {
          this.selectionsPending = response.result;
          this.totalCount = response.count;
          this.pagination = response.pagination;
        } else {
          this.toastr.warning('There are no pending selections', 'No record found')
        }
      },
      error: error => this.toastr.error(error)
    });
  }
  
  
  convertSelDecisionToDto (sel: ISelPendingDto[]): any {  //CreateSelDecision[] |undefined | null {

    if(sel.length===0) {
      this.toastr.warning('no selections made to save');
      return undefined;
    } 
    var dtos: createSelDecisionDto[]=[];

    sel.forEach(s => {
      var dto: createSelDecisionDto ={
        cVRefId :  s.cvRefId, selectionStatus : this.selectionStatus ?? '',
        decisionDate : this.todayDate, remarks: s.remarks ?? ''};

      dtos.push(dto);
    })
   
    return dtos;
  
  }

  UpdateSelections() 
  {
      if(this.pendingSelectionsSelected.length === 0) return;
      this.pendingSelectionsSelected = this.pendingSelectionsSelected.filter(x => x.checked);

      if(this.selectionStatus === '' || this.pendingSelectionsSelected.length === 0) {
        this.toastr.warning('Please press the MARK AS SELECTED Button, or choose one of the rejectioon reasons to proceed. ' +
          'also, atleast one candidate must be selected', 
          'Selection Status not chosen');
        return;
      }

      //convert selDecision to dto
      var dtos =  this.convertSelDecisionToDto(this.pendingSelectionsSelected);
       
        return this.service.registerSelectionDecisions(dtos).subscribe({
          next: (response: any) => {
            this.toastr.success('selection decisions registered');

            var cvRefIdsInserted=response;
            if (cvRefIdsInserted.length === 0) return;

            cvRefIdsInserted.forEach((i: number) =>{
                var index = this.selectionsPending.findIndex(x => x.cvRefId===i);
                if (index >=0) {
                  this.selectionsPending.splice(index,1);
                  var selIndex = this.pendingSelectionsSelected.findIndex(x => x.cvRefId ===i);
                  if(selIndex >=0) this.pendingSelectionsSelected.splice(selIndex,1);
                }
              })
          },
          error: (err: any) => {
            console.log('error:', err);
          this.toastr.error(err.details, err.message);
          }
        })
  }

  onPageChanged(event: any){
    const params = this.service.getParams();

    if (params.pageNumber !== event.page) {
        params.pageNumber = event.page;
        this.service.setParams(params);
        this.getPendingSelectionsPaged(true);
    }
  }

  onSortSelected(event: any) {
    var sort = event.value;

    const prms = this.service.getParams();
    prms.sort = sort;
    prms.pageNumber=1;
    prms.pageSize=10;
    this.service.setParams(prms);
    this.getPendingSelectionsPaged(true);
  }

  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageNumber = 1;
    this.service.setParams(params);
    this.getPendingSelectionsPaged(true);
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.sParams = new SelDecisionParams();
    this.service.setParams(this.sParams);
    this.getPendingSelectionsPaged(true);
  }

  changeSelectionStatus(status: any)  {
    this.selectionStatus = status.name;
    console.log('selection status', this.selectionStatus);
  }

  pendingSelected(pendg: any) {

    if(pendg.checked) {
      var found= this.pendingSelectionsSelected.find(x => x.cvRefId === pendg.cvRefId);
      if(!found) {
        this.pendingSelectionsSelected.push(pendg);
      } else {
        found.checked=pendg.checked;
      }
    } else {    //splice the record from sele
      var foundIndex = this.pendingSelectionsSelected.findIndex(x => x.cvRefId===pendg.cvRefId);
      if(foundIndex >=0) this.pendingSelectionsSelected.splice(foundIndex,1);
    }

  }
}
