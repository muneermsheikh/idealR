import { Injectable, inject } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, ResolveFn } from "@angular/router";
import { Observable, of } from "rxjs";
import { IEmployee } from "../shared/models/admin/employee";
import { MastersService } from "../shared/services/masters.service";

@Injectable({
     providedIn: 'root'
 })
 export class EmployeeFromIdResolver implements Resolve<IEmployee> {
 
     constructor(private empService: EmployeeService) {}
 
     resolve(route: ActivatedRouteSnapshot): Observable<IEmployee> {
        var id = route.paramMap.get('id');
        if (id==='') return null;

        return  this.empService.getEmployee(+id);
        
     }
 
 }

 export const EmployeeFromIdResolver: ResolveFn<IEmployee|null> = (
    route: ActivatedRouteSnapshot,
  ) => {
    var id = route.paramMap.get('id');
    if (id===null) return of(null);
    return inject(Emplo).getEmployee(+id!);
  };