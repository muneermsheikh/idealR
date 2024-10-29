export interface IProspectiveCandidate
{
     id: number;
     checked: boolean;
     source: string;
     personType: string;
     personId: string;
     prospectiveCandidateId: number;
     nationality: string;
     address: string;
     city: string;
     date: Date;
     candidateName: string;
     categoryRef: string;
     orderItemId: number;
     age: string;
     phoneNo: string;
     alternatePhoneNo: string;
     email: string;
     alternateEmail: string;
     currentLocation: string;
     education: string;
     ctc: string;
     workExperience: number;
     status: string;
     newStatus: string;
     statusDate: Date;
     userName: string;
     closed: boolean;
     remarks: string;
}