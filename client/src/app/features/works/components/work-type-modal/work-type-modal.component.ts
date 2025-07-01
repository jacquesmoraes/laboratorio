import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import Swal from 'sweetalert2';

import { WorkType, CreateWorkTypeDto, UpdateWorkTypeDto } from '../../models/work-type.interface';
import { WorkSection } from '../../models/work-section.interface';
import { WorkTypeService } from '../../services/work-type.service';
import { WorkSectionService } from '../../services/works-section.service';

export interface WorkTypeModalData {
  workType?: WorkType;
  isEditMode: boolean;
}

@Component({
  selector: 'app-work-type-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatCheckboxModule,
    MatDialogModule
  ],
  templateUrl: './work-type-modal.component.html',
  styleUrls: ['./work-type-modal.component.scss']
})
export class WorkTypeModalComponent implements OnInit {
  private fb = inject(FormBuilder);
  private workTypeService = inject(WorkTypeService);
  private workSectionService = inject(WorkSectionService);
  private dialogRef = inject(MatDialogRef<WorkTypeModalComponent>);
  private data = inject<WorkTypeModalData>(MAT_DIALOG_DATA);

  workTypeForm!: FormGroup;
  isEditMode = this.data.isEditMode;
  workTypeId?: number;
  workSections: WorkSection[] = [];

  ngOnInit(): void {
    this.initForm();
    this.loadWorkSections();
  }

  private initForm(): void {
    this.workTypeForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      isActive: [true],
      workSectionId: [null, Validators.required] // Mudança aqui: null em vez de string vazia
    });
  }

  private loadWorkSections(): void {
    this.workSectionService.getAll().subscribe({
      next: (sections) => {
        this.workSections = sections;
        
        // Preencher o formulário após carregar as seções (se for edição)
        if (this.isEditMode && this.data.workType) {
          this.workTypeId = this.data.workType.id;
          this.workTypeForm.patchValue({
            name: this.data.workType.name,
            description: this.data.workType.description || '',
            isActive: this.data.workType.isActive,
            workSectionId: this.data.workType.workSectionId
          });
        }
      },
      error: (error) => {
        console.error('Erro ao carregar seções de trabalho:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar seções de trabalho',
          confirmButtonText: 'OK'
        });
      }
    });
  }

  onSubmit(): void {
    if (this.workTypeForm.valid) {
      const formData = this.workTypeForm.value;
      
      // Verificar se workSectionId é válido
      if (!formData.workSectionId || formData.workSectionId === 0) {
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Selecione uma seção de trabalho',
          confirmButtonText: 'OK'
        });
        return;
      }

      if (this.isEditMode && this.workTypeId) {
        const updateData: UpdateWorkTypeDto = {
          name: formData.name,
          description: formData.description || undefined,
          isActive: formData.isActive,
          workSectionId: formData.workSectionId
        };

        this.workTypeService.update(this.workTypeId, updateData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Tipo de trabalho atualizado com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Erro ao atualizar tipo de trabalho:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao atualizar tipo de trabalho',
              confirmButtonText: 'OK'
            });
          }
        });
      } else {
        const createData: CreateWorkTypeDto = {
          name: formData.name,
          description: formData.description || undefined,
          isActive: formData.isActive,
          workSectionId: formData.workSectionId
        };

        this.workTypeService.create(createData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Tipo de trabalho criado com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Erro ao criar tipo de trabalho:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao criar tipo de trabalho',
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