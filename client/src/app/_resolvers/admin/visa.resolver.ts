import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { of } from 'rxjs';
import { IVisaHeader, VisaHeader } from 'src/app/_models/admin/visaHeader';
import { visaParams } from 'src/app/_models/params/Admin/visaParams';
import { VisaService } from 'src/app/_services/admin/visa.service';

export const visaResolver: ResolveFn<IVisaHeader | null> = (route: ActivatedRouteSnapshot) => {

  var id=route.paramMap.get('visaid');
  console.log('id', id);
  if(id === null || id === '') return of(null);
  return inject(VisaService).getVisaHeader(+id!)
};
