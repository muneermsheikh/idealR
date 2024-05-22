export interface ICVRefWithDepDto
{
     checked: boolean;
     cVRefId: number;
     customerName: string;
     orderId: number;
     orderNo: number;
     orderDate: Date;
     orderItemId: number;
     categoryName: string;
     categoryRef: string;
     customerId: number;
     candidateId: number;
     applicationNo: number;
     candidateName: string;
     agentName: string;
     referredOn: Date;
     referralDecision: string;
     selectedOn: Date;
     deployments: IDeployDto[];
}

export class CVRefWithDepDto implements ICVRefWithDepDto
{
     checked = false;
     cVRefId= 0;
     customerName= '';
     orderId= 0;
     orderNo= 0;
     orderDate = new Date();
     orderItemId= 0;
     categoryName= '';
     categoryRef= '';
     customerId= 0;
     candidateId= 0;
     applicationNo= 0;
     candidateName= '';
     agentName= '';
     referredOn = new Date();
     referralDecision= '';
     selectedOn = new Date();
     deployments: IDeployDto[] = [];
}

export interface IDeployDto
{
     id: number;
     cVRefId: number;
     transactionDate: Date;
     sequence: number;
     nextSequence: number;
     nextStageDate: Date;
}