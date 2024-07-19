import { IFeedback, IFeedbackInput } from "src/app/_models/hr/feedback";

export interface IFeedbackAndHistoryDto
{
    feedback: IFeedback;
    feedbackHistories: IFeedbackHistoryDto[];
}

export interface IFeedbackHistoryDto
{
    feedbackId: number;
    feedbackIssueDate: Date;
    //feedbackRecdDate?: Date;
}