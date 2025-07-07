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

  // Get all clients
  getClients(): Observable<Client[]> {
    return this.http.get<Client[]>(this.apiUrl);
  }

  // Get client by ID with details
  getClientById(id: number): Observable<ClientDetails> {
    return this.http.get<ClientDetails>(`${this.apiUrl}/${id}`);
  }

  // Create a new client
  createClient(client: CreateClientDto): Observable<Client> {
    return this.http.post<Client>(this.apiUrl, client);
  }

  // Update a client
  updateClient(id: number, client: UpdateClientDto): Observable<Client> {
    return this.http.put<Client>(`${this.apiUrl}/${id}`, client);
  }

  // Delete a client
  deleteClient(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

 getPaginatedClients(params: QueryParams): Observable<Pagination<Client>> {
    const queryParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString())
      .set('sort', params.sort || 'clientName')
      .set('search', params.search || '');
    
    return this.http.get<Pagination<Client>>(`${this.apiUrl}/paginated`, { params: queryParams });
  }
}