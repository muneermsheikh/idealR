import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn } from "@angular/router";
import { of } from "rxjs";
import { IStatementofAccountDto } from "src/app/_dtos/finance/statementOfAccountDto";
import { VouchersService } from "src/app/_services/finance/vouchers.service";

export const StatementOfAccountResolver: ResolveFn<IStatementofAccountDto | null | undefined> = (
    route: ActivatedRouteSnapshot
  ) => {

        var id = route.paramMap.get('id');
        if(id==='') return of(undefined);
        
        var fromdate = route.paramMap.get('fromDate');
        var uptodate = route.paramMap.get('uptoDate');

        if(fromdate==='' || uptodate==='') return of(undefined);
        
        return inject(VouchersService).getStatementOfAccount(+id!, fromdate!, uptodate!);

  };
