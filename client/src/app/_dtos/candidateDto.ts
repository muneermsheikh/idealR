export interface ICandidateDto {
    candidateId: number;
    candidateName: string;
    applicationNo: number;
    coreProfession: string;
    amountDue: number;
    candidateHistoriesDto: ICandidateHistoryDto[];
}

export interface ICandidateHistoryDto {
    orderItemId: number;
    categoryRef: string;
    customerName: string;
    referredOn: Date;
    selectionStatus: string;
    selectionStatusDate: Date;
    deploymentStatus: string;
    deploymentStatusDate: Date;
}