import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { IOfficialAndCustomerNameDto } from 'src/app/_dtos/admin/client/oficialAndCustomerNameDto';
import { CustomersService } from 'src/app/_services/admin/customers.service';

export const OfficialIdAndCustomerNamesResolver: ResolveFn<IOfficialAndCustomerNameDto[]|null> = (
) => {
  return inject(CustomersService).getOfficialAndCustomerName();
};
