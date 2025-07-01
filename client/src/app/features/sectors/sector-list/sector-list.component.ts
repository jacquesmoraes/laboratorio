import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import Swal from 'sweetalert2';

import { Sector } from '../models/sector.interface';
import { SectorService } from '../service/sector.service';

@Component({
  selector: 'app-sector-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, RouterModule],
  templateUrl: './sector-list.component.html',
  styleUrls: ['./sector-list.component.scss'],
})
export class SectorListComponent implements OnInit {
  private sectorService = inject(SectorService);
  private router = inject(Router);

  protected sectors = signal<Sector[]>([]);
  protected displayedColumns = ['name', 'actions'];
  protected loading = false;

  ngOnInit(): void {
    this.loadSectors();
  }

  private loadSectors(): void {
    this.loading = true;
    this.sectorService.getAll().subscribe({
      next: (sectors) => {
        this.sectors.set(sectors);
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar setores:', error);
        Swal.fire({
          icon: 'error',
          title: 'Erro!',
          text: 'Erro ao carregar setores',
          confirmButtonText: 'OK'
        });
        this.loading = false;
      }
    });
  }

  protected onEdit(sector: Sector): void {
    this.router.navigate(['/sectors', sector.id, 'edit']);
  }

  protected onDelete(sector: Sector): void {
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
        this.sectorService.delete(sector.id).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Sucesso!',
              text: 'Setor excluído com sucesso',
              timer: 1000,
              showConfirmButton: false
            });
            this.loadSectors();
          },
          error: (error) => {
            console.error('Erro ao excluir setor:', error);
            Swal.fire({
              icon: 'error',
              title: 'Erro!',
              text: 'Erro ao excluir setor',
              confirmButtonText: 'OK'
            });
          }
        });
      }
    });
  }
}