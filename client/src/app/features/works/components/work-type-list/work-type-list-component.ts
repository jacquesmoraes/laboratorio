import { Component, OnInit, inject } from '@angular/core';
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
  templateUrl: './work-type-list-component.html',
  styleUrl: './work-type-list-component.scss'
})
export class WorkTypeListComponent implements OnInit {
  private workTypeService = inject(WorkTypeService);
  private dialog = inject(MatDialog);

  workTypes: WorkType[] = [];
  displayedColumns: string[] = ['id', 'name', 'description', 'workSectionName', 'isActive', 'actions'];

  ngOnInit(): void {
    this.loadWorkTypes();
  }

  loadWorkTypes(): void {
    this.workTypeService.getAll().subscribe({
      next: (data) => {
        this.workTypes = data;
      },
      error: (error) => {
        console.error('Erro ao carregar tipos de trabalho:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar tipos de trabalho',
          confirmButtonText: 'OK'
        });
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
        this.workTypeService.delete(id).subscribe({
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
          error: (error) => {
            console.error('Erro ao excluir tipo de trabalho:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao excluir tipo de trabalho',
              confirmButtonText: 'OK'
            });
          }
        });
      }
    });
  }
}