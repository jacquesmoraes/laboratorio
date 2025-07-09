import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { 
  ServiceOrder, 
  ServiceOrderDetails, 
  ServiceOrderAlert, 
  CreateServiceOrderDto, 
  MoveToStageDto, 
  SendToTryInDto, 
  FinishOrderDto, 
  ServiceOrderParams, 
  Pagination 
} from '../models/service-order.interface';

@Injectable({
  providedIn: 'root'
})
export class ServiceOrdersService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/serviceorders`;

   getServiceOrders(params: ServiceOrderParams): Observable<Pagination<ServiceOrder>> {
    const queryParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString())
      .set('sort', params.sort || 'dateIn')
      .set('search', params.search || '' )     
      .set('clientId', params.clientId?.toString() || '')
       .set('excludeFinished', params.excludeFinished?.toString() || 'false')
       .set('excludeInvoiced', params.excludeInvoiced?.toString() || 'false')
      .set('status', params.status?.toString() || '');
   
    return this.http.get<Pagination<ServiceOrder>>(this.apiUrl, { params: queryParams });
  }

  getServiceOrderById(id: number): Observable<ServiceOrderDetails> {
    return this.http.get<ServiceOrderDetails>(`${this.apiUrl}/${id}`);
  }

  createServiceOrder(serviceOrder: CreateServiceOrderDto): Observable<ServiceOrderDetails> {
    return this.http.post<ServiceOrderDetails>(this.apiUrl, serviceOrder);
  }

  updateServiceOrder(id: number, serviceOrder: CreateServiceOrderDto): Observable<ServiceOrderDetails> {
    return this.http.put<ServiceOrderDetails>(`${this.apiUrl}/${id}`, serviceOrder);
  }

  deleteServiceOrder(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  moveToStage(moveToStageDto: MoveToStageDto): Observable<ServiceOrderDetails> {
    return this.http.post<ServiceOrderDetails>(`${this.apiUrl}/moveto`, moveToStageDto);
  }

  sendToTryIn(sendToTryInDto: SendToTryInDto): Observable<ServiceOrderDetails> {
    return this.http.post<ServiceOrderDetails>(`${this.apiUrl}/tryin`, sendToTryInDto);
  }

  finishOrders(finishOrderDto: FinishOrderDto): Observable<ServiceOrderDetails[]> {
    return this.http.post<ServiceOrderDetails[]>(`${this.apiUrl}/finish`, finishOrderDto);
  }

  getWorksOutForTryIn(days: number = 5): Observable<ServiceOrderAlert[]> {
    return this.http.get<ServiceOrderAlert[]>(`${this.apiUrl}/alert/tryin`, {
      params: new HttpParams().set('days', days.toString())
    });
  }

  reopenServiceOrder(id: number): Observable<ServiceOrderDetails> {
    return this.http.post<ServiceOrderDetails>(`${this.apiUrl}/${id}/reopen`, {});
  }
}