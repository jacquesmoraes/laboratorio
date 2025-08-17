import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { 
  Payment, 
  CreatePaymentDto, 
  PaymentParams, 
  Pagination 
} from '../models/payment.interface';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/payments`;

  getPayments(params: PaymentParams): Observable<Pagination<Payment>> {
    return this.http.get<Pagination<Payment>>(this.apiUrl, { 
      params: this.serializeParams(params) 
    });
  }


  getPaymentById(id: number): Observable<Payment> {
    return this.http.get<Payment>(`${this.apiUrl}/client/${id}`);
  }

  createPayment(dto: CreatePaymentDto): Observable<Payment> {
    return this.http.post<Payment>(`${this.apiUrl}/client`, dto);
  }

  private serializeParams(params: PaymentParams): HttpParams {
    let httpParams = new HttpParams();

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        if (value instanceof Date) {
          httpParams = httpParams.set(key, this.formatDateForApi(value));
        } else if (typeof value === 'string' && this.isDateString(value)) {
          const date = new Date(value);
          httpParams = httpParams.set(key, this.formatDateForApi(date));
        } else {
          httpParams = httpParams.set(key, String(value));
        }
      }
    });

    return httpParams;
  }
  private formatDateForApi(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private isDateString(value: string): boolean {
    const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
    return dateRegex.test(value) || !isNaN(Date.parse(value));
  }

}