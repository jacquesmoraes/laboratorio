import { Component, inject, OnInit, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormControl } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Scale, CreateScaleDto, UpdateScaleDto } from '../../../models/scale.interface';
import { ScaleService } from '../../../services/scale.service';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';

export interface ScaleModalData {
  scale?: Scale;
  isEditMode: boolean;
}

@Component({
  selector: 'app-scale-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatIconModule,
    MatFormFieldModule,
    MatDialogModule,
    MatInput
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './scale-modal.component.html',
  styleUrls: ['./scale-modal.component.scss']
})
export class ScaleModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly scaleService = inject(ScaleService);
  private readonly dialogRef = inject(MatDialogRef<ScaleModalComponent>);
 
  private readonly data = inject<ScaleModalData>(MAT_DIALOG_DATA);
  private readonly destroyRef = inject(DestroyRef);

  scaleForm!: FormGroup<{ name: FormControl<string> }>;

  isEditMode = this.data.isEditMode;
  scaleId?: number;

  ngOnInit(): void {
    this.scaleForm = this.fb.group({
      name: this.fb.control('', {
        validators: [Validators.required, Validators.minLength(2)],
        nonNullable: true
      })
    });

    if (this.isEditMode && this.data.scale) {
      this.scaleId = this.data.scale.id;
      this.scaleForm.patchValue({ name: this.data.scale.name });
    }
  }

  onSubmit(): void {
    if (this.scaleForm.invalid) return;

    const { name } = this.scaleForm.getRawValue();
    const request$ = this.isEditMode && this.scaleId
      ? this.scaleService.update(this.scaleId, { name })
      : this.scaleService.create({ name });

    request$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: () => {
          
        }
      });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}