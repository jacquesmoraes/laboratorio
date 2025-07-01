import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import Swal from 'sweetalert2';

import { WorkType, CreateWorkTypeDto, UpdateWorkTypeDto } from '../../models/work-type.interface';
import { WorkTypeService } from '../../services/work-type.service';
import { WorkSection } from '../../models/work-section.interface';
import { WorkSectionService } from '../../services/works-section.service';

@Component({
  selector: 'app-work-type-form-component',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSelectModule,
    MatCheckboxModule,
    RouterModule
  ],
  templateUrl: './work-type-form-component.html',
  styleUrl: './work-type-form-component.scss'
})
export class WorkTypeFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private workTypeService = inject(WorkTypeService);
  private workSectionService = inject(WorkSectionService);

  workTypeForm!: FormGroup;
  isEditMode = false;
  workTypeId: number | null = null;
  loading = false;
  workSections: WorkSection[] = [];

  ngOnInit(): void {
    this.initForm();
    this.loadWorkSections();
    this.checkEditMode();
  }

  private initForm(): void {
    this.workTypeForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      isActive: [true],
      workSectionId: ['', Validators.required]
    });
  }

  private loadWorkSections(): void {
    this.workSectionService.getAll().subscribe({
      next: (sections) => {
        this.workSections = sections;
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

  private checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.workTypeId = +id;
      this.loadWorkType(this.workTypeId);
    }
  }

  private loadWorkType(id: number): void {
    this.loading = true;
    this.workTypeService.getById(id).subscribe({
      next: (workType) => {
        this.workTypeForm.patchValue({
          name: workType.name,
          description: workType.description || '',
          isActive: workType.isActive,
          workSectionId: workType.workSectionId
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar tipo de trabalho:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar tipo de trabalho',
          confirmButtonText: 'OK'
        });
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.workTypeForm.valid) {
      this.loading = true;
      const formData = this.workTypeForm.value;

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
              timer: 2000,
              showConfirmButton: false
            });
            this.router.navigate(['/work-types']);
          },
          error: (error) => {
            console.error('Erro ao atualizar tipo de trabalho:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao atualizar tipo de trabalho',
              confirmButtonText: 'OK'
            });
            this.loading = false;
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
              timer: 2000,
              showConfirmButton: false
            });
            this.router.navigate(['/work-types']);
          },
          error: (error) => {
            console.error('Erro ao criar tipo de trabalho:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao criar tipo de trabalho',
              confirmButtonText: 'OK'
            });
            this.loading = false;
          }
        });
      }
    }
  }

  onCancel(): void {
    this.router.navigate(['/work-types']);
  }

  getErrorMessage(fieldName: string): string {
    const field = this.workTypeForm.get(fieldName);
    if (field?.hasError('required')) {
      return 'Este campo é obrigatório';
    }
    if (field?.hasError('minlength')) {
      return 'Mínimo de 2 caracteres';
    }
    return '';
  }
}