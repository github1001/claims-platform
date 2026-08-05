import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { Claim } from '../../core/models/claim.model';
import { ClaimsApiService } from '../../core/services/claims-api.service';
import { finalize } from 'rxjs';      


@Component({
  selector: 'app-track-claim',
  imports: [
    ReactiveFormsModule,
    CurrencyPipe,
    DatePipe
  ],
  templateUrl: './track-claim.html',
  styleUrl: './track-claim.scss'
})
export class TrackClaim implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly claimsApi = inject(ClaimsApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly changeDetector =
  inject(ChangeDetectorRef);

  loading = false;
  responding = false;
  errorMessage = '';
  successMessage = '';
  claim: Claim | null = null;

  readonly form = this.formBuilder.nonNullable.group({
    claimNumber: ['', Validators.required]
  });

  readonly responseForm =
    this.formBuilder.nonNullable.group({
      response: [
        '',
        [
          Validators.required,
          Validators.minLength(5)
        ]
      ]
    });

  ngOnInit(): void {
    const claimNumber =
      this.route.snapshot.queryParamMap.get('claimNumber');

    if (claimNumber) {
      this.form.controls.claimNumber.setValue(claimNumber);
      this.search();
    }
  }

  search(): void {
  if (this.form.invalid) {
    this.form.markAllAsTouched();
    return;
  }

  this.loading = true;
  this.errorMessage = '';
  this.successMessage = '';
  this.claim = null;

  const claimNumber =
    this.form.controls.claimNumber.value.trim();

  this.claimsApi
    .getClaimByNumber(claimNumber)
    .pipe(
      finalize(() => {
        this.loading = false;
        this.changeDetector.detectChanges();
      })
    )
    .subscribe({
      next: claim => {
        this.claim = claim;
        this.changeDetector.detectChanges();
      },
      error: error => {
        this.errorMessage =
          error.status === 404
            ? 'No claim was found for that reference.'
            : error?.error?.detail ??
              'The claim could not be loaded.';

        this.changeDetector.detectChanges();
      }
    });
}

  provideInformation(
  informationRequestId: string
    ): void {
      if (!this.claim || this.responseForm.invalid) {
        this.responseForm.markAllAsTouched();
        return;
      }

      this.responding = true;
      this.errorMessage = '';
      this.successMessage = '';

      const response =
        this.responseForm.controls.response.value.trim();

      this.claimsApi
        .provideInformation(
          this.claim.id,
          informationRequestId,
          {
            response
          }
        )
        .pipe(
          finalize(() => {
            this.responding = false;
            this.changeDetector.detectChanges();
          })
        )
        .subscribe({
          next: claim => {
            this.claim = claim;
            this.successMessage =
              'Additional information submitted successfully.';

            this.responseForm.reset({
              response: ''
            });

            this.changeDetector.detectChanges();
          },
          error: error => {
            this.errorMessage =
              error?.error?.detail ??
              'The information could not be submitted.';

            this.changeDetector.detectChanges();
          }
        });
    }
}