// table-price-list.component.ts atualizado
import { Component, inject, signal, computed, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { TablePriceService } from '../../services/table-price.services';
import { TablePrice } from '../../table-price.interface';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-table-price-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    SweetAlert2Module
  ],
  templateUrl: './table-price-list.component.html',
  styleUrls: ['./table-price-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TablePriceListComponent {
  private tablePriceService = inject(TablePriceService);
  private router = inject(Router);

  tablePrices = signal<TablePrice[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  isEmpty = computed(() => this.tablePrices().length === 0);

  constructor() {
    this.loadTablePrices();
  }

  private loadTablePrices(): void {
    this.loading.set(true);
    this.error.set(null);

    this.tablePriceService.getTablePrices().subscribe({
      next: (data) => {
        this.tablePrices.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Erro ao carregar tabelas de preço');
        this.loading.set(false);
        console.error('Error loading table prices:', err);
      }
    });
  }

  onNew(): void {
    this.router.navigate(['/table-price/new']);
  }

  onView(id: number): void {
    this.router.navigate(['/table-price', id]);
  }

  onEdit(id: number): void {
    this.router.navigate(['/table-price', id, 'edit']);
  }

  deleteTablePrice(id: number): void {
    Swal.fire({
      title: 'Tem certeza?',
      text: "Esta ação não poderá ser revertida!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sim, excluir!',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.tablePriceService.deleteTablePrice(id).subscribe({
          next: () => {
            this.tablePrices.update(prices => prices.filter(p => p.id !== id));
            Swal.fire('Excluído!', 'A tabela de preço foi excluída com sucesso.', 'success');
          },
          error: (err) => {
            Swal.fire('Erro!', 'Erro ao excluir tabela de preço.', 'error');
            console.error('Error deleting table price:', err);
          }
        });
      }
    });
  }
}
