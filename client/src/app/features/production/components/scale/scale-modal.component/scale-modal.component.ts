import { Component, inject, OnInit, ChangeDetectionStrategy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Scale, CreateScaleDto, UpdateScaleDto } from '../../../models/scale.interface';
import { ScaleService } from '../../../services/scale.service';
import { ErrorService } from '../../../../../core/services/error.service';

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
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './scale-modal.component.html',
  styleUrls: ['./scale-modal.component.scss']
})
export class ScaleModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly scaleService = inject(ScaleService);
  private readonly dialogRef = inject(MatDialogRef<ScaleModalComponent>);
  private readonly errorService = inject(ErrorService);
  private readonly data = inject<ScaleModalData>(MAT_DIALOG_DATA);

  protected readonly scaleForm = signal<FormGroup>(this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]]
  }));
  
  protected readonly isEditMode = signal(this.data.isEditMode);
  protected readonly scaleId = signal<number | undefined>(undefined);

  ngOnInit(): void {
    if (this.isEditMode() && this.data.scale) {
      this.scaleId.set(this.data.scale.id);
      this.scaleForm().patchValue({
        name: this.data.scale.name
      });
    }
  }

  protected onSubmit(): void {
    if (!this.scaleForm().valid) return;

    const formData = this.scaleForm().value;

    if (this.isEditMode() && this.scaleId()) {
      const updateData: UpdateScaleDto = { name: formData.name };
      this.scaleService.update(this.scaleId()!, updateData).subscribe({
        next: () => {
          this.errorService.showSuccess('Escala atualizada com sucesso');
          this.dialogRef.close(true);
        },
        error: (error) => this.errorService.showError('Erro ao atualizar escala', error)
      });
    } else {
      const createData: CreateScaleDto = { name: formData.name };
      this.scaleService.create(createData).subscribe({
        next: () => {
          this.errorService.showSuccess('Escala criada com sucesso');
          this.dialogRef.close(true);
        },
        error: (error) => this.errorService.showError('Erro ao criar escala', error)
      });
    }
  }

  protected onCancel(): void {
    this.dialogRef.close();
  }
}