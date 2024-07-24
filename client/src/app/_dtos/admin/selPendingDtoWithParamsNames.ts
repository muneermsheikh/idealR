/*export interface ISelPendingDto {
     id: number;
     checked: boolean;
     cvRefId: number;
     orderItemId: number;
     orderId: number;
     customerId: number;
     //orderNo: number;
     customerName: string;
     categoryRefAndName: string;
     applicationNo: number;
     candidateId: number;
     candidateName: string;
     referredOn: Date;
     selectionStatus: string;
     //refStatus: number;
     selectionStatusDate: Date;
     remarks: string;
}*/

import { ISelPendingDto } from "./selPendingDto";

export interface ISelPendingDtoWithParamsNamesDto {
     selPendingDto: ISelPendingDto;
     paramsNames: string;
}