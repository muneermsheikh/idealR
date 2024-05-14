import { inject } from "@angular/core";
import { ResolveFn } from "@angular/router";
import { ICustomerOfficialDto } from "src/app/_models/admin/customerOfficialDto";
import { ClientService } from "src/app/_services/admin/client.service";


export const CustomerOfficialsResolver: ResolveFn<ICustomerOfficialDto[]|null> = (
  ) => {
    return inject(ClientService).getAgents();
  };