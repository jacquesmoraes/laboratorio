import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import Swal from 'sweetalert2';

import { WorkType } from '../../models/work-type.interface';
import { WorkTypeService } from '../../services/work-type.service';

@Component({
  selector: 'app-work-type-list-component',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule
  ],
  templateUrl: './work-type-list-component.html',
  styleUrl: './work-type-list-component.scss'
})
export class WorkTypeListComponent implements OnInit {
  workTypes: WorkType[] = [];
  displayedColumns: string[] = ['id', 'name', 'description', 'workSectionName', 'isActive', 'actions'];
  loading = false;

  constructor(
    private workTypeService: WorkTypeService
  ) { }

  ngOnInit(): void {
    this.loadWorkTypes();
  }

  loadWorkTypes(): void {
    this.loading = true;
    this.workTypeService.getAll().subscribe({
      next: (data) => {
        this.workTypes = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar tipos de trabalho:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar tipos de trabalho',
          confirmButtonText: 'OK'
        });
        this.loading = false;
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