import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ICandidateFlightGrp } from 'src/app/_models/process/candidateFlightGrp';
import { ICandiateFlightItem } from 'src/app/_models/process/candidateFlightItem';
import { DeployService } from 'src/app/_services/deploy.service';

@Component({
  selector: 'app-candidate-flight-item',
  templateUrl: './candidate-flight-item.component.html',
  styleUrls: ['./candidate-flight-item.component.css']
})
export class CandidateFlightItemComponent implements OnInit{
  
  @Input() dto: ICandidateFlightGrp | undefined;

  flightCandidates: ICandiateFlightItem[]=[];

  @Output() editEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Output() travelAdviseEvent = new EventEmitter<number>();
  @Output() selectedEvent = new EventEmitter<number>();

  bExpand=false;

  constructor(private service: DeployService){}

  ngOnInit(): void {
    console.log(this.dto);
  }

  editClicked() {
    this.editEvent.emit(this.dto!.id);
  }

  deleteClicked() {
    this.deleteEvent.emit(this.dto!.id);
  }

  travelAdviseClicked() {
    this.travelAdviseEvent.emit(this.dto?.id);
  }

  /*ExpandChanged() {
    this.bExpand = !this.bExpand;

    if(!this.bExpand) {
      this.flightCandidates=[];
    } else {
      this.service.getCandidateFlightCandidates(this.dto!.id).subscribe({
        next: (response: ICandiateFlightItem[] | null) => {
          if(response===null || response.length===0) {
            this.flightCandidates=[];
          } else {
            this.flightCandidates=response;
          }
        }
      })
    }
  }
    */

}
