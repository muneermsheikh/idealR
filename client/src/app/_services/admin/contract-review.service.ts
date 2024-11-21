import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { IContractReview } from 'src/app/_models/admin/contractReview';
import { IReviewItemStatus } from 'src/app/_models/admin/reviewItemStatus';
import { contractReviewParams } from 'src/app/_models/params/Admin/contractReviewParams';
import { Pagination } from 'src/app/_models/pagination';
import { IContractReviewItem } from 'src/app/_models/admin/contractReviewItem';
import { getHttpParamsForContractReview, getPaginatedResult} from '../paginationHelper';
import { IContractReviewItemStddQ } from 'src/app/_models/admin/contractReviewItemStddQ';
import { IContractReviewDto } from 'src/app/_dtos/orders/contractReviewDto';
import { IContractReviewItemDto } from 'src/app/_dtos/orders/contractReviewItemDto';

@Injectable({
  providedIn: 'root'
})
export class ContractReviewService {

  apiUrl = environment.apiUrl;
  review?: IContractReview;
  reviewItemStatuses: IReviewItemStatus[]=[];

  cache = new Map();
  cacheS = new Map();   //for item status values

  oParams = new contractReviewParams();
  pagination: Pagination | undefined;

  
  constructor(private http: HttpClient) { }

  getOrGenerateContractReview(orderid: number) {
    return this.http.get<IContractReview>(this.apiUrl + 'ContractReview/getOrGenerate/' + orderid);
  }

  getContractReviewItem(orderitemid: number) {
    return this.http.get<IContractReviewItemDto>(this.apiUrl + 'ContractReview/reviewitem/' + orderitemid);
  }

  getContractReviewItems(orderid: number) {
    return this.http.get<IContractReviewItem[]>(this.apiUrl + 'contractreview/reviewitems/' + orderid);
  }

  InsertContractReviewItemFromOrderItemId(orderitemid: number) {
    return this.http.get<IContractReviewItemDto>(this.apiUrl + 'Orders/insertreviewitem/' + orderitemid);
  }
  
  updateContractReviewItem(model: IContractReviewItem) 
  {
    if(model.id !==0) {
      return this.http.put<IContractReviewItem>(this.apiUrl + 'ContractReview/reviewitem', model);
    } else {
      return this.http.post<IContractReviewItem>(this.apiUrl + 'ContractReview/reviewitem', model);
    }
  }

  getContractReviews(oParams: contractReviewParams) {

      const response = this.cache.get(Object.values(oParams).join('-'));
      if(response) return of(response);
  
      let params = getHttpParamsForContractReview(oParams);
    
      return getPaginatedResult<IContractReviewDto[]>(this.apiUrl + 
          'ContractReview/contractreviewspaged', params, this.http).pipe(
        map(response => {
          this.cache.set(Object.values(oParams).join('-'), response);
          return response;
        })
      )
    
  }
  
  getContractReviewStddQsData() {
    return this.http.get<IContractReviewItemStddQ[]>(this.apiUrl + 'contractreview/reviewstddq');
  }
  
  generateContractReviewObject(orderid: number) {
    return this.http.get<IContractReview>(this.apiUrl + 'ContractReview/generate/' + orderid);
  }

  saveContractReview(model: IContractReview) {
    return this.http.post<IContractReview>(this.apiUrl + 'ContractReview/contractreview', model);
    }
  
  updateContractReview(model: IContractReview) {
    return this.http.put<string>(this.apiUrl + 'ContractReview/contractreview', model)
  }

}
