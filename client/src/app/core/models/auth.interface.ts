export type UserRole = 'admin' | 'client';

// Request interfaces - DTOs do backend
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterAdminRequest {
  displayName: string;
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface RegisterClientRequest {
  clientId: number;
  displayName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface CompleteFirstAccessRequest {
  email: string;
  accessCode: string;
  newPassword: string;
  confirmNewPassword: string;
}

// Response interfaces - Records do backend
export interface UserInfo {
  userId: string;
  email: string;
  displayName: string;
  role: UserRole;
  clientId?: number;
  isFirstLogin: boolean;
  accessCode: string;
}
export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}
export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: UserInfo;
}

export interface ResendAccessCodeResponse {
  accessCode: string;
}

// State interface para gerenciamento interno
export interface AuthState {
  user: UserInfo | null;
  token: string | null;
  isAuthenticated: boolean;
}
export interface ForgotPasswordResponse {
  message: string;
}
export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  email: string;
  newPassword: string;
}

export interface ResetPasswordResponse {
  message: string;
}