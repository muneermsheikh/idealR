import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, of, switchMap, tap } from 'rxjs';
import { Pagination } from 'src/app/_models/pagination';
import { ICandidateFlight } from 'src/app/_models/process/candidateFlight';
import { ICandidateFlightGrp } from 'src/app/_models/process/candidateFlightGrp';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { DeployService } from 'src/app/_services/deploy.service';

@Component({
  selector: 'app-candidate-flights',
  templateUrl: './candidate-flights.component.html',
  styleUrls: ['./candidate-flights.component.css']
})
export class CandidateFlightsComponent implements OnInit {

  cFlight: ICandidateFlightGrp | undefined;
  
  form: FormGroup=new FormGroup({});
  
  constructor(private service: DeployService, private fb: FormBuilder,
      private toastr: ToastrService, private confirm: ConfirmService, private activatedRoute: ActivatedRoute){}

  ngOnInit(): void {
    this.activatedRoute.data.subscribe(data => { 
      this.cFlight = data['cFlight'];
    })

    if(this.cFlight) this.InitializeForm(this.cFlight);
  }

  InitializeForm(c: ICandidateFlightGrp) {

      this.form = this.fb.group({
        id: c.id, dateOfFlight: c.dateOfFlight,
        airlineName: c.airlineName, flightNo: c.flightNo,
        airportOfBoarding: c.airportOfBoarding,
        airportOfDestination: c.airportOfDestination,
        eTD_Boarding: c.eTD_Boarding,
        eTA_Destination: c.eTA_Destination,
        airportVia: c.airportVia,
        flightNoVia: c.flightNoVia,
        eTA_Via: c.eTA_Via,
        eTD_Via: c.eTD_Via,
        fullPath: c.fullPath,

        candidateFlightItems: this.fb.array(
          c.candidateFlightItems.map(x => (
            this.fb.group({
              id: x.id, depId: x.depId, depItemId: x.depItemId, cvRefId: x.cvRefId,
              applicationNo: x.applicationNo, candidateName: x.candidateName,
              customerName: x.customerName, customercity: x.customerCity
            })
          ))
        )
      })
    
  }

  get candidateFlightItems(): FormArray {
    return this.form.get("candidateFlightItems") as FormArray
  }
 
  removeCandidateFlightItem(index: number) {
    this.candidateFlightItems.removeAt(index);
    this.candidateFlightItems.markAsDirty();
    this.form.markAsDirty();
  }

  getCandidateFlights() {
  
    var params = this.service.getParams();
     this.service.getDeploymentPagedList(params)?.subscribe({
      next: response => {
        if(response !== undefined && response !== null) {
          this.cFlight = response.result;
         
        } else {
          console.log('response is undefined');
        }
      },
      error: error => console.log(error)
     })
    }
  
  deleteFlightCandidate(event: any) {
    
    var id = event;

    const observableOuter = this.confirm.confirm('confirm Delete', 
      'Are you sure you want to remove this candidate from the flight?  This will also remove the flight transaction'
    );

    observableOuter.pipe(
      filter((response) => response),

      switchMap(confirmed => this.service.deleteFlightCandidate(id).pipe(
        catchError(err => {
          this.toastr.error(err, 'failed to delete');
          return of();
        }),
        tap(res => {
          this.toastr.error('Deleted the Candidate passenger from the flight', 'success in deletion')
        }),
        catchError(err => {
          this.toastr.error(err, 'Failed to delete the Candidate from the flight');
          return of();
        })
      ))
    ).subscribe(
      () => this.toastr.success('Candidate Flight deleted', 'success')
    )
  }
      
 
}
