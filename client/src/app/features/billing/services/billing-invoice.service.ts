import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { 
  BillingInvoice, 
  CreateBillingInvoiceDto, 
  InvoiceParams, 
  Pagination 
} from '../models/billing-invoice.interface';

@Injectable({
  providedIn: 'root'
})
export class BillingInvoiceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/billingInvoices`;
  private invoicesUrl = `${environment.apiUrl}/invoices`;
  private serializeParams(params: InvoiceParams): HttpParams {
    let httpParams = new HttpParams();

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        if (value instanceof Date) {
          httpParams = httpParams.set(key, value.toISOString());
        } else {
          httpParams = httpParams.set(key, String(value));
        }
      }
    });

    return httpParams;
  }

  getInvoices(params: InvoiceParams): Observable<Pagination<BillingInvoice>> {
    return this.http.get<Pagination<BillingInvoice>>(this.apiUrl, { 
      params: this.serializeParams(params) 
    });
  }

  getInvoiceById(id: number): Observable<BillingInvoice> {
    return this.http.get<BillingInvoice>(`${this.apiUrl}/${id}`);
  }

  createInvoice(dto: CreateBillingInvoiceDto): Observable<BillingInvoice> {
    return this.http.post<BillingInvoice>(`${this.apiUrl}/invoice`, dto);
  }

  cancelInvoice(id: number): Observable<BillingInvoice> {
    return this.http.post<BillingInvoice>(`${this.apiUrl}/${id}/cancel`, {});
  }

  downloadInvoicePdf(id: number): Observable<Blob> {
    // Alinhar rota com backend — mantive dentro de billingInvoices
    return this.http.get(`${this.invoicesUrl}/${id}/pdf`, { 
      responseType: 'blob' 
    });
  }
}
