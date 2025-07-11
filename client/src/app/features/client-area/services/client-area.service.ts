import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../../../environments/environment";
import { ClientDashboardData, PaymentParams, PaginatedResponse, ClientPayment, OrderParams, ServiceOrder, InvoiceParams, ClientInvoice } from "../models/client-area.interface";

@Injectable({ providedIn: 'root' })
export class ClientAreaService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/client-area`;

  getDashboard(): Observable<ClientDashboardData> {
    return this.http.get<ClientDashboardData>(`${this.apiUrl}/dashboard`);
  }

  getPayments(params: PaymentParams = {}): Observable<PaginatedResponse<ClientPayment>> {
    const httpParams = this.toHttpParams(params);
    return this.http.get<PaginatedResponse<ClientPayment>>(`${this.apiUrl}/payments`, { params: httpParams });
  }

  getOrders(params: OrderParams = {}): Observable<PaginatedResponse<ServiceOrder>> {
    const httpParams = this.toHttpParams(params);
    return this.http.get<PaginatedResponse<ServiceOrder>>(`${this.apiUrl}/orders`, { params: httpParams });
  }

  getInvoices(params: InvoiceParams = {}): Observable<PaginatedResponse<ClientInvoice>> {
    const httpParams = this.toHttpParams(params);
    return this.http.get<PaginatedResponse<ClientInvoice>>(`${this.apiUrl}/invoices`, { params: httpParams });
  }

  downloadInvoice(invoiceId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/invoices/${invoiceId}/download`, {
      responseType: 'blob'
    });
  }

  private toHttpParams(params: object): HttpParams {
    let httpParams = new HttpParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        httpParams = httpParams.set(key, String(value));
      }
    });
    return httpParams;
  }
}
