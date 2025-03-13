import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IVisaBriefDto } from 'src/app/_dtos/admin/visaBriefDto';
import { LocalStorageService } from 'src/app/_services/admin/local-storage.service';
import { VisaService } from 'src/app/_services/admin/visa.service';

@Component({
  selector: 'app-visa-line',
  templateUrl: './visa-line.component.html',
  styleUrls: ['./visa-line.component.css']
})
export class VisaLineComponent implements OnInit{

  @Input() visa: IVisaBriefDto | undefined;
  @Input() isReport: boolean = false;

  @Output() editEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() assignEvent = new EventEmitter<IVisaBriefDto>();
  @Output() consumedEvent = new EventEmitter<number>();

  displayAssignRole=false;

  loggedInUserHasVisaEditCredential = false;
  
  constructor(private service: VisaService, private toastr: ToastrService,
    private localStorageService: LocalStorageService){
    this.loggedInUserHasVisaEditCredential = localStorageService.loggedInUserhasVisaEditRole();
  }
  
  ngOnInit(): void {
    
  }
  editEventClicked() {
    this.editEvent.emit(this.visa?.id)
  }

  deleteEventClicked() {
    this.deleteEvent.emit(this.visa?.id)
  }
  
  visaConsumedClicked() {
    this.consumedEvent.emit(this.visa?.id)
  }
  
  assignEventClicked() {
   this.assignEvent.emit(this.visa);
  }
  getOpenVisasForVisaCompany() {

  } 

  

  

}
