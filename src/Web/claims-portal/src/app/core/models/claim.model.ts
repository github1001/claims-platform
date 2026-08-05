export type ClaimType = 'Motor' | 'Property';

export type ClaimStatus =
  | 'Submitted'
  | 'UnderReview'
  | 'AdditionalInformationRequired'
  | 'Approved'
  | 'Rejected'
  | 'Settled';

export interface InformationRequest {
  id: string;
  question: string;
  response: string | null;
  requestedByOfficerId: string;
  requestedAt: string;
  respondedAt: string | null;
  status: 'Pending' | 'Responded' | 'Cancelled';
}

export interface Claim {
  id: string;
  claimNumber: string;
  claimType: ClaimType;
  claimantName: string;
  claimantEmail: string;
  incidentDate: string;
  incidentDescription: string;
  estimatedLoss: number;
  approvedSettlementAmount: number | null;
  status: ClaimStatus;
  assignedOfficerId: string | null;
  decisionReason: string | null;
  createdAt: string;
  updatedAt: string;
  closedAt: string | null;
  version: number;
  informationRequests: InformationRequest[];
}

export interface SubmitClaimRequest {
  claimType: ClaimType;
  claimantName: string;
  claimantEmail: string;
  incidentDate: string;
  incidentDescription: string;
  estimatedLoss: number;
}

export interface SubmitClaimResponse {
  id: string;
  claimNumber: string;
  status: ClaimStatus;
  submittedAt: string;
}

export interface StartReviewRequest {
  officerId: string;
}

export interface RequestInformationRequest {
  officerId: string;
  question: string;
}

export interface ProvideInformationRequest {
  response: string;
}

export interface ApproveClaimRequest {
  officerId: string;
  settlementAmount: number;
  reason: string;
}

export interface RejectClaimRequest {
  officerId: string;
  reason: string;
}

export interface SettleClaimRequest {
  officerId: string;
}