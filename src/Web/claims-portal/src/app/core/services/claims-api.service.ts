import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  ApproveClaimRequest,
  Claim,
  ProvideInformationRequest,
  RejectClaimRequest,
  RequestInformationRequest,
  SettleClaimRequest,
  StartReviewRequest,
  SubmitClaimRequest,
  SubmitClaimResponse
} from '../models/claim.model';

@Injectable({
  providedIn: 'root'
})
export class ClaimsApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.claimsApiUrl}/claims`;

  submitClaim(request: SubmitClaimRequest): Observable<SubmitClaimResponse> {
    return this.http.post<SubmitClaimResponse>(
      this.apiUrl,
      request
    );
  }

  getClaimById(claimId: string): Observable<Claim> {
    return this.http.get<Claim>(
      `${this.apiUrl}/${claimId}`
    );
  }

  getClaimByNumber(claimNumber: string): Observable<Claim> {
    return this.http.get<Claim>(
      `${this.apiUrl}/reference/${encodeURIComponent(claimNumber)}`
    );
  }

  startReview(
    claimId: string,
    request: StartReviewRequest
  ): Observable<Claim> {
    return this.http.post<Claim>(
      `${this.apiUrl}/${claimId}/start-review`,
      request
    );
  }

  requestInformation(
    claimId: string,
    request: RequestInformationRequest
  ): Observable<Claim> {
    return this.http.post<Claim>(
      `${this.apiUrl}/${claimId}/request-information`,
      request
    );
  }

  provideInformation(
    claimId: string,
    informationRequestId: string,
    request: ProvideInformationRequest
  ): Observable<Claim> {
    return this.http.post<Claim>(
      `${this.apiUrl}/${claimId}/information-requests/${informationRequestId}/response`,
      request
    );
  }

  approveClaim(
    claimId: string,
    request: ApproveClaimRequest
  ): Observable<Claim> {
    return this.http.post<Claim>(
      `${this.apiUrl}/${claimId}/approve`,
      request
    );
  }

  rejectClaim(
    claimId: string,
    request: RejectClaimRequest
  ): Observable<Claim> {
    return this.http.post<Claim>(
      `${this.apiUrl}/${claimId}/reject`,
      request
    );
  }

  settleClaim(
    claimId: string,
    request: SettleClaimRequest
  ): Observable<Claim> {
    return this.http.post<Claim>(
      `${this.apiUrl}/${claimId}/settle`,
      request
    );
  }
}