import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import Swal from 'sweetalert2';

import { WorkSection } from '../../models/work-section.interface';
import { WorkSectionService } from '../../services/works-section.service';

export interface WorkSectionModalData {
  workSection?: WorkSection;
  isEditMode: boolean;
}

@Component({
  selector: 'app-work-section-modal',
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
  templateUrl: './work-section-modal.component.html',
  styleUrls: ['./work-section-modal.component.scss']
})
export class WorkSectionModalComponent implements OnInit {
  private fb = inject(FormBuilder);
  private workSectionService = inject(WorkSectionService);
  private dialogRef = inject(MatDialogRef<WorkSectionModalComponent>);
  private data = inject<WorkSectionModalData>(MAT_DIALOG_DATA);

  workSectionForm!: FormGroup;
  isEditMode = this.data.isEditMode;
  workSectionId?: number;

  ngOnInit(): void {
    this.initForm();
    if (this.isEditMode && this.data.workSection) {
      this.workSectionId = this.data.workSection.id;
      this.workSectionForm.patchValue({
        name: this.data.workSection.name
      });
    }
  }

  private initForm(): void {
    this.workSectionForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  onSubmit(): void {
    if (this.workSectionForm.valid) {
      const formData = this.workSectionForm.value;

      if (this.isEditMode && this.workSectionId) {
        this.workSectionService.update(this.workSectionId, formData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Seção de trabalho atualizada com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Erro ao atualizar seção de trabalho:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao atualizar seção de trabalho',
              confirmButtonText: 'OK'
            });
          }
        });
      } else {
        this.workSectionService.create(formData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Seção de trabalho criada com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Erro ao criar seção de trabalho:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao criar seção de trabalho',
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