import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Sector, SectorApiResponse } from '../models/sector.interface';

@Injectable({ providedIn: 'root' })
export class SectorService {
  private apiUrl = `${environment.apiUrl}/sectors`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Sector[]> {
    return this.http.get<SectorApiResponse[]>(this.apiUrl).pipe(
      map(apiSectors => 
        apiSectors.map(apiSector => ({
          id: apiSector.sectorId,
          name: apiSector.name,
          description: undefined,
          
        }))
      )
    );
  }

  getById(id: number): Observable<Sector> {
    return this.http.get<SectorApiResponse>(`${this.apiUrl}/${id}`).pipe(
      map(apiSector => ({
        id: apiSector.sectorId,
        name: apiSector.name,
        description: undefined,
        
      }))
    );
  }

  create(sector: Omit<Sector, 'id'>): Observable<Sector> {
    return this.http.post<SectorApiResponse>(this.apiUrl, {
      name: sector.name
    }).pipe(
      map(apiSector => ({
        id: apiSector.sectorId,
        name: apiSector.name
        
        
      }))
    );
  }

  update(id: number, sector: Omit<Sector, 'id'>): Observable<Sector> {
    return this.http.put<SectorApiResponse>(`${this.apiUrl}/${id}`, {
      sectorId: id,
      name: sector.name
    }).pipe(
      map(apiSector => ({
        id: apiSector.sectorId,
        name: apiSector.name
        
        
      }))
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}