import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs";
import { PaginatedResult } from "../_models/pagination";
import { CVRefParams } from "../_models/params/Admin/cvRefParams";
import { assessmentQBankParams } from "../_models/admin/assessmentQBankParams";
import { interviewParams } from "../_models/params/Admin/interviewParams";
import { prospectiveCandidateParams } from "../_models/params/hr/prospectiveCandidateParams";
import { prospectiveSummaryParams } from "../_models/params/hr/prospectiveSummaryParams";
import { SelDecisionParams } from "../_models/params/Admin/selDecisionParams";
import { UserParams } from "../_models/params/userParams";
import { candidateParams } from "../_models/params/hr/candidateParams";
import { professionParams } from "../_models/params/masters/ProfessionParams";
import { employmentParams } from "../_models/params/Admin/employmentParam";
import { employeeParams } from "../_models/params/Admin/employeeParams";
import { IndustryParams } from "../_models/params/masters/industryParams";
import { orderParams } from "../_models/params/Admin/orderParams";
import { OpenOrderItemsParams } from "../_models/params/Admin/openOrderItemsParams";
import { contractReviewParams } from "../_models/params/Admin/contractReviewParams";
import { TaskParams } from "../_models/params/Admin/taskParams";
import { CVBriefParam } from "../_models/params/hr/cvBriefParam";
import { deployParams } from "../_models/params/process/deployParams";
import { ICallRecordParams } from "../_models/params/callRecordParams";
import { CallRecordItemToCreateDto } from "../_dtos/hr/callRecordItemToCreateDto";
import { FeedbackParams } from "../_models/params/hr/feedbackParams";
import { customerParams } from "../_models/params/Admin/customerParams";
import { HttpParamsWithStringDto } from "../_dtos/admin/HttpParamsWithStringDto";
import { CandidateFlightParams } from "../_models/params/process/CandidateFlightParams";

