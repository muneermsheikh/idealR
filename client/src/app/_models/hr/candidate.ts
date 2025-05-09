import { IPhoto } from "../admin/photo";
import { IUserPhone } from "../params/Admin/userPhone";
import { IUserProfession } from "../params/Admin/userProfession";
import { IUserQualification } from "../params/Admin/userQualification";
import { IUserAttachment } from "./userAttachment";
import { IUserExp } from "./userExp";


export interface ICandidate {
     //checked: boolean;
     id: number;
     applicationNo: number;
     gender: string;
     firstName: string;
     secondName: string;
     familyName: string;
     knownAs: string;
     customerId?: number;
     referredByName: string;
     dOB?: Date;
     placeOfBirth: string;
     aadharNo: string;
     ppNo: string;
     ecnr: string;
     address: string;
     city: string;
     pin: string;
     district: string;
     country: string;
     nationality: string;
     email: string;
     
     created: Date;
     appUserIdNotEnforced: string;
     appUserId: number;
     notificationDesired: boolean;
     candidateStatus?: string;     //EnumCnadidateStatus.NotReferred;
     photoUrl?: string;
     lastActive?: Date;
     userPhones: IUserPhone[];
     userQualifications: IUserQualification[];
     userProfessions: IUserProfession[];
     userAttachments: IUserAttachment[];
     userExperiences: IUserExp[];
     userPhotos: IPhoto[];
     //userFormFiles: File[];
}

export class Candidate implements ICandidate {
     //checked: boolean;
     id= 0;
     applicationNo= 0;
     gender= '';
     firstName= '';
     secondName= '';
     familyName= '';
     knownAs= '';
     referredBy?= 0;
     referredByName= '';
     dOB?: Date;
     placeOfBirth= '';
     aadharNo= '';
     ppNo= '';
     ecnr='F';
     address= '';
     city= '';
     pin= '';
     district= '';
     country='India';
     nationality= '';
     email= '';
     companyId= 0;
     created= new Date;
     appUserIdNotEnforced= '';
     appUserId= 0;
     notificationDesired= false;
     candidateStatus?= '';     //EnumCnadidateStatus.NotReferred;
     photoUrl?= '';
     lastActive?= new Date;
     userPhones: IUserPhone[]=[];
     userQualifications: IUserQualification[]=[];
     userProfessions: IUserProfession[]=[];
     userAttachments: IUserAttachment[]=[];
     userExperiences: IUserExp[]=[];
     userPhotos: IPhoto[]=[];
     //userFormFiles: File[];
}