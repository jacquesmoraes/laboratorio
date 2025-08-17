import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  LoginRequest,
  CompleteFirstAccessRequest,
  RegisterAdminRequest,
  RegisterClientRequest,
  ResendAccessCodeResponse,
  UserInfo,
  ChangePasswordRequest,
  
} from '../models/auth.interface';
import { ErrorMappingService } from './error.mapping.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'auth_token';
  private userKey = 'auth_user';
private authBaseUrl = 'https://localhost:7058';
  private userSignal = signal<UserInfo | null>(this.loadUser());
  private tokenSignal = signal<string | null>(this.loadToken());
private errorMapping = inject(ErrorMappingService);
  readonly isAuthenticated = computed(() => !!this.tokenSignal() && !!this.userSignal());
  readonly user = computed(() => this.userSignal());
  readonly token = computed(() => this.tokenSignal());

  constructor(private http: HttpClient, private router: Router) {}


  
  async login(dto: LoginRequest): Promise<{ success: boolean; error?: string }> {
    try {
      const res = await firstValueFrom(this.http.post<AuthResponse>(`${this.authBaseUrl}/auth/login`, dto));
      if (res) this.setSession(res);
      return { success: !!res };
    } catch (error) {
      return { 
        success: false, 
        error: this.errorMapping.getAuthErrorMessage(error) 
      };
    }
  }

  async completeFirstAccess(dto: CompleteFirstAccessRequest): Promise<{ success: boolean; error?: string }> {
    try {
      const res = await firstValueFrom(this.http.post<AuthResponse>(`${this.authBaseUrl}/auth/complete-first-access`, dto));
      if (res) this.setSession(res);
      return { success: !!res };
    } catch (error) {
      return { 
        success: false, 
        error: this.errorMapping.getAuthErrorMessage(error) 
      };
    }
  }

  async registerAdmin(dto: RegisterAdminRequest): Promise<boolean> {
    try {
      await firstValueFrom(this.http.post(`${this.authBaseUrl}/auth/register-admin`, dto));
      return true;
    } catch {
      return false;
    }
  }

  async registerClient(dto: RegisterClientRequest): Promise<boolean> {
    try {
      await firstValueFrom(this.http.post(`${this.authBaseUrl}/auth/register-client`, dto));
      return true;
    } catch {
      return false;
    }
  }

  async changePassword(request: ChangePasswordRequest): Promise<void> {
  await firstValueFrom(this.http.post(`${this.authBaseUrl}/auth/change-password`, request));
}

  async resendAccessCode(clientId: number): Promise<string | null> {
    try {
      const res = await firstValueFrom(this.http.post<ResendAccessCodeResponse>(`${this.authBaseUrl}/auth/resend-access-code/${clientId}`, {}));
      return res?.accessCode || null;
    } catch {
      return null;
    }
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.userSignal.set(null);
    this.tokenSignal.set(null);
    this.router.navigate(['/login']);
  }
  

  isAdmin(): boolean {
    return this.userSignal()?.role === 'admin';
  }

  isClient(): boolean {
    return this.userSignal()?.role === 'client';
  }

  isFirstLogin(): boolean {
    return this.userSignal()?.isFirstLogin ?? false;
  }

  getAccessCode(): string {
    return this.userSignal()?.accessCode ?? '';
  }

  
  private setSession(res: AuthResponse): void {
    localStorage.setItem(this.tokenKey, res.token);
    localStorage.setItem(this.userKey, JSON.stringify(res.user));
    this.tokenSignal.set(res.token);
    this.userSignal.set(res.user);
  }

  private loadToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private loadUser(): UserInfo | null {
    try {
      const user = localStorage.getItem(this.userKey);
      return user ? JSON.parse(user) : null;
    } catch {
      return null;
    }
  }
 
}