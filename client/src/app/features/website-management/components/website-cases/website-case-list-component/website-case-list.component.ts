import { Component, OnInit, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

import { WebsiteCaseService } from '../../../services/website-case.service';
import { WebsiteCaseAdmin } from '../../../models/website-case.interface';
import Swal, { SweetAlertResult } from 'sweetalert2';
import { ErrorMappingService } from '../../../../../core/services/error.mapping.service';
import { LoadingService } from '../../../../../core/services/loading.service';

@Component({
  selector: 'app-website-case-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatMenuModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './website-case-list.component.html',
  styleUrls: ['./website-case-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WebsiteCaseListComponent implements OnInit {
  private readonly websiteCaseService = inject(WebsiteCaseService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly errorMapping = inject(ErrorMappingService);
  private readonly loadingService = inject(LoadingService);

  // Signals
  cases = signal<WebsiteCaseAdmin[]>([]);

  ngOnInit(): void {
    this.loadCases();
  }

  loadCases(): void {
    this.websiteCaseService.getAll().subscribe({
      next: (cases) => {
        this.cases.set(cases);
      },
      error: (error) => {
        console.error('Erro ao carregar casos:', error);
        const errorMessage = this.errorMapping.mapServiceOrderError(error);
        this.snackBar.open(errorMessage, 'Fechar', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  createNewCase(): void {
    this.router.navigate(['/admin/website-management/cases/new']);
  }

  viewCase(id: number): void {
    this.router.navigate(['/admin/website-management/cases', id]);
  }

  editCase(id: number): void {
    this.router.navigate(['/admin/website-management/cases', id, 'edit']);
  }

  toggleStatus(caseItem: WebsiteCaseAdmin): void {
    this.websiteCaseService.toggleActive(caseItem.id).subscribe({
      next: () => {
        // Atualizar o estado local
        this.cases.update(cases => 
          cases.map(c => 
            c.id === caseItem.id 
              ? { ...c, isActive: !c.isActive }
              : c
          )
        );
        
        this.snackBar.open(
          `Caso ${caseItem.isActive ? 'desativado' : 'ativado'} com sucesso!`, 
          'Fechar', 
          { duration: 3000 }
        );
      },
      error: (error) => {
        console.error('Erro ao alterar status:', error);
        const errorMessage = this.errorMapping.mapServiceOrderError(error);
        this.snackBar.open(errorMessage, 'Fechar', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  deleteCase(caseItem: WebsiteCaseAdmin): void {
    Swal.fire({
      title: 'Confirmar exclusão',
      text: `Tem certeza que deseja excluir o caso "${caseItem.title}"? Esta ação não pode ser desfeita.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sim, excluir!',
      cancelButtonText: 'Cancelar'
    }).then((result: SweetAlertResult) => {
      if (result.isConfirmed) {
        this.websiteCaseService.delete(caseItem.id).subscribe({
          next: () => {
            // Remover da lista local
            this.cases.update(cases => 
              cases.filter(c => c.id !== caseItem.id)
            );
            
            this.snackBar.open('Caso excluído com sucesso!', 'Fechar', { duration: 3000 });
          },
          error: (error) => {
            console.error('Erro ao excluir caso:', error);
            const errorMessage = this.errorMapping.mapServiceOrderError(error);
            this.snackBar.open(errorMessage, 'Fechar', {
              duration: 3000,
              panelClass: ['error-snackbar']
            });
          }
        });
      }
    });
  }

  onImageError(event: Event): void {
    const target = event.target as HTMLImageElement;
    if (target) {
      target.style.display = 'none';
    }
  }
}