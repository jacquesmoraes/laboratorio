import { Component, OnInit, inject, ChangeDetectionStrategy, signal, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import Swal from 'sweetalert2';

import { WorkType } from '../../models/work-type.interface';
import { WorkTypeService } from '../../services/work-type.service';
import { WorkTypeModalComponent, WorkTypeModalData } from '../work-type-modal/work-type-modal.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-work-type-list-component',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatDialogModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './work-type-list-component.html',
  styleUrl: './work-type-list-component.scss'
})
export class WorkTypeListComponent implements OnInit {
  private workTypeService = inject(WorkTypeService);
  private dialog = inject(MatDialog);
  private readonly destroyRef = inject(DestroyRef);
  workTypes = signal<WorkType[]>([]);
  displayedColumns: string[] = [ 'name', 'description', 'workSectionName', 'isActive', 'actions'];

  ngOnInit(): void {
    
    this.loadWorkTypes();
  }

  loadWorkTypes(): void {
    this.workTypeService.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef)) 
      .subscribe({
        next: (data) => {
          this.workTypes.set(data);
        },
        error: () => {
         
        }
      });
  }

  onNew(): void {
    const dialogRef = this.dialog.open(WorkTypeModalComponent, {
      data: { isEditMode: false } as WorkTypeModalData,
      width: '550px'
    });
  
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadWorkTypes();
        // ✅ ADICIONADO: SweetAlert na lista
        Swal.fire({
          icon: 'success',
          title: 'Sucesso!',
          text: 'Tipo de trabalho criado com sucesso',
          timer: 1000,
          showConfirmButton: false
        });
      }
    });
  }

  onEdit(workType: WorkType): void {
    const dialogRef = this.dialog.open(WorkTypeModalComponent, {
      data: { workType, isEditMode: true } as WorkTypeModalData,
      width: '550px'
    });
  
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadWorkTypes();
        
        Swal.fire({
          icon: 'success',
          title: 'Sucesso!',
          text: 'Tipo de trabalho atualizado com sucesso',
          timer: 1000,
          showConfirmButton: false
        });
      }
    });
  }

  deleteWorkType(id: number): void {
    Swal.fire({
      title: 'Tem certeza?',
      text: 'Esta ação não pode ser desfeita!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sim, excluir!',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.workTypeService.delete(id)
          .pipe(takeUntilDestroyed(this.destroyRef)) // ← Adicionar
          .subscribe({
            next: () => {
              Swal.fire({
                icon: 'success',
                title: 'Sucesso!',
                text: 'Tipo de trabalho excluído com sucesso',
                timer: 1000,
                showConfirmButton: false
              });
              this.loadWorkTypes();
            },
            error: () => {
             
            }
          });
      }
    });
  }
}