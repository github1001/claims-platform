import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { finalize } from 'rxjs';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Observable } from 'rxjs';
import { Claim } from '../../core/models/claim.model';
import { ClaimsApiService } from '../../core/services/claims-api.service';

@Component({
  selector: 'app-officer-workspace',
  imports: [
    ReactiveFormsModule,
    CurrencyPipe,
    DatePipe
  ],
  templateUrl: './officer-workspace.html',
  styleUrl: './officer-workspace.scss'
})
export class OfficerWorkspace {
  private readonly formBuilder = inject(FormBuilder);
  private readonly claimsApi = inject(ClaimsApiService);
  private readonly changeDetector =
  inject(ChangeDetectorRef);

  private readonly officerId =
    '11111111-1111-1111-1111-111111111111';

  claim: Claim | null = null;
  loading = false;
  actionInProgress = false;
  errorMessage = '';
  successMessage = '';

  readonly searchForm = this.formBuilder.nonNullable.group({
    claimNumber: ['', Validators.required]
  });

  readonly informationForm =
    this.formBuilder.nonNullable.group({
      question: [
        '',
        [
          Validators.required,
          Validators.minLength(5)
        ]
      ]
    });

  readonly approveForm =
    this.formBuilder.nonNullable.group({
      settlementAmount: [
        0,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],
      reason: [
        '',
        [
          Validators.required,
          Validators.minLength(5)
        ]
      ]
    });

  readonly rejectForm =
    this.formBuilder.nonNullable.group({
      reason: [
        '',
        [
          Validators.required,
          Validators.minLength(5)
        ]
      ]
    });

  search(): void {
    if (this.searchForm.invalid) {
      this.searchForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.clearMessages();
    this.claim = null;

    const claimNumber =
      this.searchForm.controls.claimNumber.value.trim();

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

  startReview(): void {
    if (!this.claim) {
      return;
    }

    this.runAction(
      this.claimsApi.startReview(
        this.claim.id,
        {
          officerId: this.officerId
        }
      ),
      'Claim review started.'
    );
  }

  requestInformation(): void {
    if (!this.claim || this.informationForm.invalid) {
      this.informationForm.markAllAsTouched();
      return;
    }

    const question =
      this.informationForm.controls.question.value.trim();

      this.runAction(
        this.claimsApi.requestInformation(
          this.claim.id,
          {
            officerId: this.officerId,
            question
          }
        ),
        'Additional information requested.',
        () => this.informationForm.reset({
          question: ''
        })
      );
  }

  approve(): void {
    if (!this.claim || this.approveForm.invalid) {
      this.approveForm.markAllAsTouched();
      return;
    }

    const value = this.approveForm.getRawValue();

    this.runAction(
        this.claimsApi.approveClaim(
          this.claim.id,
          {
            officerId: this.officerId,
            settlementAmount: value.settlementAmount,
            reason: value.reason.trim()
          }
        ),
      'Claim approved.',
      () => this.approveForm.reset({
        settlementAmount: 0,
        reason: ''
      })
    );
  }

  reject(): void {
    if (!this.claim || this.rejectForm.invalid) {
      this.rejectForm.markAllAsTouched();
      return;
    }

    const reason =
      this.rejectForm.controls.reason.value.trim();

    this.runAction(
      this.claimsApi.rejectClaim(
        this.claim.id,
        {
          officerId: this.officerId,
          reason
        }
      ),
      'Claim rejected.',
      () => this.rejectForm.reset({
        reason: ''
      })
    );
  }

  settle(): void {
    if (!this.claim) {
      return;
    }

    this.runAction(
      this.claimsApi.settleClaim(
        this.claim.id,
        {
          officerId: this.officerId
        }
      ),
      'Claim settled.'
    );
  }

  private runAction(
    request: Observable<Claim>,
    successMessage: string,
    onSuccess?: () => void
  ): void {
    this.actionInProgress = true;
    this.clearMessages();

    request
      .pipe(
        finalize(() => {
          this.actionInProgress = false;
          this.changeDetector.detectChanges();
        })
      )
      .subscribe({
        next: claim => {
          this.claim = claim;
          this.successMessage = successMessage;
          onSuccess?.();

          this.changeDetector.detectChanges();
        },
        error: error => {
          this.errorMessage =
            error?.error?.detail ??
            'The requested action could not be completed.';

          this.changeDetector.detectChanges();
        }
      });
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}