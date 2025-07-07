import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { 
  TablePrice, 
  TablePriceOption, 
  CreateTablePriceDto, 
  UpdateTablePriceDto,
  WorkType, 
  ClientWorkPrice
} from '../table-price.interface';
import { WorkTypeService } from '../../works/services/work-type.service';

@Injectable({
  providedIn: 'root'
})
export class TablePriceService {
  private http = inject(HttpClient);
  private workTypeService = inject(WorkTypeService);
  private apiUrl = `${environment.apiUrl}/tableprice`;
private serviceOrdersApiUrl = `${environment.apiUrl}/serviceorders`;


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

  // Create new table price
  createTablePrice(dto: CreateTablePriceDto): Observable<TablePrice> {
    return this.http.post<TablePrice>(this.apiUrl, dto);
  }

  // Update table price
  updateTablePrice(id: number, dto: UpdateTablePriceDto): Observable<TablePrice> {
    return this.http.put<TablePrice>(`${this.apiUrl}/${id}`, dto);
  }

  // Delete table price
  deleteTablePrice(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Get price by client and work type
  getPriceByClientAndWorkType(clientId: number, workTypeId: number): Observable<ClientWorkPrice> {
  return this.http.get<ClientWorkPrice>(`${this.serviceOrdersApiUrl}/client/${clientId}/worktype/${workTypeId}`);
}



  // Get work types using existing service
  getWorkTypes(): Observable<WorkType[]> {
    return this.workTypeService.getAll().pipe(
      map(workTypes => workTypes.filter(wt => wt.isActive))
    );
  }
}