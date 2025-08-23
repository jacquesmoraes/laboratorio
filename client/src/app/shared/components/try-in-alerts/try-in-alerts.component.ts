import { Component, signal, inject, OnInit, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ServiceOrderAlert } from '../../../features/service-order/models/service-order.interface';
import { ServiceOrdersService } from '../../../features/service-order/services/service-order.service';
import { MatFormField, MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-try-in-alerts',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatSelectModule,
    MatFormField,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './try-in-alerts.component.html',
  styleUrls: ['./try-in-alerts.component.scss']
})
export class TryInAlertsComponent implements OnInit {
  private serviceOrdersService = inject(ServiceOrdersService);
  private router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  alerts = signal<ServiceOrderAlert[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  daysThreshold = signal(30);
  daysOptions = [5, 10, 15, 20, 30, 45, 60];

  ngOnInit() {
    this.loadAlerts();
  }

  loadAlerts() {
    this.loading.set(true);
    this.error.set(null);

    this.serviceOrdersService.getWorksOutForTryIn(this.daysThreshold())
    .pipe(takeUntilDestroyed(this.destroyRef)) 
      .subscribe({
        next: (data) => {
          this.alerts.set(data);
          this.loading.set(false);
        },
        error: (err) => {
                    this.loading.set(false);
          
        }
      });
  }
  
  onDaysThresholdChange() {
    this.loadAlerts();
  }

  viewOrder(orderId: number) {
    this.router.navigate(['/admin/service-orders', orderId]);
  }

  viewAllOrders() {
    this.router.navigate(['/admin/service-orders'], { 
      queryParams: { status: 'TryIn' } 
    });
  }

  refreshAlerts() {
    this.loadAlerts();
  }
}