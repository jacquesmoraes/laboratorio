import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import Swal from 'sweetalert2';
import { WorkSection } from '../../models/work-section.interface';
import { WorkSectionService } from '../../services/works-section.service';

@Component({
  selector: 'app-work-section-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './work-section-list.component.html',
  styleUrls: ['./work-section-list.component.scss']
})
export class WorkSectionListComponent implements OnInit {
  workSections: WorkSection[] = [];
  displayedColumns: string[] = ['id', 'name', 'actions'];
  loading = false;

  constructor(
    private workSectionService: WorkSectionService
  ) { }

  ngOnInit(): void {
    this.loadWorkSections();
  }

  loadWorkSections(): void {
    this.loading = true;
    this.workSectionService.getAll().subscribe({
      next: (data) => {
        this.workSections = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar seções de trabalho:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar seções de trabalho',
          confirmButtonText: 'OK'
        });
        this.loading = false;
      }
    });
  }

  deleteWorkSection(id: number): void {
  // Teste se o Swal está disponível
  if (typeof Swal !== 'undefined') {
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
        this.performDelete(id);
      }
    });
  } else {
    // Fallback para confirm padrão
    if (confirm('Tem certeza que deseja excluir esta seção de trabalho?')) {
      this.performDelete(id);
    }
  }
}

private performDelete(id: number): void {
  this.workSectionService.delete(id).subscribe({
    next: () => {
      if (typeof Swal !== 'undefined') {
        Swal.fire({
          icon: 'success',
          title: 'Sucesso!',
          text: 'Seção de trabalho excluída com sucesso',
          timer: 2000,
          showConfirmButton: false
        });
      } else {
        alert('Seção de trabalho excluída com sucesso');
      }
      this.loadWorkSections();
    },
    error: (error) => {
      console.error('Erro ao excluir seção de trabalho:', error);
      if (typeof Swal !== 'undefined') {
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao excluir seção de trabalho',
          confirmButtonText: 'OK'
        });
      } else {
        alert('Erro ao excluir seção de trabalho');
      }
    }
  });
}
}