import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import Swal from 'sweetalert2';

import { SectorService } from '../service/sector.service';

@Component({
  selector: 'app-sector-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
     MatCardModule, 
    MatIconModule,
    RouterModule
  ],
  templateUrl: './sector-form.component.html',
  styleUrls: ['./sector-form.component.scss']
})
export class SectorFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private sectorService = inject(SectorService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  protected sectorForm!: FormGroup;
  protected isEditMode = false;
  protected sectorId?: number;
  protected loading = false;

  ngOnInit(): void {
    this.initForm();
    this.checkEditMode();
  }

  private initForm(): void {
    this.sectorForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: ['']
    });
  }

  private checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.sectorId = +id;
      this.loadSector(this.sectorId);
    }
  }

  private loadSector(id: number): void {
    this.loading = true;
    this.sectorService.getById(id).subscribe({
      next: (sector) => {
        this.sectorForm.patchValue({
          name: sector.name,
          description: sector.description || ''
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar setor:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar setor',
          confirmButtonText: 'OK'
        });
        this.loading = false;
      }
    });
  }

  protected onSubmit(): void {
    if (this.sectorForm.valid) {
      this.loading = true;
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
            this.router.navigate(['/sectors']);
          },
          error: (error) => {
            console.error('Erro ao atualizar setor:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao atualizar setor',
              confirmButtonText: 'OK'
            });
            this.loading = false;
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
            this.router.navigate(['/sectors']);
          },
          error: (error) => {
            console.error('Erro ao criar setor:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao criar setor',
              confirmButtonText: 'OK'
            });
            this.loading = false;
          }
        });
      }
    }
  }

  protected onCancel(): void {
    this.router.navigate(['/sectors']);
  }

  protected getErrorMessage(fieldName: string): string {
    const field = this.sectorForm.get(fieldName);
    if (field?.hasError('required')) {
      return 'Este campo é obrigatório';
    }
    if (field?.hasError('minlength')) {
      return 'Mínimo de 2 caracteres';
    }
    return '';
  }
}