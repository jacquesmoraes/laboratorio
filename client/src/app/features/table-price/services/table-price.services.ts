import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TablePrice, TablePriceOption } from '../table-price.interface';


@Injectable({
  providedIn: 'root'
})
export class TablePriceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/tableprice`;

  // Get all table prices
  getTablePrices(): Observable<TablePrice[]> {
    return this.http.get<TablePrice[]>(this.apiUrl);
  }

  // Get table prices as options for dropdown
  getTablePriceOptions(): Observable<TablePriceOption[]> {
    return this.getTablePrices().pipe(
      map(tablePrices => 
        tablePrices
          .filter(tp => tp.status) // Only active table prices
          .map(tp => ({
            value: tp.id,
            label: tp.name,
            description: tp.description
          }))
      )
    );
  }

  // Get table price by ID
  getTablePriceById(id: number): Observable<TablePrice> {
    return this.http.get<TablePrice>(`${this.apiUrl}/${id}`);
  }
}