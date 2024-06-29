import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'depStatus'
})
export class DepStatusPipe implements PipeTransform {

  statuses= [
    {sequence: 0, status: 'None'}, 
    {sequence: 100, status: 'Selected'}, 
    {sequence: 150, status: 'Offer Letter Accepted'},
    {sequence: 200, status: 'Document Certificate Initiated'},
    {sequence: 300, status: 'Ref For Med Tests'},
    {sequence: 400, status: 'Med Fit'}, 
    {sequence: 500, status: 'Med Unfit'},
    {sequence: 550, status: 'Visa Documents Prepared'},
    {sequence: 600, status: 'Visa Docs submitted'}, 
    {sequence: 700, status: 'Visa Endorsed'}, 
    {sequence: 800, status: 'Visa denied'},
    {sequence: 900, status: 'Emig Application Lodged'},
    {sequence: 1000, status: 'Emig Documents Lodged'},
    {sequence: 1100, status: 'Emig Granted'},
    {sequence: 1200, status: 'Emig Denied'},
    {sequence: 1300, status: 'Travel Tkt booked'},
    {sequence: 1400, status: 'Docs couriered to candidate'},
    {sequence: 1500, status: 'Traveled'},
    {sequence: 1600, status: 'Arrival Acknowledged by client'},
    
    {sequence: 5000, status: 'Concluded'}
  ]  

  transform(value: number): string {
    
    var dto = this.statuses.filter(x => x.sequence === value).map(x => x.status)[0];
    if(dto===null || dto===undefined) return 'undefined';
    return dto;

  }

}
