export class SelDecisionParams
{
     id: number=0;
     orderItemId: number=0;
     candidateId: number=0;
     orderId: number=0;
     orderNo: number=0;
     applicationNo: number=0;
     customerName: string='';
     professionId: number=0;
     professionName: string='';
     referredOn:Date = new Date();
     cVRefId: number=0;
     includeSelection: boolean=false;
     includeEmployment: boolean=false;
     includeDeployment: boolean=false;
     
     pageNumber = 1;
     pageSize=15;
     sort = "";
     search: string='';
}