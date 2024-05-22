import { ICVRefDeployDto } from "./cvRefDeployDto";
import { IDeployDto } from "./cvReferredDto";

export interface ICVRefDto
{
     id: number;

     checked: boolean;
     customerId: number;
     customerName: string;
     candidateId: number;
     orderId: number;
     orderNo: number;
     orderDate: Date;
     orderItemId: number;
     srNo: number;
     refStatus: string;
     selectionStatus: string;

     professionName: string;
     cvRefId: number;
     categoryRef: string;
     applicationNo: number;
     candidateName: string;
     referredOn: Date;
     selectedOn: Date;
     
}

export class CVRefDto implements ICVRefDto
{
     id = 0;
     checked = false;
     customerId = 0;
     customerName = '';
     candidateId = 0;
     orderId = 0;
     orderNo = 0;
     orderDate = new Date();
     orderItemId = 0;
     srNo = 0;
     refStatus = '';
     selectionStatus = '';
     professionName = '';
     
     cvRefId = 0;
     categoryRef = '';
     applicationNo = 0;
     candidateName = '';
     referredOn = new Date();
     selectedOn = new Date();

}

