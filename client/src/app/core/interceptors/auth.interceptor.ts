import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { from, throwError, Observable } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { HttpEvent } from '@angular/common/http';
import { CsrfService } from '../services/Csrf.service';

export const authInterceptor: HttpInterceptorFn = (
  request,
  next
): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);
  const csrfService = inject(CsrfService);

  const token = authService.token();

  
  if (['POST', 'PUT', 'PATCH', 'DELETE'].includes(request.method)) {
    return from(csrfService.getToken()).pipe(
      switchMap(csrfToken => {
        request = request.clone({
          withCredentials: true,
          setHeaders: {
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            ...(csrfToken ? { 'X-CSRF-TOKEN': csrfToken } : {})
          }
        });

        return next(request).pipe(
          catchError((error: HttpErrorResponse) => handleError(error, authService, csrfService))
        );
      })
    );
  }

  // Para métodos sem CSRF
  request = request.clone({
    withCredentials: true,
    setHeaders: {
      ...(token ? { Authorization: `Bearer ${token}` } : {})
     }
  });

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => handleError(error, authService, csrfService))
  );
};

function handleError(
  error: HttpErrorResponse,
  authService: AuthService,
  csrfService: CsrfService
) {
  if (error.status === 401) {
    authService.logout();
    csrfService.clearToken();
  }
  return throwError(() => error);
}
