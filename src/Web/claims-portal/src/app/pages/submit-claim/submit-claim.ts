import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { RouterLink } from '@angular/router';

import { ClaimsApiService } from '../../core/services/claims-api.service';
import { SubmitClaimResponse } from '../../core/models/claim.model';

@Component({
  selector: 'app-submit-claim',
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './submit-claim.html',
  styleUrl: './submit-claim.scss'
})
export class SubmitClaim {
  private readonly formBuilder = inject(FormBuilder);
  private readonly claimsApi = inject(ClaimsApiService);

  submitting = false;
  errorMessage = '';
  submittedClaim: SubmitClaimResponse | null = null;

  readonly form = this.formBuilder.nonNullable.group({
    claimType: ['Motor', Validators.required],
    claimantName: ['', Validators.required],
    claimantEmail: [
      '',
      [
        Validators.required,
        Validators.email
      ]
    ],
    incidentDate: ['', Validators.required],
    incidentDescription: [
      '',
      [
        Validators.required,
        Validators.minLength(10)
      ]
    ],
    estimatedLoss: [
      0,
      [
        Validators.required,
        Validators.min(1)
      ]
    ]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    this.submittedClaim = null;

    const value = this.form.getRawValue();

    this.claimsApi.submitClaim({
      claimType: value.claimType as 'Motor' | 'Property',
      claimantName: value.claimantName,
      claimantEmail: value.claimantEmail,
      incidentDate: value.incidentDate,
      incidentDescription: value.incidentDescription,
      estimatedLoss: value.estimatedLoss
    }).subscribe({
      next: response => {
        this.submittedClaim = response;
        this.submitting = false;
        this.form.reset({
          claimType: 'Motor',
          claimantName: '',
          claimantEmail: '',
          incidentDate: '',
          incidentDescription: '',
          estimatedLoss: 0
        });
      },
      error: error => {
        this.errorMessage =
          error?.error?.detail ??
          'The claim could not be submitted.';

        this.submitting = false;
      }
    });
  }
}