// table-price-list.component.ts atualizado
import { Component, inject, signal, computed, ChangeDetectionStrategy, DestroyRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TablePriceService } from '../../services/table-price.services';
import { TablePrice } from '../../table-price.interface';

import { MatTooltip } from '@angular/material/tooltip';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-table-price-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatTooltip,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    
  ],
  templateUrl: './table-price-list.component.html',
  styleUrls: ['./table-price-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TablePriceListComponent implements OnInit {
  private tablePriceService = inject(TablePriceService);
  private router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  tablePrices = signal<TablePrice[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  isEmpty = computed(() => this.tablePrices().length === 0);

  ngOnInit(): void {
    this.loadTablePrices();
  }

  private loadTablePrices(): void {
    this.loading.set(true);
    this.error.set(null);

    this.tablePriceService.getTablePrices()
      .pipe(takeUntilDestroyed(this.destroyRef)) // ← Adicionar
      .subscribe({
        next: (data) => {
          this.tablePrices.set(data);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
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
        this.tablePriceService.deleteTablePrice(id)
          .pipe(takeUntilDestroyed(this.destroyRef)) // ← Adicionar
          .subscribe({
            next: () => {
              this.tablePrices.update(prices => prices.filter(p => p.id !== id));
              Swal.fire('Excluído!', 'A tabela de preço foi excluída com sucesso.', 'success');
            },
            error: () => {
             
            }
          });
      }
    });
  }
}
