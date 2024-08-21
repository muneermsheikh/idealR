
export interface IIntervwCandAttachment {
     id: number;
     candidateId: number;
     attachmentSizeInBytes: number;
     url: string;
     fileName: string;
     dateUploaded: Date;
}

export class IntervwCandAttachment implements IIntervwCandAttachment {
     id=0;
     candidateId=0;
     attachmentSizeInBytes = 0;
     fileName='';
     url = '';
     dateUploaded = new Date();
     
}