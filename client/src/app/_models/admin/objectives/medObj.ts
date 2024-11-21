export interface IMedObj 
{
    fromdate: Date;
    uptodate: Date;
    pageNumber: number;
    pageSize: number;
}

export class MedObj implements IMedObj
{
    fromdate = new Date();
    uptodate = new Date();
    pageNumber = 1;
    pageSize = 15;
}