import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Client, CreateClientDto, UpdateClientDto, ClientDetails, Pagination, QueryParams } from '../models/client.interface';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/clients`;

  private serializeParams(params: Record<string, any>): HttpParams {
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

  getClients(): Observable<Client[]> {
    return this.http.get<Client[]>(this.apiUrl);
  }

  getClientById(clientId: number): Observable<ClientDetails> {
    return this.http.get<ClientDetails>(`${this.apiUrl}/${clientId}`);
  }

  createClient(dto: CreateClientDto): Observable<Client> {
    return this.http.post<Client>(this.apiUrl, dto);
  }

  updateClient(clientId: number, dto: UpdateClientDto): Observable<Client> {
    return this.http.put<Client>(`${this.apiUrl}/${clientId}`, dto);
  }

  deleteClient(clientId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${clientId}`);
  }

  getPaginatedClients(params: QueryParams): Observable<Pagination<Client>> {
    return this.http.get<Pagination<Client>>(`${this.apiUrl}/paginated`, { 
      params: this.serializeParams(params) 
    });
  }
}
