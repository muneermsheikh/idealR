
export interface IDeploymentPendingDto {
    depId: number;
    checked: boolean;
    cvRefId: number;
    referredOn: Date;
    selectedOn: Date;
    applicationNo: number;
    orderItemId: number;
    candidateName: string;
    customerName: string;
    cityOfWorking: string;
    orderNo: number;
    orderDate: Date;
    categoryName: string;
    currentStatus: string;

    ecnr: boolean;
    deploySequence: number;
    nextSequence: number;
    currentSeqDate: Date;
    nextStageDate: Date;

}

export class DeploymentPendingDto implements IDeploymentPendingDto {
    depId: number=0;
    checked: boolean=false;
    cvRefId: number=0;
    referredOn = new Date();
    selectedOn = new Date();
    orderItemId=0;
    applicationNo: number=0;
    candidateName: string='';
    ecnr=false;
    currentStatus = '';
    
    customerName: string='';
    cityOfWorking: string='';
    orderNo: number=0;
    orderDate: Date=new Date();
    categoryName: string='';
    
    selDecisionDate: Date=new Date();
    deploySequence: number=0;
    currentSeqDate = new Date('1900-01-01');
    nextSequence: number=0;
    nextStageDate: Date=new Date();

}