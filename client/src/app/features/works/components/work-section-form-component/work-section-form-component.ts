import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import Swal from 'sweetalert2';


import { WorkSectionService } from '../../services/works-section.service';

@Component({
  selector: 'app-work-section-form-component',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    RouterModule
  ],
  templateUrl: './work-section-form-component.html',
  styleUrl: './work-section-form-component.scss'
})
export class WorkSectionFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private workSectionService = inject(WorkSectionService);

  workSectionForm!: FormGroup;
  isEditMode = false;
  workSectionId: number | null = null;
  loading = false;

  ngOnInit(): void {
    this.initForm();
    this.checkEditMode();
  }

  private initForm(): void {
    this.workSectionForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  private checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.workSectionId = +id;
      this.loadWorkSection(this.workSectionId);
    }
  }

  private loadWorkSection(id: number): void {
    this.loading = true;
    this.workSectionService.getById(id).subscribe({
      next: (workSection) => {
        this.workSectionForm.patchValue({
          name: workSection.name
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar seção de trabalho:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar seção de trabalho',
          confirmButtonText: 'OK'
        });
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.workSectionForm.valid) {
      this.loading = true;
      const formData = this.workSectionForm.value;

      if (this.isEditMode && this.workSectionId) {
        this.workSectionService.update(this.workSectionId, formData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Seção de trabalho atualizada com sucesso',
              timer: 2000,
              showConfirmButton: false
            });
            this.router.navigate(['/work-sections']);
          },
          error: (error) => {
            console.error('Erro ao atualizar seção de trabalho:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao atualizar seção de trabalho',
              confirmButtonText: 'OK'
            });
            this.loading = false;
          }
        });
      } else {
        this.workSectionService.create(formData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Seção de trabalho criada com sucesso',
              timer: 2000,
              showConfirmButton: false
            });
            this.router.navigate(['/work-sections']);
          },
          error: (error) => {
            console.error('Erro ao criar seção de trabalho:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao criar seção de trabalho',
              confirmButtonText: 'OK'
            });
            this.loading = false;
          }
        });
      }
    }
  }

  onCancel(): void {
    this.router.navigate(['/work-sections']);
  }

  getErrorMessage(fieldName: string): string {
    const field = this.workSectionForm.get(fieldName);
    if (field?.hasError('required')) {
      return 'Este campo é obrigatório';
    }
    if (field?.hasError('minlength')) {
      return 'Mínimo de 2 caracteres';
    }
    return '';
  }
}