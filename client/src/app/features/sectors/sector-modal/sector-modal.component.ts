import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Sector } from '../models/sector.interface';
import { SectorService } from '../service/sector.service';
import { ErrorService } from '../../../core/services/error.service';

export interface SectorModalData {
  sector?: Sector;
  isEditMode: boolean;
}

@Component({
  selector: 'app-sector-modal',
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
  templateUrl: './sector-modal.component.html',
  styleUrls: ['./sector-modal.component.scss']
})
export class SectorModalComponent implements OnInit {
  private fb = inject(FormBuilder);
  private sectorService = inject(SectorService);
  private dialogRef = inject(MatDialogRef<SectorModalComponent>);
  private errorService = inject(ErrorService);
  private data = inject<SectorModalData>(MAT_DIALOG_DATA);

  readonly isEditMode = this.data.isEditMode;
  private sectorId = this.data.sector?.id;

  readonly sectorForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    description: ['']
  });

  ngOnInit(): void {
    if (this.isEditMode && this.data.sector) {
      this.sectorForm.patchValue({
        name: this.data.sector.name,
        description: this.data.sector.description || ''
      });
    }
  }

  onSubmit(): void {
    if (this.sectorForm.invalid) return;

    const { name, description } = this.sectorForm.value;

    const payload = {
      name: name!,
      description: description || undefined
    };

    const request$ = this.isEditMode && this.sectorId
      ? this.sectorService.update(this.sectorId, payload)
      : this.sectorService.create(payload);

    request$.subscribe({
      next: () => {
        this.errorService.showSuccess(
          this.isEditMode ? 'Setor atualizado com sucesso' : 'Setor criado com sucesso'
        );
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.errorService.showError(
          this.isEditMode ? 'Erro ao atualizar setor' : 'Erro ao criar setor',
          err
        );
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
