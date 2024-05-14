export interface ICVReferredDto
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
     referralDecision: number;
     selectedOn: Date;
     deployments: IDeployDto[];
}

export class CVReferredDto implements ICVReferredDto
{
     checked = false;
     cVRefId= 0;
     customerName= '';
     orderId= 0;
     orderNo= 0;
     orderDate = new Date('1900-01-01');
     orderItemId= 0;
     categoryName= '';
     categoryRef= '';
     customerId= 0;
     candidateId= 0;
     applicationNo= 0;
     candidateName= '';
     agentName= '';
     referredOn = new Date('1900-01-01');
     referralDecision= 0;
     selectedOn = new Date('1900-01-01');
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