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
    let httpParams = new HttpParams();

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        httpParams = httpParams.set(key, value.toString());
      }
    });

    return this.http.get<Pagination<Payment>>(this.apiUrl, { params: httpParams });
  }

  getPaymentById(id: number): Observable<Payment> {
    return this.http.get<Payment>(`${this.apiUrl}/client/${id}`);
  }

  createPayment(dto: CreatePaymentDto): Observable<Payment> {
    return this.http.post<Payment>(`${this.apiUrl}/client`, dto);
  }
}