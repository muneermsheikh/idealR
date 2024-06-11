export interface IOfferConclusioDto
{
    employmentId: number;
    acceptedString: string;
    conclusionDate: Date;
}

export class OfferConclusioDto implements IOfferConclusioDto
{
    employmentId = 0;
    acceptedString = '';
    conclusionDate= new Date();
}