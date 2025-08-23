import { Component, signal, inject, OnInit, OnDestroy, DestroyRef } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators, FormControl, FormGroup } from '@angular/forms';
import { BillingInvoiceService } from '../../services/billing-invoice.service';
import { CreateBillingInvoiceDto } from '../../models/billing-invoice.interface';
import { OrderStatus, ServiceOrder, ServiceOrderParams } from '../../../service-order/models/service-order.interface';
import { ServiceOrdersService } from '../../../service-order/services/service-order.service';
import { ClientService } from '../../../clients/services/clients.service';
import { Client } from '../../../clients/models/client.interface';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-billing-invoice-create',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, DatePipe, CurrencyPipe],
  templateUrl: './billing-invoice-create.component.html',
  styleUrls: ['./billing-invoice-create.component.scss']
})
export class BillingInvoiceCreateComponent implements OnInit {
  private billingService = inject(BillingInvoiceService);
  private serviceOrderService = inject(ServiceOrdersService);
  private clientService = inject(ClientService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  loading = signal(false);
  clients = signal<Client[]>([]);
  serviceOrders = signal<ServiceOrder[]>([]);
  selectedServiceOrders = signal<number[]>([]);

  invoiceForm: FormGroup<{
    clientId: FormControl<number | null>;
    description: FormControl<string | null>;
  }> = this.fb.group({
    clientId: this.fb.control<number | null>(null, Validators.required),
    description: this.fb.control<string | null>(null)
  });

  ngOnInit(): void {
    this.loadClients();

    this.invoiceForm.controls.clientId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(clientId => {
        if (clientId !== null) {
          this.loadServiceOrders(clientId);
        } else {
          this.serviceOrders.set([]);
          this.selectedServiceOrders.set([]);
        }
      });
  }

  loadClients(): void {
    this.loading.set(true);
    this.clientService.getClients()
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: clients => {
        this.clients.set(clients);
        this.loading.set(false);
      },
      error: err => {
        this.loading.set(false);
      }
    });
  }

  loadServiceOrders(clientId: number): void {
    this.loading.set(true);

    const params: ServiceOrderParams = {
      pageNumber: 1,
      pageSize: 1000,
      clientId,
      status: OrderStatus.Finished,
      excludeFinished: false,
      excludeInvoiced: true,
      startDate: '',
      endDate: '',
    };

    this.serviceOrderService.getServiceOrders(params)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: response => {
        this.serviceOrders.set(response.data);
        this.selectedServiceOrders.set([]);
        this.loading.set(false);
      },
      error: err => {
        this.loading.set(false);
      }
    });
  }

  onServiceOrderToggle(orderId: number, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.selectedServiceOrders.update(orders =>
      checked ? [...orders, orderId] : orders.filter(id => id !== orderId)
    );
  }

  onSubmit(): void {
    if (this.invoiceForm.valid && this.selectedServiceOrders().length > 0) {
      this.loading.set(true);

      const dto: CreateBillingInvoiceDto = {
        clientId: this.invoiceForm.controls.clientId.value!, // aqui já é number
        serviceOrderIds: this.selectedServiceOrders(),
        description: this.invoiceForm.controls.description.value ?? ''
      };

      this.billingService.createInvoice(dto)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: invoice => {
          this.loading.set(false);
          this.router.navigate(['/admin/billing', invoice.billingInvoiceId]);
        },
        error: err => {

          this.loading.set(false);
          
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/admin/billing']);
  }

  getSelectedTotal(): number {
    return this.serviceOrders()
      .filter(order => this.selectedServiceOrders().includes(order.serviceOrderId))
      .reduce((total, order) => total + order.orderTotal, 0);
  }


}
