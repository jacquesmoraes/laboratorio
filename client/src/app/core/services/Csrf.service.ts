import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CsrfService {
  private tokenSignal = signal<string | null>(null);
  readonly token = this.tokenSignal.asReadonly();

  constructor(private http: HttpClient) {}

  async getToken(): Promise<string> {
  if (this.tokenSignal()) {
    return this.tokenSignal()!;
  }

  try {
    const response = await firstValueFrom(
      this.http.get<{ token: string }>(`${environment.apiUrl}/csrf/token`, {
        withCredentials: true,
      })
    );

    const token = response?.token ?? '';
    this.tokenSignal.set(token);
    return token;
  } catch (error) {
    console.error('[CSRF Service] Erro ao obter token CSRF:', error);
    return '';
  }
}


  clearToken(): void {
    this.tokenSignal.set(null);
  }
}
