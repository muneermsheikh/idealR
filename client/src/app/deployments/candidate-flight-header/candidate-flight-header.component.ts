import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { filter, switchMap } from 'rxjs';
import { Pagination } from 'src/app/_models/pagination';
import { CandidateFlightParams } from 'src/app/_models/params/process/CandidateFlightParams';
import { ICandidateFlightGrp } from 'src/app/_models/process/candidateFlightGrp';
import { ConfirmService } from 'src/app/_services/confirm.service';
import { DeployService } from 'src/app/_services/deploy.service';

@Component({
  selector: 'app-candidate-flight-header',
  templateUrl: './candidate-flight-header.component.html',
  styleUrls: ['./candidate-flight-header.component.css']
})
export class CandidateFlightHeaderComponent implements OnInit {

  cFlights: ICandidateFlightGrp[] = [];
  pagination: Pagination | undefined;
  totalCount=0;

  fParams = new CandidateFlightParams();

  constructor(private service: DeployService, private router: Router,
    private confirm: ConfirmService,  private toastr: ToastrService) {}

  ngOnInit(): void {
    this.loadFlights();
  }

  
  loadFlights() {
    var params = this.service.getFlightParams();
    
    this.service.getCandidateFlightHeadersPagedList(params)?.subscribe({
    next: response => {
      if(response !== undefined && response !== null) {
          this.cFlights = response.result;
          this.totalCount = response?.count;
          this.pagination = response.pagination;
        } 
      },
      error: error => console.log(error)
    })
  }

  travelAdviseClicked(event: any) {
      var id = +event;
      this.service.composeTravelAdviseForClient(id).subscribe({
        next: (response: string) => {
          console.log('response', response);
          this.toastr.success(response, 'Success')
        },
        error: (err: any) => this.toastr.error(err.error.details, 'Error encountered')
      })
  }

  onPageChanged(event: any){
    const params = this.service.getFlightParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event.page;
      this.service.setFlightParams(params);

      this.loadFlights();
    }
  }

  deleteClicked(event: any) {
    var id=event;
    var confirmMsg = 'confirm delete this Candidate Flight?. ' + 
      'WARNING: this will also delete the flight transaction, and cannot be undone';

    const observableInner = this.service.deleteCandidateFlight(id);
    const observableOuter = this.confirm.confirm('confirm Delete', confirmMsg);

    observableOuter.pipe(
        filter((confirmed) => confirmed),
        switchMap((confirmed) => {
          return observableInner
        })
    ).subscribe(response => {
      if(response) {
        this.toastr.success('Flight data deleted for the chosen candidate', 'deletion successful');
        var index = this.cFlights.findIndex(x => x.id == id);
        if(index >= 0) this.cFlights.splice(index,1);
      } else {
        this.toastr.error('Error in deleting the candidate Flight data', 'failed to delete')
      }
      
    });

  }

  editClicked(event: any) {
    
  }

  onReset() {
    this.fParams = new CandidateFlightParams();
    this.service.setFlightParams(this.fParams);
    this.loadFlights();
  }

  applyParams() {
    this.service.setFlightParams(this.fParams);
    this.loadFlights();
  }

  close() {
      this.router.navigateByUrl("/");
  }



}
