import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ICandidateFlightData } from 'src/app/_models/process/candidateFlightData';
import { IFlightdata } from 'src/app/_models/process/flightData';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { DeployService } from 'src/app/_services/deploy.service';

@Component({
  selector: 'app-choose-flight-modal',
  templateUrl: './choose-flight-modal.component.html',
  styleUrls: ['./choose-flight-modal.component.css']
})
export class ChooseFlightModalComponent implements OnInit {

  @Input() updateFlight = new EventEmitter<ICandidateFlightData>();
  @Output() selectedEvent = new EventEmitter<IFlightdata>();
  
  destinationAirport: string = '';
  filterDestination: string = '';

  flightdata: IFlightdata[]=[];   //used in templates display - filtered or otherwise
  originalFlightdata: IFlightdata[]=[];   //retrieved from api

  flightSelected: IFlightdata | undefined;
  
  bsValue = new Date();
  bsRangeValue= new Date();
  maxDate = new Date();
  minDate = new Date();
  bsValueDate = new Date();
  flightDate = new Date();

  constructor(public bsModalRef: BsModalRef
    //, private toastr:ToastrService
    , private confirmService: ConfirmService, private service: DeployService
    //, private fb: FormBuilder 
    ) {

      service.getFlightData().subscribe({
        next: response => {
          this.originalFlightdata = response;
          this.filterDestination = 'Yes';
          console.log(response);
        }
      })
    }
  
  ngOnInit(): void {
    
  }

  selectedClicked(flt: any) {   //flt: IFlightData
      this.flightSelected=flt;
      this.flightdata.filter(x => x.id != flt.id).forEach(x => x.checked=false);
    //ts = new Date((new Date()).getTime() + 24*60*60*1000);
  }

  filter() {

      this.flightdata =this.filterDestination==='Y' 
          ? this.flightdata.filter(x => x.airportOfDestination === this.destinationAirport) 
          : this.originalFlightdata;
   }

  close() {
    this.updateFlight.emit(undefined);
    this.bsModalRef.hide();
  }

  registerDate() {
      if(this.flightDate.getFullYear() < 2000) {
        return;
      }

      if(this.flightSelected !== undefined) {
          let etd: Date = new Date(this.flightDate.getTime());
          let[hours, mins] = this.flightSelected.eTD_Boarding.toString().split(":");
          etd.setHours(etd.getHours() + +hours + 5);    //angular decreases the time by 5:30 hrs, dont know why.
          etd.setMinutes(etd.getMinutes() + +mins + 30);

          let eta: Date = new Date(this.flightDate.getTime());
          let[hrs, mints] = this.flightSelected.eTA_Destination.toString().split(":");
          eta.setHours(eta.getHours() + +hrs + 5);
          eta.setMinutes(eta.getMinutes() + +mints + 30);

          console.log('etd', etd, 'eta', eta);
          
    
          var candFlightData: ICandidateFlightData = {
              dateOfFlight: this.flightDate, flightNo: this.flightSelected!.flightNo, 
              airportOfBoarding: this.flightSelected!.airportOfBoarding, 
              flightNoVia: this.flightSelected!.flightNoVia, depId: 0,
              airportOfDestination: this.flightSelected!.airportOfDestination, 
              eTD_Boarding: etd, eTA_Destination: eta, airportVia: this.flightSelected.airportVia };
              
            this.updateFlight.emit(candFlightData);
            this.bsModalRef.hide();
        
      }
    
  }

 
}
