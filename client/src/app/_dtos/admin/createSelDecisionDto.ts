import { CreateSelDecision } from "../../_models/admin/createSelDecision";


export class createSelDecisionDto
{
	cVRefId: number=0;
	decisionDate: Date=new Date();
	selectionStatus: string='';
	rejectionReason: string = '';
	remarks: string='';
}

export class selDecisionsToAddParams
{
	selDecisionsToAddDto: CreateSelDecision[]=[];
	advisesToClients: boolean=false;
	selectionEmailToCandidates: boolean = false;
	rejectionEmaiLToCandidates: boolean=false;
	rejectionSMSToCandidates: boolean = false;
	selectionSMSToCandidates: boolean = false;
  
}