import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { IClientIdAndNameDto } from 'src/app/_dtos/admin/clientIdAndNameDto';
import { CustomersService } from 'src/app/_services/admin/customers.service';

export const AgentsResolver: ResolveFn<IClientIdAndNameDto[]|null> = (
) => {
  return inject(CustomersService).getAgentIdAndNames();
};
