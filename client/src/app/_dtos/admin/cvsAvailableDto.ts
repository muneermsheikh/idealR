import { IUserProfession } from "src/app/_models/params/Admin/userProfession";

export interface ICvsAvailableDto {
     candAssessmentId: number;
     checked: boolean;
     candidateId: number;
     mobileNo: string;
     gender: string;
     applicationNo: number;
     fullName: string;
     city: string;
     orderCategoryRef: string;
     orderItemId: number;
     gradeAssessed: string;
     assessedOn: Date;
     userProfessions: IUserProfession[];
}

export class CvsAvailableDto implements ICvsAvailableDto {
     candAssessmentId = 0;
     checked=false;
     candidateId = 0;
     mobileNo = '';
     gender='male';
     applicationNo = 0;
     fullName = '';
     city= '';
     orderCategoryRef = '';
     orderItemId=0;
     gradeAssessed='';
     assessedOn=new Date();
     userProfessions: IUserProfession[]=[];
}