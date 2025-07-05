import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import Swal from 'sweetalert2';
import { Scale } from '../../../models/scale.interface';
import { Shade, UpdateShadeDto, CreateShadeDto } from '../../../models/shade.interface';
import { ScaleService } from '../../../services/scale.service';
import { ShadeService } from '../../../services/shade.service';


export interface ShadeModalData {
  shade?: Shade;
  isEditMode: boolean;
}

@Component({
  selector: 'app-shade-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatDialogModule
  ],
  templateUrl: './shade-modal.component.html',
  styleUrls: ['./shade-modal.component.scss']
})
export class ShadeModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly shadeService = inject(ShadeService);
  private readonly scaleService = inject(ScaleService);
  private readonly dialogRef = inject(MatDialogRef<ShadeModalComponent>);
  private readonly data = inject<ShadeModalData>(MAT_DIALOG_DATA);

  protected readonly shadeForm = this.fb.group({
    color: ['', [Validators.required, Validators.minLength(2)]],
    scaleId: [null as number | null, Validators.required]
  });

  protected readonly isEditMode = signal(this.data.isEditMode);
  protected readonly scales = signal<Scale[]>([]);
  
  private shadeId?: number;

  ngOnInit(): void {
    this.loadScales();
  }

  private loadScales(): void {
    this.scaleService.getAll().subscribe({
      next: (scales) => {
        this.scales.set(scales);
        
        // Preencher o formulário após carregar as escalas (se for edição)
        if (this.isEditMode() && this.data.shade) {
          this.shadeId = this.data.shade.id;
          this.shadeForm.patchValue({
            color: this.data.shade.color || '',
            scaleId: this.data.shade.scaleId
          });
        }
      },
      error: (error) => {
        console.error('Erro ao carregar escalas:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar escalas',
          confirmButtonText: 'OK'
        });
      }
    });
  }

  protected onSubmit(): void {
    if (this.shadeForm.valid) {
      const formData = this.shadeForm.value;
      
      // Verificar se scaleId é válido
      if (!formData.scaleId || formData.scaleId === 0) {
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Selecione uma escala',
          confirmButtonText: 'OK'
        });
        return;
      }

      if (this.isEditMode() && this.shadeId) {
        const updateData: UpdateShadeDto = {
          color: formData.color || undefined,
          scaleId: formData.scaleId
        };

        this.shadeService.update(this.shadeId, updateData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Cor atualizada com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Erro ao atualizar cor:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao atualizar cor',
              confirmButtonText: 'OK'
            });
          }
        });
      } else {
        const createData: CreateShadeDto = {
          color: formData.color || undefined,
          scaleId: formData.scaleId
        };

        this.shadeService.create(createData).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Cor criada com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Erro ao criar cor:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao criar cor',
              confirmButtonText: 'OK'
            });
          }
        });
      }
    }
  }

  protected onCancel(): void {
    this.dialogRef.close();
  }
}