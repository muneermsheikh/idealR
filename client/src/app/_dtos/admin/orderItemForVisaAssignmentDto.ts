export interface IOrderItemForVisaAssignmentDto
{
    checked: boolean;
    orderItemId: number;
    customerInBrief: string;
    category: string;
    quantity: number;
    assigned: number;
    unassigned: number;
    quantityAssigned: number;
}