export function getPaginatedResult<T>(url: string, params: HttpParams, http: HttpClient) {
 
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>;
    return http.get<T>(url, { observe: 'response', params }).pipe(
      map((response:any) => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })
    );
  }

  export function getPaginationHeadersCVRefParams(oParams: CVRefParams) {
    let params = new HttpParams();

    params = params.append('pageNumber', oParams.pageNumber);
    params = params.append('pageSize', oParams.pageSize);

    if (oParams.agentId !== 0) params = params.append('agentId', oParams.agentId!.toString());
    //if (oParams.professionId !== 0) params = params.append('professionId', oParams.professionId!.toString());
    if (oParams.applicationNo !== 0) params = params.append('applicationNo', oParams.applicationNo!.toString());
    if (oParams.candidateId !== 0) params = params.append('candidateId', oParams.candidateId!.toString());
    if (oParams.orderId !== 0) params = params.append('orderId', oParams.orderId.toString());
    if (oParams.orderNo !== 0) params = params.append('orderNo', oParams.orderNo.toString());
    params = params.append('selectionStatus', oParams.selectionStatus);
    //if (oParams.orderItemId !== 0) params = params.append('orderItemId', oParams.orderItemId!.toString());
    
    //if (oParams.search) params = params.append('search', oParams.search);
    
    params = params.append('sort', oParams.sort);


    return params;
  }

  export function getPaginationHeadersAssessmentQBankParams(oParams: assessmentQBankParams){

      let params = new HttpParams();

      params = params.append('pageNumber', oParams.pageNumber);
      params = params.append('pageSize', oParams.pageSize);

      if(oParams.professionId !== 0) params = params.append('professionId', oParams.professionId.toString());
          
      if(oParams.professionName !== '') params = params.append('professionName', oParams.professionName);
      
      if (oParams.search) params = params.append('search', oParams.search);
      
      params = params.append('sort', oParams.sort);

      return params;
  }

  export function getPaginationHeadersInterviewParams(oParams: interviewParams){

    let params = new HttpParams();

    params = params.append('pageNumber', oParams.pageNumber);
    params = params.append('pageSize', oParams.pageSize);

    if (oParams.orderNo !== 0) params = params.append('orderNo', oParams.orderNo.toString());
    if (oParams.orderId !== 0) params = params.append('orderIdId', oParams.orderId!.toString());
    if (oParams.customerId !== 0) params = params.append('customerId', oParams.customerId!.toString());
    if (oParams.customerNameLike !== '') params = params.append('customerNameLike', oParams.customerNameLike);
    if (oParams.interviewVenue !== '') params = params.append('interviewVenue', oParams.interviewVenue);
    if (oParams.search) params = params.append('search', oParams.search);
    
    params = params.append('sort', oParams.sort);

    return params;

  }

  export function getHttpParamsForProspectiveCandidates(oParams: prospectiveCandidateParams) {

    let params = new HttpParams();

    params = params.append('pageNumber', oParams.pageNumber);
    params = params.append('pageSize', oParams.pageSize);

    if (oParams.status !== ''  && oParams.status !== undefined)  params = params.append('status', oParams.status);
    params = params.append('statusClass', oParams.statusClass);
    if (oParams.categoryRef !== ''  && oParams.categoryRef !== undefined)  params = params.append('categoryRef', oParams.categoryRef);

    if (oParams.dateAdded !=='' && oParams.dateAdded !== undefined ){
      params = params.append('dateAdded', oParams.dateAdded);
    }
  
    if (oParams.search) params = params.append('search', oParams.search);
    
    params = params.append('sort', oParams.sort)

    return params;
  }

  export function getHttpParamsForProspectiveSummary(oParams: prospectiveSummaryParams) {

    let params = new HttpParams();

    params = params.append('pageNumber', oParams.pageNumber);
    params = params.append('pageSize', oParams.pageSize);

    if (oParams.status !== ''  && oParams.status !== undefined)  params = params.append('status', oParams.status);
    if (oParams.categoryRef !== ''  && oParams.categoryRef !== undefined)  params = params.append('categoryRef', oParams.categoryRef);
    if (oParams.dateRegistered.getFullYear() < 2000 && oParams.dateRegistered !== undefined ){
      params = params.append('dateAdded', oParams.dateRegistered.toDateString());
    }
    
    if (oParams.search) params = params.append('search', oParams.search);
    
    params = params.append('sort', oParams.sort);

    return params;
  }

  export function getPaginationHeadersSelectionParams(sParams: SelDecisionParams): HttpParams {
    
    var dto = new HttpParamsWithStringDto();
    var st='';

    let params = new HttpParams();
    params = params.append('pageSize', sParams.pageSize)
    params = params.append('pageNumber', sParams.pageNumber );
    st = "Page No:" + sParams.pageNumber;
    st +=  ", Page Size:" + sParams.pageSize;

    if (sParams.orderItemId !== 0) {
      st += ", Order ItemId: " + sParams.orderItemId;
      params = params.append('orderItemId', sParams.orderItemId.toString());
    }

    if (sParams.orderId !== 0) {
      st += ", Order Id: " + sParams.orderId;
      params = params.append('orderId', sParams.orderId.toString());
    }

    if (sParams.professionId !== 0) 
      st += ", Order Item: " + sParams.professionName;      
      params = params.append('categoryId', sParams.professionId.toString());
    if (sParams.professionName !== '') 
      params = params.append('categoryName', sParams.professionName);
    if (sParams.orderId !== 0) {
      params = params.append('orderId', sParams.orderId!.toString());
      st += ", Order Id: " + sParams.orderId;
    }
    if (sParams.orderNo !== 0) {
      params = params.append('orderNo', sParams.orderNo!.toString());
      st += ", Order No: " + sParams.orderNo;
    }
    if (sParams.candidateId !== 0) 
      params = params.append('candidateId', sParams.candidateId!.toString());
    if (sParams.applicationNo !== 0) {
      params = params.append('applicationNo', sParams.applicationNo!.toString());
      st += ", Application No: " + sParams.applicationNo;
    }
    if (sParams.cVRefId !== 0) 
      params = params.append('cVRefId', sParams.cVRefId!.toString());
    if (sParams.includeEmployment === true) {
      params = params.append('includeEmployment', "true");
      st += ", Include Employment: " + sParams.includeEmployment;
    }
    if (sParams.search) {
      params = params.append('search', sParams.search);
      st += ", Search criteria: " + sParams.search;
    }
   
    params = params.append('sort', sParams.sort);
    
    dto.params=params;
    dto.stParams=st;
    return params;
  }

  export function getPaginationHeaderAssessmentQBankParams(sParams: assessmentQBankParams): HttpParams {

    let params = new HttpParams();

    //params = params.append('pageNumber', sParams.pageNumber);
    //params = params.append('pageSize', sParams.pageSize)
    
    if (sParams.professionId !== 0) 
      params = params.append('professionId', sParams.professionId.toString())

    return params;
  }


  export function getHttpParamsForUserParams(userParams: UserParams)
  {
    let params = new HttpParams();

    params = params.append('pageNumber', userParams.pageNumber);
    params = params.append('pageSize', userParams.pageSize);

    if(userParams.email !== '') params = params.append('email', userParams.email);
    if(userParams.knownAs !== '') params = params.append('email', userParams.knownAs);
    if(userParams.phoneNumber !== '') params = params.append('email', userParams.phoneNumber);
    
    if(userParams.gender !== '') params = params.append('gender', userParams.gender);
    if(userParams.orderBy !== '') params = params.append('orderBy', userParams.orderBy);
    
    return params;
  }

  export function getHttpParamsForCandidate(cvParams: candidateParams)
  {
    let params = new HttpParams();

    params = params.append('pageNumber', cvParams.pageNumber.toString());
    params = params.append('pageSize', cvParams.pageSize.toString());

    if(cvParams.agentId !== 0) params = params.append('agendId', cvParams.agentId.toString());
    if(cvParams.professionId !== 0) params = params.append('professionId', cvParams.professionId.toString());
    if(cvParams.search !== '') params = params.append('search', cvParams.search);
    if(cvParams.sort !== '') params = params.append('sort', cvParams.sort);
    if(cvParams.typeOfCandidate !== '') params = params.append('typeOfCandidate', cvParams.typeOfCandidate);
    return params;
  }

  export function getHttpParamsForCustomers(custParams: customerParams)
  {
      let params = new HttpParams();

      params = params.append('pageNumber', custParams.pageNumber);
      params = params.append('pageSize', custParams.pageSize)
    
      if (custParams.customerType !== "") params = params.append('customerType', custParams.customerType);
      if (custParams.customerCityName !== '') params = params.append('customerCityName', custParams.customerCityName!);
      if (custParams.customerIndustryId !== 0) params = params.append('customerIndustryId', custParams.customerIndustryId!.toString());
      if (custParams.search) params = params.append('search', custParams.search);
      
      params = params.append('sort', custParams.sort);
      params = params.append('pageIndex', custParams.pageNumber.toString());
      params = params.append('pageSize', custParams.pageSize.toString());
        
      params = params.append('customerType', custParams.customerType);
  
      params = params.append('sort', custParams.sort);

      return params;
  }

  export function getHttpParamsForProfession(mParams: professionParams)
  {

    let params = new HttpParams();

    params = params.append('pageNumber', mParams.pageNumber.toString());
    params = params.append('pageSize', mParams.pageSize.toString())

    if (mParams.professionName !== '') params = params.append('name', mParams.professionName);
    if (mParams.id !== 0) params = params.append('id', mParams.id!.toString());
    if (mParams.search) params = params.append('search', mParams.search);
    
    return params;

  }

  export function getHttpParamsForEmployment(oParams: employmentParams)
  {
      let params = new HttpParams();

      params = params.append('pageNumber', oParams.pageNumber);
      params = params.append('pageSize', oParams.pageSize)
    
      if (oParams.cvRefId !== 0) params = params.append('cvRefId', oParams.cvRefId!.toString());
      if (oParams.orderItemId !== 0) params = params.append('orderItemId', oParams.orderItemId!.toString());
      if (oParams.categoryId !== 0) params = params.append('categoryId', oParams.categoryId!.toString());
      if (oParams.orderId !== 0) params = params.append('orderId', oParams.orderId!.toString());
      if (oParams.orderNo !== 0) params = params.append('orderNo', oParams.orderNo!.toString());
      if (oParams.customerId !== 0) params = params.append('customerId', oParams.customerId!.toString());
      if(oParams.applicationNo !== 0) params = params.append('applicationNo', oParams.applicationNo!.toString());
      if (oParams.candidateName !== '') params = params.append('candidateName', oParams.candidateName);
      params = params.append('approved', oParams.approved );
      
      if (oParams.selectionDateFrom.getFullYear() > 2000) 
        params = params.append('selectionDateFrom', oParams.selectionDateFrom.toString());
      
      if (oParams.selectionDateUpto.getFullYear() > 2000) 
        params = params.append('selectionDateUpto', oParams.selectionDateUpto.toString());
      
      if (oParams.search) 
        params = params.append('search', oParams.search);
      
      params = params.append('sort', oParams.sort);

      return params;
  }

  export function getHttpParamsForEmployees(oParams: employeeParams) {

      let params = new HttpParams();

      params = params.append('pageNumber', oParams.pageNumber);
      params = params.append('pageSize', oParams.pageSize)

      //if(oParams.dOJ !== null) params = params.append('dOJ', oParams.dOJ);
      if(oParams.department !== '') params = params.append('department', oParams.department);
      if(oParams.position !== '') params = params.append('position', oParams.position);
      if(oParams.search !== '') params = params.append('search', oParams.search);
      if(oParams.sort !== '') params = params.append('sort', oParams.sort);

      return params;
  } 

  export function getHttpParamsForIndustries(mParams: IndustryParams) {

      let params = new HttpParams();

      params = params.append('pageNumber', mParams.pageNumber);
      params = params.append('pageSize', mParams.pageSize)

      if (mParams.industryName !== '') params = params.append('industryName', mParams.industryName);
      if (mParams.id !== 0) params = params.append('id', mParams.id!.toString());
      if (mParams.search) params = params.append('search', mParams.search);
      
      return params;
  }

  export function getHttpParamsForOrders(oParams: orderParams) {

    let params = new HttpParams();

    params = params.append('pageNumber', oParams.pageNumber);
    params = params.append('pageSize', oParams.pageSize)

    if (oParams.city !== "") params = params.append('cityOfWorking', oParams.city);
      
    if (oParams.search) params = params.append('search', oParams.search);
      
    params = params.append('sort', oParams.sort);

    return params;
      
  }

  export function getHttpParamsForOrderItems(oParams: OpenOrderItemsParams) {

    let params = new HttpParams();

    params = params.append('pageNumber', oParams.pageNumber);
    params = params.append('pageSize', oParams.pageSize)

    
    if (oParams.customerId !== 0) params = params.append('customerId', oParams.customerId.toString());
    if (oParams.orderId !== 0) params = params.append('orderId', oParams.orderId.toString());
    if (oParams.orderItemIds.length > 0) params = params.append('orderItemIds', oParams.orderItemIds.join(','));
    if (oParams.professionIds.length > 0) params = params.append('professionIdsIds', oParams.professionIds.join(','));
    
    return params;
  }

  export function getHttpParamsForContractReview(oParams: contractReviewParams) {

    let params = new HttpParams();

    params = params.append('pageNumber', oParams.pageNumber);
    params = params.append('pageSize', oParams.pageSize)

    if (oParams.city !== "") params = params.append('city', oParams.city);
    if (oParams.categoryId !== 0) params = params.append('categoryId', oParams.categoryId.toString());
    params = params.append('orderids', oParams.orderId);
    if (oParams.search) params = params.append('search', oParams.search);
    params = params.append('sort', oParams.sort);
    
    return params;
  }
  
  export function getHttpParamsForTask(oParams: TaskParams) {

    let params = new HttpParams();

    params = params.append('pageNumber', oParams.pageNumber);
    params = params.append('pageSize', oParams.pageSize)
    
    if (oParams.taskStatus !== '' ) 
        params = params.append('taskStatus', oParams.taskStatus); 
    if (oParams.orderId !== 0 ) 
        params = params.append('orderId', oParams.orderId.toString()); 
    if (oParams.assignedToUsername !== '' ) params = params.append('assignedToUsername', oParams.assignedToUsername); 
    if (oParams.assignedByUsername !== '' ) params = params.append('assignedByUsername', oParams.assignedByUsername); 
    if (new Date(oParams.taskDate).getFullYear() > 2000) params = params.append('taskDate', oParams.taskDate.toString()); 
    
    if(oParams.candidateId !==0) {
      params = params.append('candidateId', oParams.candidateId.toString());
    }
    if (oParams.search) params = params.append('search', oParams.search);
    
    params = params.append('sort', oParams.sort);
    
    return params;
  }

  export function GetHttpParamsForCVRefBrief(cvParams:CVBriefParam) {

    let params = new HttpParams();

    params = params.append('pageNumber', cvParams.pageNumber);
    params = params.append('pageSize', cvParams.pageSize)
      
    if(cvParams.candidateId !== 0) params = params.append('candidateId', cvParams.candidateId.toString());
    if(cvParams.assessmentId !== 0) params = params.append('assessmentId', cvParams.assessmentId.toString());
    if(cvParams.assessmentId !== 0) params = params.append('assessmentId', cvParams.assessmentId.toString());
    
    return params;
  }

  export function GetHttpParamsForCandidateFlightHdr(dParams: CandidateFlightParams) {
    let params = new HttpParams();

    params = params.append('pageNumber', dParams.pageNumber);
    params = params.append('pageSize', dParams.pageSize)
   
    //if(dParams.dateOfFlight.toString() !== '') 
     // params = params.append('dateOfFlight', dParams.dateOfFlight.toString());
    
    if(dParams.airineName !== '') params = params.append('airlineName', dParams.airineName);
    if(dParams.airportOfBoarding !== '') params = params.append('airportOfBoarding', dParams.airportOfBoarding);
    if(dParams.flightNo !== '') params = params.append('flightNo', dParams.flightNo);

    return params;
  }

  export function GetHttpParamsForDepProcess(dParams: deployParams) {

    let params = new HttpParams();

    params = params.append('pageNumber', dParams.pageNumber);
    params = params.append('pageSize', dParams.pageSize)
    
    if(dParams.cvRefId !== 0) {
       params = params.append('cvRefId', dParams.cvRefId)
    } else if(dParams.orderItemId !== 0 ) {
      params = params.append('orderItemId', dParams.orderItemId);
    } else if(dParams.candidateId !== 0) {
        params = params.append('candidateId', dParams.candidateId);
    } else if(dParams.customerId !== 0) {
      params = params.append("customerId", dParams.customerId.toString());
    } else if(dParams.orderNo !== 0) {
      params = params.append("orderNo", dParams.orderNo.toString());
    } else if (dParams.customerName !=='') {
      params = params.append("customerName", dParams.customerName);
    } else if (dParams.categoryName !== '') {
      params = params.append('categoryName', dParams.categoryName);
    } else if (dParams.applicationNo !== 0) {
      params = params.append('applicationNo', dParams.applicationNo.toString())
    } else if (dParams.candidateName !== '') {
      params = params.append('candidateName', dParams.candidateName);
    }

    if(dParams.status !== '') params=params.append('status', dParams.status);

    /*if(dParams.selectedOn.getFullYear() > 2000) 
        params = params.append('selectedOn', dParams.selectedOn.toString());
    */

    return params;
  }


  export function GetHttpParamsForCallRecord(hParams:ICallRecordParams) {
  
    let params = new HttpParams();

    if(hParams.personId !== '') params = params.append('personId', hParams.personId);
    params = params.append('personType', hParams.personType);
    if(hParams.emailId !== '') params = params.append('emailId', hParams.emailId);
    if(hParams.mobileNo !== '') params = params.append('mobileNo', hParams.mobileNo);
    if(hParams.categoryRef !== '') params = params.append('categoryRef', hParams.categoryRef);
    if(hParams.status !== '') params = params.append('status', hParams.status);
   
    params = params.append('pageIndex', hParams.pageNumber.toString());
    params = params.append('pageSize', hParams.pageSize.toString());

    return params;
  }  

  export function GetHttpParamsForCallItemCreate(hParams: CallRecordItemToCreateDto) {
    
      let params = new HttpParams();
  
      params = params.append('personId', hParams.personId);
      params = params.append('personName', hParams.personName);
      params = params.append('personType', hParams.personType);
      
      if(hParams.callRecordId !== 0) params = params.append('callRecordId', hParams.callRecordId);
      if(hParams.email !== '') params = params.append('email', hParams.email);
      if(hParams.phoneNo !== '') params = params.append('phoneNo', hParams.phoneNo);
      if(hParams.categoryRef !== '') params = params.append('categoryRef', hParams.categoryRef);
      if(hParams.status !== '') params = params.append('status', hParams.status);
     
      return params;
    
  }

  
  export function GetHttpParamsForFeedback(fParams:FeedbackParams) {
  
    var returnPrams = new HttpParamsWithStringDto();
    var st='';

    let params = new HttpParams();

    if(fParams.email !== '') {
      st = 'Email: ' + fParams.email;
      params = params.append('email', fParams.email);
    }

    if(fParams.phoneNo !== '') {
        st += st ==='' ? fParams.phoneNo : ', ' + fParams.phoneNo;
       params = params.append('phoneNo', fParams.phoneNo);
    }
    st += st ==='' ? fParams.pageNumber : ', ' + fParams.pageNumber;
    st +=  ', ' + fParams.pageSize;
    params = params.append('pageNumber', fParams.pageNumber.toString());
    params = params.append('pageSize', fParams.pageSize.toString());

    return params;
  }  