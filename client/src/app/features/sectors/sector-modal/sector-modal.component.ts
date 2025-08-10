import { Component, OnInit, inject, signal, computed, DestroyRef, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Sector } from '../models/sector.interface';
import { SectorService } from '../service/sector.service';

import Swal from 'sweetalert2';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

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
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './sector-modal.component.html',
  styleUrls: ['./sector-modal.component.scss']
})
export class SectorModalComponent implements OnInit {
  private fb = inject(FormBuilder);
  private sectorService = inject(SectorService);
  private dialogRef = inject(MatDialogRef<SectorModalComponent>);
  private data = inject<SectorModalData>(MAT_DIALOG_DATA);
  private readonly destroyRef = inject(DestroyRef);
  readonly isEditMode = this.data.isEditMode;
  private sectorId = this.data.sector?.id;

  readonly sectorForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    description: ['']
  });

  ngOnInit(): void {
    if (this.isEditMode && this.data.sector) {
      this.sectorForm.patchValue({
        name: this.data.sector.name
        
      });
    }
  }

  onSubmit(): void {
    if (this.sectorForm.invalid) return;

    const { name } = this.sectorForm.value;

    const payload = {
      name: name!
    };

    const request$ = this.isEditMode && this.sectorId
      ? this.sectorService.update(this.sectorId, payload)
      : this.sectorService.create(payload);

    request$
      .pipe(takeUntilDestroyed(this.destroyRef)) 
      .subscribe({
        next: () => {
         
          Swal.fire(
            'Sucesso!',
            this.isEditMode ? 'Setor atualizado com sucesso' : 'Setor criado com sucesso',
            'success'
          );
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
