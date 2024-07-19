import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { ICOA } from 'src/app/_models/finance/coa';
import { COAService } from 'src/app/_services/finance/coa.service';

@Component({
  selector: 'app-coa-edit-modal',
  templateUrl: './coa-edit-modal.component.html',
  styleUrls: ['./coa-edit-modal.component.css']
})
export class CoaEditModalComponent implements OnInit {

  @Output() editCOAEvent = new EventEmitter<ICOA>();
  
  matchingCOA: string='';
  matchingNames: string[]=[];

  title: string='';
  coa?: ICOA;
  
  accountClasses=[
    {'accountClass':'exp'}, {'accountClass':'banks'},{'accountClass':'salesiata'},{'accountClass':'personalaccount'}
    ,{'accountClass':'sales'},{'accountClass':'candidate'},{'accountClass':'asset'}
  ]

  constructor(public bsModalRef: BsModalRef, private coaService: COAService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.checkExists();
  }

  checkExists() {
    if(this.coa===null || this.coa!.accountName ==='') return;

    this.matchingNames=[];
    this.coaService.getMatchingCOAs(this.coa!.accountName).subscribe(response => {

      this.matchingNames=response;
    }, error => {
      this.matchingNames=[];
    })
    
  }

  updateCOA() {
    if(this.coa!.accountClass==='' || this.coa!.accountName==='' || this.coa!.accountType==='' || this.coa!.divn==='') {
      this.toastr.warning('all properties except Opening Balance are mandatory');
      return;
    }
    
    this.editCOAEvent.emit(this.coa);
    this.bsModalRef.hide();
  }

  classChanged() {

  }

}
