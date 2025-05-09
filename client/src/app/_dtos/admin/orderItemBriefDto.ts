
export interface IOrderItemBriefDto
{
     srNo: number;
     checked: boolean;
     orderNo: number;
     orderDate: Date;
     customerName: string;
     orderItemId: number;
     requireInternalReview: string;
     categoryId: number;
     categoryRef: string;
     categoryName: string;
     categoryRefAndName: string;
     quantity: number;
     status: string;
     assessmentQDesigned: boolean;
     salaryMin: number;
     salaryMax: number;
}

export class OrderItemBriefDto implements IOrderItemBriefDto
{
     srNo = 0;
     checked: boolean = false;
     orderNo = 0;
     orderDate= new Date();
     customerName='';
     orderItemId=0;
     requireInternalReview = '';
     categoryId = 0;
     categoryRef='';
     categoryName = '';
     categoryRefAndName='';
     quantity=0;
     status='';
     assessmentQDesigned = false;
     salaryMax = 0;
     salaryMin = 0;
}