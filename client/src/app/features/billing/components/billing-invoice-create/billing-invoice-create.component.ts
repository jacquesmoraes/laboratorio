import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BillingInvoiceService } from '../../services/billing-invoice.service';
import { CreateBillingInvoiceDto } from '../../models/billing-invoice.interface';
import { OrderStatus, ServiceOrder, ServiceOrderParams } from '../../../service-order/models/service-order.interface';
import { ServiceOrdersService } from '../../../service-order/services/service-order.service';
import { ClientService } from '../../../clients/services/clients.services';
import { Client } from '../../../clients/models/client.interface';

@Component({
  selector: 'app-billing-invoice-create',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './billing-invoice-create.component.html',
  styleUrls: ['./billing-invoice-create.component.scss']
})
export class BillingInvoiceCreateComponent implements OnInit {
  private billingService = inject(BillingInvoiceService);
  private serviceOrderService = inject(ServiceOrdersService);
  private clientService = inject(ClientService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  loading = signal(false);
  clients = signal<Client[]>([]);
  serviceOrders = signal<ServiceOrder[]>([]);
  selectedServiceOrders = signal<number[]>([]);



  invoiceForm: FormGroup = this.fb.group({
    clientId: ['', Validators.required],
    description: ['']
  });

  ngOnInit(): void {
    this.loadClients();

    this.invoiceForm.get('clientId')?.valueChanges.subscribe(clientId => {
      if (clientId) {
        this.loadServiceOrders(clientId);
      } else {
        this.serviceOrders.set([]);
        this.selectedServiceOrders.set([]);
      }
    });
  }

  loadClients(): void {
    this.loading.set(true);
    this.clientService.getClients().subscribe({
      next: (clients) => {
        this.clients.set(clients);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar clientes:', error);
        this.loading.set(false);
      }
    });
  }

  loadServiceOrders(clientId: number): void {
    this.loading.set(true);

    const params: ServiceOrderParams = {
      pageNumber: 1,
      pageSize: 1000,
      clientId: clientId,
      status: OrderStatus.Finished,
      excludeFinished: false,
       excludeInvoiced: true //
    };

    this.serviceOrderService.getServiceOrders(params).subscribe({
      next: (response) => {
        this.serviceOrders.set(response.data);
        this.selectedServiceOrders.set([]);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar ordens de serviço:', error);
        this.loading.set(false);
      }
    });
  }

  
  onServiceOrderToggle(orderId: number, event: any): void {
    const checked = event.target.checked;
    if (checked) {
      this.selectedServiceOrders.update(orders => [...orders, orderId]);
    } else {
      this.selectedServiceOrders.update(orders => orders.filter(id => id !== orderId));
    }
  }

  onSubmit(): void {
    if (this.invoiceForm.valid && this.selectedServiceOrders().length > 0) {
      this.loading.set(true);

      const dto: CreateBillingInvoiceDto = {
        clientId: this.invoiceForm.value.clientId,
        serviceOrderIds: this.selectedServiceOrders(),
        description: this.invoiceForm.value.description
      };

      this.billingService.createInvoice(dto).subscribe({
        next: (invoice) => {
          this.loading.set(false);
          this.router.navigate(['/billing', invoice.billingInvoiceId]);
        },
        error: (error) => {
          console.error('Erro ao criar fatura:', error);
          this.loading.set(false);
          alert('Erro ao criar fatura');
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/billing']);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  getSelectedTotal(): number {
    return this.serviceOrders()
      .filter(order => this.selectedServiceOrders().includes(order.serviceOrderId))
      .reduce((total, order) => total + order.orderTotal, 0);
  }

}