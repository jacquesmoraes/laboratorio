import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import Swal from 'sweetalert2';

import { Sector } from '../models/sector.interface';
import { SectorService } from '../service/sector.service';

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
  template: `
    <div class="modal-container">
      <h2 mat-dialog-title>
        {{ isEditMode ? 'Editar' : 'Novo' }} Setor
      </h2>

      <form [formGroup]="sectorForm" (ngSubmit)="onSubmit()">
        <mat-dialog-content>
          <mat-form-field appearance="outline" class="w-100">
            <mat-label>Nome</mat-label>
            <input matInput formControlName="name" placeholder="Digite o nome do setor">
            <mat-error *ngIf="sectorForm.get('name')?.hasError('required')">
              Nome é obrigatório
            </mat-error>
            <mat-error *ngIf="sectorForm.get('name')?.hasError('minlength')">
              Nome deve ter pelo menos 2 caracteres
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="w-100">
            <mat-label>Descrição (opcional)</mat-label>
            <textarea matInput formControlName="description" placeholder="Digite uma descrição"></textarea>
          </mat-form-field>
        </mat-dialog-content>

        <mat-dialog-actions align="end">
          <button mat-button type="button" (click)="onCancel()">
            Cancelar
          </button>
          <button 
            mat-raised-button 
            color="primary" 
            type="submit" 
            [disabled]="sectorForm.invalid">
            {{ isEditMode ? 'Atualizar' : 'Criar' }}
          </button>
        </mat-dialog-actions>
      </form>
    </div>
  `,
  styles: [`
    .modal-container {
      min-width: 400px;
      padding: 16px;
    }

    mat-dialog-content {
      margin: 16px 0;
    }

    .w-100 {
      width: 100%;
    }

    mat-form-field {
      margin-bottom: 16px;
    }
  `]
})
export class SectorModalComponent implements OnInit {
  private fb = inject(FormBuilder);
  private sectorService = inject(SectorService);
  private dialogRef = inject(MatDialogRef<SectorModalComponent>);
  private data = inject<SectorModalData>(MAT_DIALOG_DATA);

  sectorForm!: FormGroup;
  isEditMode = this.data.isEditMode;
  sectorId?: number;

  ngOnInit(): void {
    this.initForm();
    if (this.isEditMode && this.data.sector) {
      this.sectorId = this.data.sector.id;
      this.sectorForm.patchValue({
        name: this.data.sector.name,
        description: this.data.sector.description || ''
      });
    }
  }

  private initForm(): void {
    this.sectorForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: ['']
    });
  }

  onSubmit(): void {
    if (this.sectorForm.valid) {
      const formValue = this.sectorForm.value;
      const sectorData = {
        name: formValue.name,
        description: formValue.description || undefined
      };

      if (this.isEditMode && this.sectorId) {
        this.sectorService.update(this.sectorId, sectorData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Setor atualizado com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Erro ao atualizar setor:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao atualizar setor',
              confirmButtonText: 'OK'
            });
          }
        });
      } else {
        this.sectorService.create(sectorData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Setor criado com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Erro ao criar setor:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao criar setor',
              confirmButtonText: 'OK'
            });
          }
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}