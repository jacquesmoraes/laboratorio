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

 getInvoices(params: InvoiceParams): Observable<Pagination<BillingInvoice>> {
  let httpParams = new HttpParams();

  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      httpParams = httpParams.set(key, value.toString());
    }
  });

  return this.http.get<Pagination<BillingInvoice>>(this.apiUrl, { params: httpParams });
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
    return this.http.get(`${environment.apiUrl}/invoices/${id}/pdf`, { 
      responseType: 'blob' 
    });
  }
}