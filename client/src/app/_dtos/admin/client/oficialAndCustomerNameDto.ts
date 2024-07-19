export interface IOfficialAndCustomerNameDto
{
    id: number;
    customerId: number;
    customerName: string;
    officialName: string;
    email: string;
    phoneNo: string;
    customerIsBlacklisted: boolean;
}