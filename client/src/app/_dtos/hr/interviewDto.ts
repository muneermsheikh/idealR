import { IInterviewItemDto } from "src/app/_models/hr/interviewItemDto";

export class interviewDto
{
    id = 0;
    orderId = 0;
    orderNo = 0;
    orderDate = new Date;
    customerName = '';
    interviewMode = '';
    interviewerName = '';
    interviewVenue = ''
    interviewDateFrom = new Date();
    interviewDateUpto = new Date();
    interviewLeaderName = '';
    customerRepresentativeName = '';
    interviewStatus = '';
    interviewItems: IInterviewItemDto[]=[];
}