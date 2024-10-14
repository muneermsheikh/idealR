import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { IFlightdata } from 'src/app/_models/process/flightData';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { DeployService } from 'src/app/_services/deploy.service';

@Component({
  selector: 'app-choose-flight-modal',
  templateUrl: './choose-flight-modal.component.html',
  styleUrls: ['./choose-flight-modal.component.css']
})
export class ChooseFlightModalComponent implements OnInit {
  
  @Input() flightData: IFlightdata | undefined;
  @Output() candidateFlightEvent = new EventEmitter<IFlightdata | null>();
  
  constructor(public bsModalRef: BsModalRef
    , private toastr: ToastrService
    , private confirmService: ConfirmService, private service: DeployService
    ) { }
  
  ngOnInit(): void {}

   
  close() {
    this.candidateFlightEvent.emit(null);
    this.bsModalRef.hide();
  }

 
  registerDate() {
          
      /*let etd: Date = new Date(this.flightDate.getTime());
      let[hours, mins] = this.flightSelected.eTD_Boarding.toString().split(":");
      etd.setHours(etd.getHours() + +hours + 5);    //angular decreases the time by 5:30 hrs, dont know why.
      etd.setMinutes(etd.getMinutes() + +mins + 30);

      let eta: Date = new Date(this.flightDate.getTime());
      let[hrs, mints] = this.flightSelected.eTA_Destination.toString().split(":");
      eta.setHours(eta.getHours() + +hrs + 5);
      eta.setMinutes(eta.getMinutes() + +mints + 30);
      */

        if(!this.DataVerified()) return;
        //this.flightData!.eTA_DestinationString= formatDateTime(this.flightData!.eTA_Destination);
        //this.flightData!.eTD_BoardingString= formatDateTime(this.flightData!.eTD_Boarding);
        this.candidateFlightEvent.emit(this.flightData);
        this.bsModalRef.hide();
  }

  DataVerified() {
    var err='';

      if(!isValidDate(this.flightData!.eTA_DestinationString) 
        || !isValidDate(this.flightData!.eTD_BoardingString)) err = 'Invalid Date - ETA / ETD';
      
      if(this.flightData?.airlineVia !== '') {
        if(!isValidDate(this.flightData!.eTD_ViaString) 
          || !isValidDate(this.flightData!.eTA_ViaString)) err += 'Invalid Date - ETD';
      }
      if(this.flightData?.airlineName=='') err += ' Airline name not mentioned';
      if(this.flightData?.airportOfBoarding=='') err += ' Airport of Boarding not mentioned';
      if(this.flightData?.airportOfDestination == '') err += ' Airport of destination not provided';
      if(this.flightData?.flightNo=='') err += ' Flight No not provided';
      

      if(err == '') {
        return true;
      } else {
        this.toastr.warning(err, 'errors encountered');
        return false;
      }
  }

  dateChanged(eventDate: string): Date | null {
    console.log('dateChanged:', eventDate);
    return !!eventDate ? new Date(eventDate) : null;
  }

}
function padTo2Digits(num: number): string {
  return num.toString().padStart(2, '0');
}

function formatDateTime(date: Date): string {
  console.log('date in formatDateTime', date);
  return (
    date.getFullYear() + '-' +
    padTo2Digits(date.getMonth() + 1) + '-' +
    padTo2Digits(date.getDate()) + 'T' +
    padTo2Digits(date.getHours()) + ':' +
    padTo2Digits(date.getMinutes())
  );
}

function isValidDate(dateString: string): boolean {
  const date = new Date(dateString);
  return !isNaN(date.getTime());
}

