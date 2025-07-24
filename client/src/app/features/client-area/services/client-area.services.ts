import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { InvoiceParams, BillingInvoice } from '../../billing/models/billing-invoice.interface';
import { PaymentParams, Payment } from '../../payments/models/payment.interface';
import { Pagination,  ServiceOrder } from '../../service-order/models/service-order.interface';
import { environment } from '../../../../environments/environment';
import { ClientAreaInvoice, ClientAreaServiceOrder, ClientAreaServiceOrderDetails, ClientDashboard, ServiceOrderParams } from '../models/client-area.model';

@Injectable({ providedIn: 'root' })
export class ClientAreaService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/client-area`;

  /**
   * Dashboard com dados básicos e totais financeiros.
   */
  getDashboard(): Observable<ClientDashboard> {
    return this.http.get<ClientDashboard>(`${this.apiUrl}/dashboard`);
  }

  /**
   * Paginação de pagamentos do cliente.
   */
  getPayments(params: PaymentParams): Observable<Pagination<Payment>> {
    return this.http.get<Pagination<Payment>>(`${this.apiUrl}/payments`, {
      params: this.toHttpParams(params)
    });
  }

  /**
   * Paginação de faturas do cliente.
   */
  getInvoices(params: InvoiceParams): Observable<Pagination<ClientAreaInvoice>> {
  return this.http.get<Pagination<ClientAreaInvoice>>(`${this.apiUrl}/invoices`, {
    params: this.toHttpParams(params)
  });
}


  /**
   * Baixar PDF da fatura.
   */
  downloadInvoice(id: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/invoices/${id}/download`, {
      responseType: 'blob'
    });
  }

  /**
   * Paginação de ordens de serviço do cliente.
   */
  getOrders(params: ServiceOrderParams): Observable<Pagination<ClientAreaServiceOrder>> {
    return this.http.get<Pagination<ClientAreaServiceOrder>>(`${this.apiUrl}/orders`, {
      params: this.toHttpParams(params)
    });
  }
  getOrderDetails(id: number): Observable<ClientAreaServiceOrderDetails> {
  return this.http.get<ClientAreaServiceOrderDetails>(`${this.apiUrl}/orders/${id}`);
}


  private toHttpParams<T extends object>(obj: T): HttpParams {
  let httpParams = new HttpParams();
  Object.entries(obj).forEach(([key, value]) => {
    if (value !== undefined && value !== null) {
      httpParams = httpParams.set(key, String(value));
    }
  });
  return httpParams;
}

}
